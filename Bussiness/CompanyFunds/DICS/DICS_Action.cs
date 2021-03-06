﻿using SAPLinks.Bussiness.CompanyFunds.DICS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.CompanyFunds.DICS
{
    public class DICS_Action : CompanyObject, ICompanyAction
    {
        public DICS_Action()
        {
            company = "DICS";
            systype = 0;//0:公司经费 1:个人经费
            _filePath = "DICS_Path_C".ToAppSetting();
            _fileName = "DICS_Name_C".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DICS_Ext_C".ToAppSetting();
            BasicDataLoad();
        }
        public void Start()
        {
            DataConvert_New DICS = D_DICS();
            //文件拼接
            string fileData = DICS.file_sb.ToString();
            //脚本拼接
            string sql = DICS.upLinks_sql.ToString();
            if (string.IsNullOrEmpty(sql))
            {
                MainFile.WriteFile(filePath, fileName, fileData);
                return;
            }
            SqlCommand cmd = SQLHelper.GetTransactionSqlCommand(connStr);
            SQLHelper.ExecuteNonQuery(ref cmd, sql);
            if (MainFile.WriteFile(filePath, fileName, fileData))
                cmd.Transaction.Commit();
            else
                cmd.Transaction.Rollback();
            cmd.Connection.Close();
            //FKTZS_ZF fktzs_zf = D_FKTZS_ZF();
            //FKTZS_ZSK fktzs_zsk = D_FKTZS_ZSK();
            //ZSK zsk = D_ZSK();
            ////文件拼接
            //string fileData = fktzs_zf.file_sb.ToString() + fktzs_zsk.file_sb.ToString() + zsk.file_sb.ToString();
            ////脚本拼接
            //string sql = fktzs_zf.upLinks_sql.ToString() + fktzs_zsk.upLinks_sql.ToString() + zsk.upLinks_sql.ToString();
            //if (string.IsNullOrEmpty(sql))
            //{
            //    MainFile.WriteFile(filePath, fileName, fileData);
            //    return;
            //}
            //SqlCommand cmd = SQLHelper.GetTransactionSqlCommand(connStr);
            //SQLHelper.ExecuteNonQuery(ref cmd, sql);
            //if (MainFile.WriteFile(filePath, fileName, fileData))
            //    cmd.Transaction.Commit();
            //else
            //    cmd.Transaction.Rollback();
            //cmd.Connection.Close();
        }


        private DataConvert_New D_DICS()
        {
            LogInfo.Log.Info("《DICS公司经费》数据装载");
            DataConvert_New entity = new DICS(this);            
            entity.GetData();
            return entity;
        }

        /// <summary>
        /// 付款通知书-支付
        /// </summary>
        //private FKTZS_ZF D_FKTZS_ZF()
        //{
        //    LogInfo.Log.Info("《付款通知书（支付）》数据装载");
        //    FKTZS_ZF entity = new FKTZS_ZF();
        //    entity.GetData(this);
        //    return entity;
        //}
        ///// <summary>
        ///// 付款通知书-销账
        ///// </summary>
        //private FKTZS_ZSK D_FKTZS_ZSK()
        //{
        //    LogInfo.Log.Info("《付款通知书（暂收款）》数据装载");
        //    FKTZS_ZSK entity = new FKTZS_ZSK();
        //    entity.GetData(this);
        //    return entity;
        //}
        ///// <summary>
        ///// 暂收款
        ///// </summary>
        //private ZSK D_ZSK()
        //{
        //    LogInfo.Log.Info("《暂收款》数据装载");
        //    ZSK entity = new ZSK();
        //    entity.GetData(this);
        //    return entity;
        //}
    }
}
