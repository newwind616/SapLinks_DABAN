using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness.DSC_B
{
    public class KPSQ : AfterSaleObject
    {
        List<string> list_sale = new List<string>();//销售集合
        List<string> list_purchase = new List<string>();//采购集合
        StringBuilder saleData = new StringBuilder();//销售生成文件
        StringBuilder purchaseData = new StringBuilder();//采购生成文件
        T_SAP_COMPANYFUNDS_LINKS_QUEUE queue = new T_SAP_COMPANYFUNDS_LINKS_QUEUE();

        public KPSQ(string filePath, string salePrefix, string prPrefix, string fileSuffix, string company, string sap_cd, BaseAction baseAction) : base(filePath, salePrefix, prPrefix, fileSuffix, company, sap_cd, baseAction)
        {
        }

        public override void GetData()
        {
            DoKPQS();
            DoKPSQ_WC();
            DoKPQS_CX();
            SaveToKPSQ(queue.GetSql_ISLINK1(), saleData.ToString(), purchaseData.ToString());
        }
        #region 开票申请生成采购、销售数据
        private void DoKPQS()
        {
            string sql = @"select AA.* from (
	select AA.TASKID,AA.FinishAt,AA.SerialNum,AA.XH,BB.Z_XH,BB.Z_ZP,BB.Z_GLBH,BB.Z_GLBH_PK,BB.Z_HTBH,AA.KHBM Z_KHBM,AA.KHMC Z_KHMC,BB.Z_YWLX,BB.Z_YYDD,BB.S_MC,BB.S_DJ,BB.S_KPJE,BB.S_SL,BB.S_BHSJE,BB.C_SAKNR,BB.C_SNWBMC,BB.C_WBJE,BB.C_WBCS,BB.C_CXZBBB,BB.CD,BB.MAIL,BB.MOBILE from (
		--开票信息，开票序号 开始
		select AA.TASKID,AA.FinishAt,AA.SerialNum,BB.ID,BB.XH,BB.KHBM,BB.KHMC from (
			select AA.TASKID,BB.FinishAt,BB.SerialNum from DABAN_BPM_DSC_B.[dbo].[KPSQ_C_H] AA 
			left join BPMDB.DBO.BPMInstTasks BB on AA.TASKID=BB.TaskID
			where AA.PROCESS_TYPE='0' and AA.YCWC_TYPE='0' and BB.State='Approved'
		) AA left join DABAN_BPM_DSC_B.[dbo].[KPSQ_FP_D] BB on AA.TASKID=BB.TASKID
		--开票信息，开票序号 结束
	) AA left join DABAN_BPM_DSC_B.[dbo].[KPSQ_ZYDXX_D] BB on AA.TASKID=BB.TASKID and AA.ID=BB.KPSQ_FP_D_ID
	--left join BPMDB.dbo.MAIN_CUSTOMER CC on AA.KHBM = CC.CSR_ID and CC.COMPANY='DSC_B'
--与Queue表关联，得到可以生成联携的数据
	) AA left join BPMDB.dbo.SAP_COMPANYFUNDS_LINKS_QUEUE BB on AA.SerialNum=BB.APPLY_NO
where BB.ISLINK=0";
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                queue.Add(Convert.ToString(dt.Rows[i]["SerialNum"]));
                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["C_CXZBBB"])) && VerificationPurchaseSaknr(Convert.ToString(dt.Rows[i]["C_SAKNR"])))
                {
                    DoPurchase(dt.Rows[i]);
                    KPSQPurchaseCount++;
                }
                DoSale(dt.Rows[i]);
                KPSQSaleCount++;
            }
        }
        private void DoPurchase(DataRow dr)
        {
            list_purchase.Clear();
            string z_glbh = Convert.ToString(dr["Z_GLBH"]);
            string sql = @"select {3} from [DABAN_BPM_{0}].[dbo].[MAIN_PURCHASE_I] 
                                where SAKNR like '{1}%'and CXZBBB='{2}'";
            string cd = Convert.ToString(dr["CD"]);
            string cxzbbb = Convert.ToString(dr["C_CXZBBB"]);
            string kostlSQL = string.Format(sql, cd, "410101150", cxzbbb, "KOSTL");
            string kostl = Convert.ToString(SQLHelperExt.ExecuteFirstValue(context.connStr, kostlSQL));//采购文件4101011502这个科目明细中的第2个字段
            string zuonrSQL = string.Format(sql, cd, "2121030000", cxzbbb, "ZUONR");
            string zuonr = Convert.ToString(SQLHelperExt.ExecuteFirstValue(context.connStr, zuonrSQL));//采购联动文件科目为”2121030000“的明细行的第四个字段，供应商代码
            decimal c_wbcs = Convert.ToDecimal(dr["C_WBCS"]);
            string serialnum = Convert.ToString(dr["SerialNum"]);
            string xh = Convert.ToString(dr["XH"]);
            string z_khmc = Convert.ToString(dr["Z_KHMC"]);
            decimal s_kpje = Convert.ToDecimal(dr["S_KPJE"]);
            string saknr = Convert.ToString(dr["C_SAKNR"]);//外包供应商编码
            string snwbmc = Convert.ToString(dr["C_SNWBMC"]);//外包供应商名称
            string z_ywlx = Convert.ToString(dr["Z_YWLX"]);
            string z_yydd = Convert.ToString(dr["Z_YYDD"]);
            string finishat = Convert.ToDateTime(dr["FinishAt"]).ToString("yyyyMMdd");
            string c_saknr = Convert.ToString(dr["C_SAKNR"]).Length > 3 ? Convert.ToString(dr["C_SAKNR"]).Substring(2, 1) : "";
            string sgtxt = string.Format("{0}+{1}{2}（{3}）", c_saknr, z_ywlx, z_khmc, s_kpje.ToString());
            string bktxt = saknr + snwbmc;//抬头摘要
            #region 第一条
            list_purchase.Add(z_glbh);//参照号（XBLNR）
            list_purchase.Add(finishat);//凭证日期（BLDAT）
            list_purchase.Add(finishat);//记账日期（BUDAT）
            list_purchase.Add(bktxt);//抬头摘要（BKTXT）
            list_purchase.Add("CNY");//币种（WAERS）
            list_purchase.Add("1");//汇率（KURSF）
            list_purchase.Add("F66666666");//客户 / 供应商 / 会计科目代码（NEWKO）
            list_purchase.Add("31");//记账码（NEWBS）
            list_purchase.Add("");//特殊总账标识（NEWUM）
            list_purchase.Add("");//資産取引タイプ（NEWBW）
            list_purchase.Add("");//成本中心（KOSTL）
            list_purchase.Add("");//利润中心（PRCTR）
            list_purchase.Add("");//WBS要素（PROJK）
            list_purchase.Add("");//内部订单号（AUFNR）
            list_purchase.Add(c_wbcs.ToString());//凭证货币金额（WRBTR）
            list_purchase.Add("");//本地货币金额（DMBTR）
            list_purchase.Add("");//税码（MWSKZ）
            list_purchase.Add(z_glbh);//分配（ZUONR）
            list_purchase.Add(sgtxt);//明細テキスト（SGTXT）
            list_purchase.Add(zuonr);//取引先参照キー１（XREF1）
            list_purchase.Add("");//取引先参照キー２（XREF2）
            list_purchase.Add("");//取引先参照キー３（XREF3）
            list_purchase.Add(finishat);//支払基準日（ZFBDT）
            list_purchase.Add("");//支払条件（ZTERM）
            list_purchase.Add("");//支払方法（ZLSCH）
            list_purchase.Add("");//支払保留（ZLSPR）
            list_purchase.Add("");//日数（ZBD1T）
            list_purchase.Add("");//取引銀行ID（HBKID）
            list_purchase.Add("");//銀行タイプ（BVTYP）
            list_purchase.Add("");//起算日（VALUT）
            list_purchase.Add("");//手形振出日（WDATE）
            list_purchase.Add("");//銀行住所（WBANK） 
            #endregion
            purchaseData.AppendLine(list_purchase.ToTSVString());
            list_purchase.Clear();
            #region 第二条
            list_purchase.Add(z_glbh);//参照号（XBLNR）
            list_purchase.Add(finishat);//凭证日期（BLDAT）
            list_purchase.Add(finishat);//记账日期（BUDAT）
            list_purchase.Add(bktxt);//抬头摘要（BKTXT）
            list_purchase.Add("CNY");//币种（WAERS）
            list_purchase.Add("1");//汇率（KURSF） 
            list_purchase.Add("4101011502");//客户 / 供应商 / 会计科目代码（NEWKO）
            list_purchase.Add("40");//记账码（NEWBS）
            list_purchase.Add("");//特殊总账标识（NEWUM）
            list_purchase.Add("");//資産取引タイプ（NEWBW）
            list_purchase.Add(GetKostl(kostl));//成本中心（KOSTL）
            list_purchase.Add(GetKostl(kostl));//利润中心（PRCTR）
            list_purchase.Add("");//WBS要素（PROJK）
            list_purchase.Add("");//内部订单号（AUFNR）
            list_purchase.Add(c_wbcs.ToString());//凭证货币金额（WRBTR）
            list_purchase.Add("");//本地货币金额（DMBTR）
            list_purchase.Add("");//税码（MWSKZ）
            list_purchase.Add(z_glbh);//分配（ZUONR）
            list_purchase.Add(sgtxt);//明細テキスト（SGTXT）
            list_purchase.Add("X/" + z_ywlx);//取引先参照キー１（XREF1）
            list_purchase.Add(z_yydd);//取引先参照キー２（XREF2）
            list_purchase.Add("");//取引先参照キー３（XREF3）
            list_purchase.Add("");//支払基準日（ZFBDT）
            list_purchase.Add("");//支払条件（ZTERM）
            list_purchase.Add("");//支払方法（ZLSCH）
            list_purchase.Add("");//支払保留（ZLSPR）
            list_purchase.Add("");//日数（ZBD1T）
            list_purchase.Add("");//取引銀行ID（HBKID）
            list_purchase.Add("");//銀行タイプ（BVTYP）
            list_purchase.Add("");//起算日（VALUT）
            list_purchase.Add("");//手形振出日（WDATE）
            list_purchase.Add("");//銀行住所（WBANK） 
            #endregion
            purchaseData.AppendLine(list_purchase.ToTSVString());
        }
        private void DoSale(DataRow dr)
        {
            list_sale.Clear();
            #region 查询DOCS数据获得
            string cd = Convert.ToString(dr["CD"]);
            string z_glbh_pk = Convert.ToString(dr["Z_GLBH_PK"]);
            string sql = string.Format("select BSTZD2,VTWEG,CITY1,WAERK,ZZVKBUR,ZZPRCTR,ZZJGY,TEL_NUMBER,MAIL,MOBILE from DABAN_BPM_{0}.dbo.MAIN_INCOME_H where CBXZ='{1}'", cd, z_glbh_pk);
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            sql = string.Format("select CITY2 from DABAN_BPM_{0}.dbo.MAIN_INCOME_H where CBXZ='{1}'", cd, z_glbh_pk);//客户国家代码
            string kna1land1 = Convert.ToString(SQLHelperExt.ExecuteFirstValue(context.connStr, sql));
            string bstzd2 = Convert.ToString(dt.Rows[0]["BSTZD2"]);//原价区分
            string vtweg = Convert.ToString(dt.Rows[0]["VTWEG"]);//分销渠道
            string city1 = Convert.ToString(dt.Rows[0]["CITY1"]);//客户城市
            string waerk = Convert.ToString(dt.Rows[0]["WAERK"]);//凭证货币
            string zzvkbur = Convert.ToString(dt.Rows[0]["ZZVKBUR"]);//联络所
            string zzprctr = Convert.ToString(dt.Rows[0]["ZZPRCTR"]);//利润中心
            string zzjgy = Convert.ToString(dt.Rows[0]["ZZJGY"]);//事业别
            string tel_number = Convert.ToString(dt.Rows[0]["TEL_NUMBER"]);//客户电话号码
            string mail = Convert.ToString(dr["MAIL"]);
            string mobile = Convert.ToString(dr["MOBILE"]);
            #endregion
            string z_glbh = Convert.ToString(dr["Z_GLBH"]);
            string z_khbm = Convert.ToString(dr["Z_KHBM"]);
            string z_khmc = Convert.ToString(dr["Z_KHMC"]);
            //string kna1land1 = Convert.ToString(dr["KNA1LAND1"]);//客户国家代码
            string serialnum = Convert.ToString(dr["SerialNum"]);//
            string xh = Convert.ToString(dr["XH"]);
            string zxh = Convert.ToString(dr["Z_XH"]);
            int s_sl = Convert.ToInt32(dr["S_SL"]);//税率 
            string finishat = Convert.ToDateTime(dr["FinishAt"]).ToString("yyyyMMdd");
            string z_yydd = Convert.ToString(dr["Z_YYDD"]);
            string s_mc = Convert.ToString(dr["S_MC"]);
            decimal s_kpje = Convert.ToDecimal(dr["S_KPJE"]);
            #region 第一条
            list_sale.Add("H");//标识符
            list_sale.Add(z_glbh);//管理 / 销售编号
            list_sale.Add("0");//正负
            list_sale.Add("D");//DRBR区分
            list_sale.Add(bstzd2);//原价区分
            list_sale.Add(sap_cd);//销售组织
            list_sale.Add(vtweg);//分销渠道
            list_sale.Add(z_khbm);//客户编码
            list_sale.Add(z_khmc);//客户名称
            list_sale.Add(city1);//客户城市
            list_sale.Add(kna1land1);//客户国家代码
            list_sale.Add(serialnum);//客户的采购号
            list_sale.Add(waerk);//凭证货币
            list_sale.Add(finishat);//定价日期
            list_sale.Add("");//机种
            list_sale.Add(z_glbh);//销售单号
            list_sale.Add(zzvkbur);//联络所
            list_sale.Add(zzprctr);//利润中心
            list_sale.Add(zzjgy);//事业别
            list_sale.Add(z_yydd);//营业担当
            list_sale.Add(tel_number);//客户电话号码 
            if (mailMobile)
            {
                list_sale.Add(mail);//邮箱
                list_sale.Add(mobile);//手机号 
            }
            #endregion
            saleData.AppendLine(list_sale.ToTSVString());
            list_sale.Clear();
            #region 第二条
            list_sale.Add("I");//标识符
            list_sale.Add(z_glbh);//管理 / 销售编号
            list_sale.Add("10");//明细号
            list_sale.Add("ZDSC_SERV_1");//物料号
            list_sale.Add(s_mc);//物料描述
            list_sale.Add(sap_cd);//工厂
            list_sale.Add("1");//数量
            list_sale.Add("PC");//数量单位
            list_sale.Add(s_kpje.ToString());//含税总价
            list_sale.Add(s_sl.ToString());//税率百分数
            list_sale.Add(zzprctr);//利润中心 
            #endregion
            saleData.AppendLine(list_sale.ToTSVString());
        }
        #endregion
        #region 开票申请冲销生成采购、销售数据
        private void DoKPQS_CX()
        {
            string sql = @"select AA.* from (
	select AA.TASKID,AA.FinishAt,AA.SerialNum,AA.KPSQ_FP_D_XH XH,BB.Z_XH,BB.Z_ZP,BB.Z_GLBH,BB.Z_GLBH_PK,BB.Z_HTBH,AA.KHBM Z_KHBM,AA.KHMC Z_KHMC,BB.Z_YWLX,BB.Z_YYDD,BB.S_MC,BB.S_DJ,BB.S_KPJE,BB.S_SL,BB.S_BHSJE,BB.C_SAKNR,BB.C_SNWBMC,BB.C_WBJE,BB.C_WBCS,BB.C_CXZBBB,BB.CD from (
		select AA.TASKID,AA.FinishAt,AA.SerialNum,BB.ID,BB.XH,BB.KPSQ_FP_D_XH,BB.KHBM,BB.KHMC from (
			--开票信息冲销，开票序号 开始
			select AA.TASKID,BB.FinishAt,BB.SerialNum from DABAN_BPM_DSC_B.[dbo].[KPSQ_C_H] AA 
			left join BPMDB.DBO.BPMInstTasks BB on AA.TASKID=BB.TaskID
			where AA.PROCESS_TYPE='1' and AA.YCWC_TYPE='0' and BB.State='Approved'
			--开票信息冲销，开票序号 开始
		) AA left join DABAN_BPM_DSC_B.[dbo].[KPSQ_FP_D] BB on AA.TASKID=BB.TASKID
	) AA left join DABAN_BPM_DSC_B.[dbo].[KPSQ_ZYDXX_D] BB on AA.TASKID=BB.TASKID and AA.ID=BB.KPSQ_FP_D_ID
	--left join BPMDB.dbo.MAIN_CUSTOMER CC on AA.KHBM = CC.CSR_ID and CC.COMPANY='DSC_B'
--与Queue表关联，得到可以生成联携的数据
) AA left join BPMDB.dbo.SAP_COMPANYFUNDS_LINKS_QUEUE BB on AA.SerialNum=BB.APPLY_NO
where BB.ISLINK=0";
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                queue.Add(Convert.ToString(dt.Rows[i]["SerialNum"]));
                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["C_CXZBBB"])) && VerificationPurchaseSaknr(Convert.ToString(dt.Rows[i]["C_SAKNR"])))
                {
                    DoPurchase_CX(dt.Rows[i]);
                    KPSQCXPurchaseCount++;
                }
                DoSale_CX(dt.Rows[i]);
                KPSQCXSaleCount++;
            }
        }
        private void DoPurchase_CX(DataRow dr)
        {
            list_purchase.Clear();
            string z_glbh = Convert.ToString(dr["Z_GLBH"]);
            string sql = @"select {3} from [DABAN_BPM_{0}].[dbo].[MAIN_PURCHASE_I] 
                                where SAKNR like '{1}%'and CXZBBB='{2}'";
            string cd = Convert.ToString(dr["CD"]);
            string cxzbbb = Convert.ToString(dr["C_CXZBBB"]);
            string kostlSQL = string.Format(sql, cd, "410101150", cxzbbb, "KOSTL");
            string kostl = Convert.ToString(SQLHelperExt.ExecuteFirstValue(context.connStr, kostlSQL));//采购文件4101011502这个科目明细中的第2个字段
            string zuonrSQL = string.Format(sql, cd, "2121030000", cxzbbb, "ZUONR");
            string zuonr = Convert.ToString(SQLHelperExt.ExecuteFirstValue(context.connStr, zuonrSQL));//采购联动文件科目为”2121030000“的明细行的第四个字段，供应商代码
            decimal c_wbcs = Convert.ToDecimal(dr["C_WBCS"]);
            string serialnum = Convert.ToString(dr["SerialNum"]);
            string xh = Convert.ToString(dr["XH"]);
            string z_khmc = Convert.ToString(dr["Z_KHMC"]);
            decimal s_kpje = Convert.ToDecimal(dr["S_KPJE"]);
            string saknr = Convert.ToString(dr["C_SAKNR"]);//外包供应商编码
            string snwbmc = Convert.ToString(dr["C_SNWBMC"]);//外包供应商名称
            string z_ywlx = Convert.ToString(dr["Z_YWLX"]);
            string z_yydd = Convert.ToString(dr["Z_YYDD"]);
            string finishat = Convert.ToDateTime(dr["FinishAt"]).ToString("yyyyMMdd");
            string c_saknr = Convert.ToString(dr["C_SAKNR"]).Length > 3 ? Convert.ToString(dr["C_SAKNR"]).Substring(2, 1) : "";
            string sgtxt = string.Format("{0}+{1}{2}（{3}）", c_saknr, z_ywlx, z_khmc, s_kpje.ToString());
            string bktxt = saknr + snwbmc;//抬头摘要
            #region 第一条
            list_purchase.Add(z_glbh);//参照号（XBLNR）
            list_purchase.Add(finishat);//凭证日期（BLDAT）
            list_purchase.Add(finishat);//记账日期（BUDAT）
            list_purchase.Add(bktxt);//抬头摘要（BKTXT）
            list_purchase.Add("CNY");//币种（WAERS）
            list_purchase.Add("1");//汇率（KURSF）
            list_purchase.Add("F66666666");//客户 / 供应商 / 会计科目代码（NEWKO）
            list_purchase.Add("25");//记账码（NEWBS）
            list_purchase.Add("");//特殊总账标识（NEWUM）
            list_purchase.Add("");//資産取引タイプ（NEWBW）
            list_purchase.Add("");//成本中心（KOSTL）
            list_purchase.Add("");//利润中心（PRCTR）
            list_purchase.Add("");//WBS要素（PROJK）
            list_purchase.Add("");//内部订单号（AUFNR）
            list_purchase.Add(c_wbcs.ToString());//凭证货币金额（WRBTR）
            list_purchase.Add("");//本地货币金额（DMBTR）
            list_purchase.Add("");//税码（MWSKZ）
            list_purchase.Add(z_glbh);//分配（ZUONR）
            list_purchase.Add(sgtxt);//明細テキスト（SGTXT）
            list_purchase.Add(zuonr);//取引先参照キー１（XREF1）
            list_purchase.Add("");//取引先参照キー２（XREF2）
            list_purchase.Add("");//取引先参照キー３（XREF3）
            list_purchase.Add(finishat);//支払基準日（ZFBDT）
            list_purchase.Add("");//支払条件（ZTERM）
            list_purchase.Add("");//支払方法（ZLSCH）
            list_purchase.Add("");//支払保留（ZLSPR）
            list_purchase.Add("");//日数（ZBD1T）
            list_purchase.Add("");//取引銀行ID（HBKID）
            list_purchase.Add("");//銀行タイプ（BVTYP）
            list_purchase.Add("");//起算日（VALUT）
            list_purchase.Add("");//手形振出日（WDATE）
            list_purchase.Add("");//銀行住所（WBANK） 
            #endregion
            purchaseData.AppendLine(list_purchase.ToTSVString());
            list_purchase.Clear();
            #region 第二条
            list_purchase.Add(z_glbh);//参照号（XBLNR）
            list_purchase.Add(finishat);//凭证日期（BLDAT）
            list_purchase.Add(finishat);//记账日期（BUDAT）
            list_purchase.Add(bktxt);//抬头摘要（BKTXT）
            list_purchase.Add("CNY");//币种（WAERS）
            list_purchase.Add("1");//汇率（KURSF） 
            list_purchase.Add("4101011502");//客户 / 供应商 / 会计科目代码（NEWKO）
            list_purchase.Add("50");//记账码（NEWBS）
            list_purchase.Add("");//特殊总账标识（NEWUM）
            list_purchase.Add("");//資産取引タイプ（NEWBW）
            list_purchase.Add(GetKostl(kostl));//成本中心（KOSTL）
            list_purchase.Add(GetKostl(kostl));//利润中心（PRCTR）
            list_purchase.Add("");//WBS要素（PROJK）
            list_purchase.Add("");//内部订单号（AUFNR）
            list_purchase.Add(c_wbcs.ToString());//凭证货币金额（WRBTR）
            list_purchase.Add("");//本地货币金额（DMBTR）
            list_purchase.Add("");//税码（MWSKZ）
            list_purchase.Add(z_glbh);//分配（ZUONR）
            list_purchase.Add(sgtxt);//明細テキスト（SGTXT）
            list_purchase.Add("X/" + z_ywlx);//取引先参照キー１（XREF1）
            list_purchase.Add(z_yydd);//取引先参照キー２（XREF2）
            list_purchase.Add("");//取引先参照キー３（XREF3）
            list_purchase.Add("");//支払基準日（ZFBDT）
            list_purchase.Add("");//支払条件（ZTERM）
            list_purchase.Add("");//支払方法（ZLSCH）
            list_purchase.Add("");//支払保留（ZLSPR）
            list_purchase.Add("");//日数（ZBD1T）
            list_purchase.Add("");//取引銀行ID（HBKID）
            list_purchase.Add("");//銀行タイプ（BVTYP）
            list_purchase.Add("");//起算日（VALUT）
            list_purchase.Add("");//手形振出日（WDATE）
            list_purchase.Add("");//銀行住所（WBANK） 
            #endregion
            purchaseData.AppendLine(list_purchase.ToTSVString());
        }
        private void DoSale_CX(DataRow dr)
        {
            list_sale.Clear();
            #region 查询DOCS数据获得
            string cd = Convert.ToString(dr["CD"]);
            string z_glbh_pk = Convert.ToString(dr["Z_GLBH_PK"]);
            string sql = string.Format("select BSTZD2,VTWEG,CITY1,WAERK,ZZVKBUR,ZZPRCTR,ZZJGY,TEL_NUMBER,MAIL,MOBILE from DABAN_BPM_{0}.dbo.MAIN_INCOME_H where CBXZ='{1}'", cd, z_glbh_pk);
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            sql = string.Format("select CITY2 from DABAN_BPM_{0}.dbo.MAIN_INCOME_H where CBXZ='{1}'", cd, z_glbh_pk);//客户国家代码
            string kna1land1 = Convert.ToString(SQLHelperExt.ExecuteFirstValue(context.connStr, sql));
            string bstzd2 = Convert.ToString(dt.Rows[0]["BSTZD2"]);//原价区分
            string vtweg = Convert.ToString(dt.Rows[0]["VTWEG"]);//分销渠道
            string city1 = Convert.ToString(dt.Rows[0]["CITY1"]);//客户城市
            string waerk = Convert.ToString(dt.Rows[0]["WAERK"]);//凭证货币
            string zzvkbur = Convert.ToString(dt.Rows[0]["ZZVKBUR"]);//联络所
            string zzprctr = Convert.ToString(dt.Rows[0]["ZZPRCTR"]);//利润中心
            string zzjgy = Convert.ToString(dt.Rows[0]["ZZJGY"]);//事业别
            string tel_number = Convert.ToString(dt.Rows[0]["TEL_NUMBER"]);//客户电话号码
            string mail = Convert.ToString(dt.Rows[0]["MAIL"]);
            string mobile = Convert.ToString(dt.Rows[0]["MOBILE"]);
            #endregion
            string z_glbh = Convert.ToString(dr["Z_GLBH"]);
            string z_khbm = Convert.ToString(dr["Z_KHBM"]);
            string z_khmc = Convert.ToString(dr["Z_KHMC"]);
            //string kna1land1 = Convert.ToString(dr["KNA1LAND1"]);//客户国家代码
            string serialnum = Convert.ToString(dr["SerialNum"]);//
            string xh = Convert.ToString(dr["XH"]);
            string zxh = Convert.ToString(dr["Z_XH"]);
            int s_sl = Convert.ToInt32(dr["S_SL"]);//税率 
            string finishat = Convert.ToDateTime(dr["FinishAt"]).ToString("yyyyMMdd");
            string z_yydd = Convert.ToString(dr["Z_YYDD"]);
            string s_mc = Convert.ToString(dr["S_MC"]);
            decimal s_kpje = Convert.ToDecimal(dr["S_KPJE"]);
            #region 第一条
            list_sale.Add("H");//标识符
            list_sale.Add(z_glbh);//管理 / 销售编号
            list_sale.Add("1");//正负
            list_sale.Add("D");//DRBR区分
            list_sale.Add(bstzd2);//原价区分
            list_sale.Add(sap_cd);//销售组织
            list_sale.Add(vtweg);//分销渠道
            list_sale.Add(z_khbm);//客户编码
            list_sale.Add(z_khmc);//客户名称
            list_sale.Add(city1);//客户城市
            list_sale.Add(kna1land1);//客户国家代码
            list_sale.Add(serialnum);//客户的采购号
            list_sale.Add(waerk);//凭证货币
            list_sale.Add(finishat);//定价日期
            list_sale.Add("");//机种
            list_sale.Add(z_glbh);//销售单号
            list_sale.Add(zzvkbur);//联络所
            list_sale.Add(zzprctr);//利润中心
            list_sale.Add(zzjgy);//事业别
            list_sale.Add(z_yydd);//营业担当
            list_sale.Add(tel_number);//客户电话号码 
            if (mailMobile)
            {
                list_sale.Add(mail);//邮箱
                list_sale.Add(mobile);//手机号 
            }
            #endregion
            saleData.AppendLine(list_sale.ToTSVString());
            list_sale.Clear();
            #region 第二条
            list_sale.Add("I");//标识符
            list_sale.Add(z_glbh);//管理 / 销售编号
            list_sale.Add("10");//明细号
            list_sale.Add("ZDSC_SERV_1");//物料号
            list_sale.Add(s_mc);//物料描述
            list_sale.Add(sap_cd);//工厂
            list_sale.Add("1");//数量
            list_sale.Add("PC");//数量单位
            list_sale.Add(s_kpje.ToString());//含税总价
            list_sale.Add(s_sl.ToString());//税率百分数
            list_sale.Add(zzprctr);//利润中心 
            #endregion
            saleData.AppendLine(list_sale.ToTSVString());
        }
        #endregion

        #region 开票申请无偿生成采购、销售数据
        private void DoKPSQ_WC()
        {
            string sql = @"select AA.* from (
	select AA.TASKID,AA.FinishAt,AA.SerialNum,AA.XH,BB.Z_XH,BB.Z_ZP,BB.Z_GLBH,BB.Z_GLBH_PK,BB.Z_HTBH,AA.KHBM Z_KHBM,AA.KHMC Z_KHMC,BB.Z_YWLX,BB.Z_YYDD,BB.S_MC,BB.S_DJ,BB.S_KPJE,BB.S_SL,BB.S_BHSJE,BB.C_SAKNR,BB.C_SNWBMC,BB.C_WBJE,BB.C_WBCS,BB.C_CXZBBB,BB.CD from (
		--开票信息，开票序号 开始
		select AA.TASKID,AA.FinishAt,AA.SerialNum,BB.ID,BB.XH,BB.KHBM,BB.KHMC from (
			select AA.TASKID,BB.FinishAt,BB.SerialNum from DABAN_BPM_DSC_B.[dbo].[KPSQ_C_H] AA 
			left join BPMDB.DBO.BPMInstTasks BB on AA.TASKID=BB.TaskID
			where AA.PROCESS_TYPE='0' and AA.YCWC_TYPE='1' and BB.State='Approved'
		) AA left join DABAN_BPM_DSC_B.[dbo].[KPSQ_FP_D] BB on AA.TASKID=BB.TASKID
		--开票信息，开票序号 结束
	) AA left join DABAN_BPM_DSC_B.[dbo].[KPSQ_ZYDXX_D] BB on AA.TASKID=BB.TASKID and AA.ID=BB.KPSQ_FP_D_ID
	--left join BPMDB.dbo.MAIN_CUSTOMER CC on AA.KHBM = CC.CSR_ID and CC.COMPANY='DSC_B'
--与Queue表关联，得到可以生成联携的数据
	) AA left join BPMDB.dbo.SAP_COMPANYFUNDS_LINKS_QUEUE BB on AA.SerialNum=BB.APPLY_NO
