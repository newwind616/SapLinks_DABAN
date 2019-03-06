using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.DABANToEDI
{
    public class EDIObject : BaseAction
    {
        /// <summary>
        /// 会计期间
        /// </summary>
        //public static DataTable MAIN_ACCOUNTINGPERIOD;
        ///// <summary>
        ///// 科目转换
        ///// </summary>
        //public static DataTable SAP_CODECONVERT;
        public string company;
        public string _fileName;
        public string _filePath;
        public string _fileExt;
        public int systype;
        protected string filePath
        {
            get
            {
                if (_filePath.LastIndexOf('\\') == _filePath.Length - 1)
                    return _filePath;
                else
                    return _filePath + "\\";
            }
        }
        protected string fileName { get { return _fileName + _fileExt; } }
        /// <summary>
        /// 科目转换数据加载
        /// 会计期间数据装载
        /// 日期：2017-11-9 16:12:51
        /// 修改内容:修改会计期间时间取数逻辑，获取当前公司最大日期（防止用户维护多条会计期间后，取的是最小日期的会计期间）
        /// </summary>
        //protected void BasicDataLoad()
        //{
        //    //MAIN_ACCOUNTINGPERIOD = SQLHelper.ExecuteDataset(connStr, CommandType.Text, string.Format("SELECT * FROM BPMDB.DBO.MAIN_ACCOUNTINGPERIOD WHERE COMPANY='{0}' AND SYSTYPE='{1}'",company,systype)).Tables[0];
        //    MAIN_ACCOUNTINGPERIOD = SQLHelper.ExecuteDataset(connStr, CommandType.Text, string.Format("SELECT COMPANY,DATENAME(YEAR,MAX([DATE])) [YEAR],DATENAME(MONTH,MAX([DATE])) [MONTH],SYSTYPE, MAX([DATE]) [DATE] FROM BPMDB.DBO.MAIN_ACCOUNTINGPERIOD WHERE COMPANY='{0}' AND SYSTYPE='{1}' GROUP BY COMPANY,SYSTYPE", company, systype)).Tables[0];
        //    SAP_CODECONVERT = SQLHelper.ExecuteDataset(connStr, CommandType.Text, string.Format("SELECT * FROM BPMDB.DBO.SAP_CODECONVERT WHERE COMPANY='{0}' AND SYSTYPE='{1}'", company, systype)).Tables[0];
        //}
    }
}
