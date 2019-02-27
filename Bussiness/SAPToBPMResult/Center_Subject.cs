using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult
{
    public delegate void LoadEventHander();
    public class Center_Subject
    {
        public event LoadEventHander LoadEvent;
        public virtual void Refresh()
        {
            LoadEvent?.Invoke();
        }
    }
}
