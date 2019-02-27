using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.KFSQ
{
    public class AccVouch
    {
        /************抬头****************/
        [ClassPropertySort(Index = 0, IsUsed = true)]
        public string I_KOKRS { get; set; }
        [ClassPropertySort(Index = 1, IsUsed = true)]
        public string I_AUART { get; set; }
        [ClassPropertySort(Index = 2, IsUsed = true)]
        public string I_AUFNR { get; set; }
        [ClassPropertySort(Index = 3, IsUsed = true)]
        public string I_KTEXT { get; set; }
        [ClassPropertySort(Index = 4, IsUsed = true)]
        public string I_BUKRS { get; set; }
        [ClassPropertySort(Index = 5, IsUsed = true)]
        public string I_SCOPE { get; set; }
        [ClassPropertySort(Index = 6, IsUsed = true)]
        public string I_USER0 { get; set; }
        [ClassPropertySort(Index = 7, IsUsed = true)]
        public string I_USER1 { get; set; }
        [ClassPropertySort(Index = 8, IsUsed = true)]
        public string I_USER2 { get; set; }
        [ClassPropertySort(Index = 9, IsUsed = true)]
        public string I_USER3 { get; set; }
        [ClassPropertySort(Index = 10, IsUsed = true)]
        public string I_USER4 { get; set; }
        [ClassPropertySort(Index = 11, IsUsed = true)]
        public string I_USER5 { get; set; }
        [ClassPropertySort(Index = 12, IsUsed = true)]
        public string I_USER6 { get; set; }
    }
}
