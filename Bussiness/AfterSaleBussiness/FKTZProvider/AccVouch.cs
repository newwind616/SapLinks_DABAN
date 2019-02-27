using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 数据组装实体，最终输出的样式实体
    /// ClassPropertySort:Index:排序输出位置，IsUsed:是否需要输出
    /// </summary>
    public class AccVouch
    {
        /************抬头****************/
        [ClassPropertySort(Index = 0, IsUsed = true)]
        public string XBLNR { get; set; }
        [ClassPropertySort(Index = 1, IsUsed = true)]
        public string BLDAT { get; set; }
        [ClassPropertySort(Index = 2, IsUsed = true)]
        public string BUDAT { get; set; }
        [ClassPropertySort(Index = 3, IsUsed = true)]
        public string BKTXT { get; set; }
        [ClassPropertySort(Index = 4, IsUsed = true)]
        public string WAERS { get; set; }
        [ClassPropertySort(Index = 5, IsUsed = true)]
        public string KURSF { get; set; }
        /************明细****************/
        [ClassPropertySort(Index = 6, IsUsed = true)]
        public string NEWKO { get; set; }
        [ClassPropertySort(Index = 7, IsUsed = true)]
        public string NEWBS { get; set; }
        [ClassPropertySort(Index = 8, IsUsed = true)]
        public string NEWUM { get; set; }
        [ClassPropertySort(Index = 9, IsUsed = true)]
        public string NEWBW { get; set; }
        [ClassPropertySort(Index = 10, IsUsed = true)]
        public string KOSTL { get; set; }
        [ClassPropertySort(Index = 11, IsUsed = true)]
        public string PRCTR { get; set; }
        [ClassPropertySort(Index = 12, IsUsed = true)]
        public string PROJK { get; set; }
        [ClassPropertySort(Index = 13, IsUsed = true)]
        public string AUFNR { get; set; }
        [ClassPropertySort(Index = 14, IsUsed = true)]
        public string WRBTR { get; set; }
        [ClassPropertySort(Index = 15, IsUsed = true)]
        public string DMBTR { get; set; }
        [ClassPropertySort(Index = 16, IsUsed = true)]
        public string MWSKZ { get; set; }
        [ClassPropertySort(Index = 17, IsUsed = true)]
        public string ZUONR { get; set; }
        [ClassPropertySort(Index = 18, IsUsed = true)]
        public string SGTXT { get; set; }
        [ClassPropertySort(Index = 19, IsUsed = true)]
        public string XREF1 { get; set; }
        [ClassPropertySort(Index = 20, IsUsed = true)]
        public string XREF2 { get; set; }
        [ClassPropertySort(Index = 21, IsUsed = true)]
        public string XREF3 { get; set; }
        [ClassPropertySort(Index = 22, IsUsed = true)]
        public string ZFBDT { get; set; }
        [ClassPropertySort(Index = 23, IsUsed = true)]
        public string ZTERM { get; set; }
        [ClassPropertySort(Index = 24, IsUsed = true)]
        public string ZLSCH { get; set; }
        [ClassPropertySort(Index = 25, IsUsed = true)]
        public string ZLSPR { get; set; }
        [ClassPropertySort(Index = 26, IsUsed = true)]
        public string ZBD1T { get; set; }
        [ClassPropertySort(Index = 27, IsUsed = true)]
        public string HBKID { get; set; }
        [ClassPropertySort(Index = 28, IsUsed = true)]
        public string BVTYP { get; set; }
        [ClassPropertySort(Index = 29, IsUsed = true)]
        public string VALUT { get; set; }
        [ClassPropertySort(Index = 30, IsUsed = true)]
        public string WDATE { get; set; }
        [ClassPropertySort(Index = 31, IsUsed = true)]
        public string WBANK { get; set; }
    }
}
