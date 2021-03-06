﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness.WC
{
    /// <summary>
    /// 3：应付暂清帐
    /// 应付账款-暂
    /// </summary>
    public class AccountPayableAdvanceReceivedAC : AAccountPayableAdvanceReceivedAC
    {
        protected override List<AccVouch> DoLoad()
        {
            List<AccVouch> list = new List<AccVouch>();
            for (int i = 0; i < context.InvoiceEntitys.Count; i++)
            {
                InvoiceEntity invoiceEntity = context.InvoiceEntitys[i];
                AccVouch accVouch = new AccVouch();
                accVouch.XBLNR = context.ApplyNoEntity.ApplyNo;//参照号（XBLNR）
                accVouch.BLDAT = context.ApplyNoEntity.FinishAt;//凭证日期（BLDAT）
                accVouch.BUDAT = accVouch.BLDAT;//记账日期（BUDAT）
                string ywlx = string.Empty;//支付类型(取明细第一个)
                if (context.FKTZSZYDEntitys.Count != 0)
                    ywlx = context.FKTZSZYDEntitys[0].Z_YWLX;
                accVouch.BKTXT = string.Format("{0}-{1}-{2}/{3}付{4}", context.Fktzs_C_HEntitys.PayObjId.Substring(2, 1), context.Fktzs_C_HEntitys.ApplyDept, ywlx, context.Fktzs_C_HEntitys.ApplyDisplayName, context.Fktzs_C_HEntitys.PayObjName);//抬头摘要（BKTXT）
                accVouch.WAERS = "CNY";//币种（WAERS）
                accVouch.KURSF = "1";//汇率（KURSF）
                accVouch.NEWKO = "2171010101";//客户 / 供应商 / 会计科目代码（NEWKO）
                accVouch.NEWBS = "40";//记账码（NEWBS）
                accVouch.NEWUM = "";//特殊总账标识（NEWUM）
                accVouch.NEWBW = "";//資産取引タイプ（NEWBW）
                accVouch.KOSTL = "";//成本中心（KOSTL）
                accVouch.PRCTR = "";//利润中心（PRCTR）
                accVouch.PROJK = "";//WBS要素（PROJK）
                accVouch.AUFNR = "";//内部订单号（AUFNR）
                accVouch.WRBTR = invoiceEntity.Tax.ToString().Abs();//凭证货币金额（WRBTR）//专票对应税额
                accVouch.DMBTR = "";//本地货币金额（DMBTR）
                accVouch.MWSKZ = "";//税码（MWSKZ）
                accVouch.ZUONR = "";//分配（ZUONR）
                accVouch.SGTXT = invoiceEntity.Inv_No + accVouch.BKTXT;//明細テキスト（SGTXT）
                accVouch.XREF1 = "";//取引先参照キー１（XREF1）
                accVouch.XREF2 = "";//取引先参照キー２（XREF2）
                accVouch.XREF3 = "";//取引先参照キー３（XREF3）
                accVouch.ZFBDT = "";//支払基準日（ZFBDT）
                accVouch.ZTERM = "";//支払条件（ZTERM）
                accVouch.ZLSCH = "";//支払方法（ZLSCH）
                accVouch.ZLSPR = "";//支払保留（ZLSPR）
                accVouch.ZBD1T = "";//日数（ZBD1T）
                accVouch.HBKID = "";//取引銀行ID（HBKID）
                accVouch.BVTYP = "";//銀行タイプ（BVTYP）
                accVouch.VALUT = "";//起算日（VALUT）
                accVouch.WDATE = "";//手形振出日（WDATE）
                accVouch.WBANK = "";//銀行住所（WBANK）
                list.Add(accVouch);
            }
            return list;
        }
    }
}
