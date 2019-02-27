using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness
{
    interface IBaseAction
    {
        Boolean PostEvent(string methodName);
        void MessageQueue(string Title, string Message);
        string ErrMsg();
    }
}