where BB.ISLINK=0";
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                queue.Add(Convert.ToString(dt.Rows[i]["SerialNum"]));
                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["C_CXZBBB"])) && VerificationPurchaseSaknr(Convert.ToString(dt.Rows[i]["C_SAKNR"])))
                {
                    DoPurchase_WC(dt.Rows[i]);
                    KPSQPurchaseCount++;
                }
                DoSale_WC(dt.Rows[i]);
                KPSQSaleCount++;
            }
        }
        private void DoPurchase_WC(DataRow dr)
        {
            list_purchase.Clear();
            string z_glbh_pk = Convert.ToString(dr["Z_GLBH_PK"]);
            string z_glbh = Convert.ToString(dr["Z_GLBH"]);
            string sql = @"select {3} from [DABAN_BPM_{0}].[dbo].[MAIN_PURCHASE_I] 
                                where SAKNR like '{1}%'and CXZBBB='{2}'";
            string cd = Convert.ToString(dr["CD"]);
            string cxzbbb = Convert.ToString(dr["C_CXZBBB"]);
            string kostlSQL = string.Format(sql, cd, "410101150", cxzbbb, "KOSTL");
            string kostl = Convert.ToString(SQLHelperExt.ExecuteFirstValue(context.connStr, kostlSQL));//采购文件4101011502这个科目明细中的第2个字段
            string zuonrSQL = string.Format(sql, cd, "2121030000", cxzbbb, "ZUONR");
            string zuonr = Convert.ToString(SQLHelperExt.ExecuteFirstValue(context.connStr, zuonrSQL));//采购联动文件科目为”2121030000“的明细行的第四个字段，供应商代码
            decimal c_wbcs = Convert.ToDecimal(dr["C_WBCS"]);
            string serialnum = Convert.ToString(dr["SerialNum"]);
            string xh = Convert.ToString(dr["XH"]);
            string z_khmc = Convert.ToString(dr["Z_KHMC"]);
            decimal s_kpje = Convert.ToDecimal(dr["S_KPJE"]);
            string saknr = Convert.ToString(dr["C_SAKNR"]);//外包供应商编码
            string snwbmc = Convert.ToString(dr["C_SNWBMC"]);//外包供应商名称
            string z_ywlx = Convert.ToString(dr["Z_YWLX"]);
            string z_yydd = Convert.ToString(dr["Z_YYDD"]);
            string finishat = Convert.ToDateTime(dr["FinishAt"]).ToString("yyyyMMdd");
            string c_saknr = Convert.ToString(dr["C_SAKNR"]).Length > 3 ? Convert.ToString(dr["C_SAKNR"]).Substring(2, 1) : "";
            string sgtxt = string.Format("{0}+{1}{2}（{3}）", c_saknr, z_ywlx, z_khmc, s_kpje.ToString());
            string bktxt = saknr + snwbmc;//抬头摘要
            string zf = z_glbh_pk.Substring(z_glbh_pk.Length - 1, 1);//最后一位为正负标志位
            #region 第一条
            list_purchase.Add(z_glbh);//参照号（XBLNR）
            list_purchase.Add(finishat);//凭证日期（BLDAT）
            list_purchase.Add(finishat);//记账日期（BUDAT）
            list_purchase.Add(bktxt);//抬头摘要（BKTXT）
            list_purchase.Add("CNY");//币种（WAERS）
            list_purchase.Add("1");//汇率（KURSF）
            list_purchase.Add("F55555555");//客户 / 供应商 / 会计科目代码（NEWKO）
            if (zf == "0")
                list_purchase.Add("31");//记账码（NEWBS）
            else
                list_purchase.Add("25");//记账码（NEWBS）
            list_purchase.Add("");//特殊总账标识（NEWUM）
            list_purchase.Add("");//資産取引タイプ（NEWBW）
            list_purchase.Add("");//成本中心（KOSTL）
            list_purchase.Add("");//利润中心（PRCTR）
            list_purchase.Add("");//WBS要素（PROJK）
            list_purchase.Add("");//内部订单号（AUFNR）
            list_purchase.Add(c_wbcs.ToString());//凭证货币金额（WRBTR）
            list_purchase.Add("");//本地货币金额（DMBTR）
            list_purchase.Add("");//税码（MWSKZ）
            list_purchase.Add(z_glbh);//分配（ZUONR）
            list_purchase.Add(sgtxt);//明細テキスト（SGTXT）
            list_purchase.Add(zuonr);//取引先参照キー１（XREF1）
            list_purchase.Add("");//取引先参照キー２（XREF2）
            list_purchase.Add("");//取引先参照キー３（XREF3）
            list_purchase.Add(finishat);//支払基準日（ZFBDT）
            list_purchase.Add("");//支払条件（ZTERM）
            list_purchase.Add("");//支払方法（ZLSCH）
            list_purchase.Add("");//支払保留（ZLSPR）
            list_purchase.Add("");//日数（ZBD1T）
            list_purchase.Add("");//取引銀行ID（HBKID）
            list_purchase.Add("");//銀行タイプ（BVTYP）
            list_purchase.Add("");//起算日（VALUT）
            list_purchase.Add("");//手形振出日（WDATE）
            list_purchase.Add("");//銀行住所（WBANK） 
            #endregion
            purchaseData.AppendLine(list_purchase.ToTSVString());
            list_purchase.Clear();
            #region 第二条
            list_purchase.Add(z_glbh);//参照号（XBLNR）
            list_purchase.Add(finishat);//凭证日期（BLDAT）
            list_purchase.Add(finishat);//记账日期（BUDAT）
            list_purchase.Add(bktxt);//抬头摘要（BKTXT）
            list_purchase.Add("CNY");//币种（WAERS）
            list_purchase.Add("1");//汇率（KURSF） 
            list_purchase.Add("4101011501");//客户 / 供应商 / 会计科目代码（NEWKO）
            if (zf == "0")
                list_purchase.Add("40");//记账码（NEWBS）
            else
                list_purchase.Add("50");//记账码（NEWBS）
            list_purchase.Add("");//特殊总账标识（NEWUM）
            list_purchase.Add("");//資産取引タイプ（NEWBW）
            list_purchase.Add(GetKostl(kostl));//成本中心（KOSTL）
            list_purchase.Add(GetKostl(kostl));//利润中心（PRCTR）
            list_purchase.Add("");//WBS要素（PROJK）
            list_purchase.Add("");//内部订单号（AUFNR）
            list_purchase.Add(c_wbcs.ToString());//凭证货币金额（WRBTR）
            list_purchase.Add("");//本地货币金额（DMBTR）
            list_purchase.Add("");//税码（MWSKZ）
            list_purchase.Add(z_glbh);//分配（ZUONR）
            list_purchase.Add(sgtxt);//明細テキスト（SGTXT）
            list_purchase.Add("X/" + z_ywlx);//取引先参照キー１（XREF1）
            list_purchase.Add(z_yydd);//取引先参照キー２（XREF2）
            list_purchase.Add("");//取引先参照キー３（XREF3）
            list_purchase.Add("");//支払基準日（ZFBDT）
            list_purchase.Add("");//支払条件（ZTERM）
            list_purchase.Add("");//支払方法（ZLSCH）
            list_purchase.Add("");//支払保留（ZLSPR）
            list_purchase.Add("");//日数（ZBD1T）
            list_purchase.Add("");//取引銀行ID（HBKID）
            list_purchase.Add("");//銀行タイプ（BVTYP）
            list_purchase.Add("");//起算日（VALUT）
            list_purchase.Add("");//手形振出日（WDATE）
            list_purchase.Add("");//銀行住所（WBANK） 
            #endregion
            purchaseData.AppendLine(list_purchase.ToTSVString());
        }
        private void DoSale_WC(DataRow dr)
        {
            list_sale.Clear();
            #region 查询DOCS数据获得
            string cd = Convert.ToString(dr["CD"]);
            string z_glbh_pk = Convert.ToString(dr["Z_GLBH_PK"]);
            string sql = string.Format("select BSTZD2,VTWEG,CITY1,WAERK,ZZVKBUR,ZZPRCTR,ZZJGY,TEL_NUMBER,MAIL,MOBILE from DABAN_BPM_{0}.dbo.MAIN_INCOME_H where CBXZ='{1}'", cd, z_glbh_pk);
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            sql = string.Format("select CITY2 from DABAN_BPM_{0}.dbo.MAIN_INCOME_H where CBXZ='{1}'", cd, z_glbh_pk);//客户国家代码
            string kna1land1 = Convert.ToString(SQLHelperExt.ExecuteFirstValue(context.connStr, sql));
            string bstzd2 = Convert.ToString(dt.Rows[0]["BSTZD2"]);//原价区分
            string vtweg = Convert.ToString(dt.Rows[0]["VTWEG"]);//分销渠道
            string city1 = Convert.ToString(dt.Rows[0]["CITY1"]);//客户城市
            string waerk = Convert.ToString(dt.Rows[0]["WAERK"]);//凭证货币
            string zzvkbur = Convert.ToString(dt.Rows[0]["ZZVKBUR"]);//联络所
            string zzprctr = Convert.ToString(dt.Rows[0]["ZZPRCTR"]);//利润中心
            string zzjgy = Convert.ToString(dt.Rows[0]["ZZJGY"]);//事业别
            string tel_number = Convert.ToString(dt.Rows[0]["TEL_NUMBER"]);//客户电话号码
            string mail = Convert.ToString(dt.Rows[0]["MAIL"]);
            string mobile = Convert.ToString(dt.Rows[0]["MOBILE"]);
            #endregion
            string z_glbh = Convert.ToString(dr["Z_GLBH"]);
            string z_khbm = Convert.ToString(dr["Z_KHBM"]);
            string z_khmc = Convert.ToString(dr["Z_KHMC"]);
            //string kna1land1 = Convert.ToString(dr["KNA1LAND1"]);//客户国家代码
            string serialnum = Convert.ToString(dr["SerialNum"]);//
            string xh = Convert.ToString(dr["XH"]);
            string zxh = Convert.ToString(dr["Z_XH"]);
            int s_sl = Convert.ToInt32(dr["S_SL"]);//税率 
            string finishat = Convert.ToDateTime(dr["FinishAt"]).ToString("yyyyMMdd");
            string z_yydd = Convert.ToString(dr["Z_YYDD"]);
            //string s_mc = Convert.ToString(dr["S_MC"]);
            decimal s_kpje = Convert.ToDecimal(dr["S_KPJE"]);
            string zf = z_glbh_pk.Substring(z_glbh_pk.Length - 1, 1);//最后一位为正负标志位
            #region 第一条
            list_sale.Add("H");//标识符
            list_sale.Add(z_glbh_pk.Substring(z_glbh_pk.IndexOf(z_glbh), 16));//管理 / 销售编号
            list_sale.Add(zf);//正负
            list_sale.Add("D");//DRBR区分
            list_sale.Add(bstzd2);//原价区分
            list_sale.Add(sap_cd);//销售组织
            list_sale.Add(vtweg);//分销渠道
            list_sale.Add(z_khbm);//客户编码
            list_sale.Add(z_khmc);//客户名称
            list_sale.Add(city1);//客户城市
            list_sale.Add(kna1land1);//客户国家代码
            list_sale.Add(serialnum);//客户的采购号
            list_sale.Add(waerk);//凭证货币
            list_sale.Add(finishat);//定价日期
            list_sale.Add("");//机种
            list_sale.Add(z_glbh);//销售单号
            list_sale.Add(zzvkbur);//联络所
            list_sale.Add(zzprctr);//利润中心
            list_sale.Add(zzjgy);//事业别
            list_sale.Add(z_yydd);//营业担当
            list_sale.Add(tel_number);//客户电话号码 
            if (mailMobile)
            {
                list_sale.Add(mail);//邮箱
                list_sale.Add(mobile);//手机号 
            }
            #endregion
            saleData.AppendLine(list_sale.ToTSVString());
            list_sale.Clear();
            #region 第二条
            list_sale.Add("I");//标识符
            list_sale.Add(z_glbh_pk.Substring(z_glbh_pk.IndexOf(z_glbh), 16));//管理 / 销售编号
            list_sale.Add("10");//明细号
            list_sale.Add("ZDSC_SERV_1");//物料号
            list_sale.Add("修理费");//物料描述
            list_sale.Add(sap_cd);//工厂
            list_sale.Add("1");//数量
            list_sale.Add("PC");//数量单位
            list_sale.Add(s_kpje.ToString());//含税总价
            list_sale.Add(s_sl.ToString());//税率百分数
            list_sale.Add(zzprctr);//利润中心 
            #endregion
            saleData.AppendLine(list_sale.ToTSVString());
        }
        #endregion
        //protected override string GetSaleFileName(DateTime time)
        //{
        //    string saleFileName = string.Format("{0}_{1}_{2}{3}", SalePrefix, sap_cd, time.ToString("yyyyMMddHHmmss"), FileSuffix);
        //    return saleFileName;
        //}
        //protected override string GetPurchaseFileName(DateTime time)
        //{
        //    string purchaseFileName = string.Format("{0}_{1}_{2}{3}", PurchasePrefix, sap_cd, time.ToString("yyyyMMddHHmmss"), FileSuffix);
        //    return purchaseFileName;
        //}
    }
}
