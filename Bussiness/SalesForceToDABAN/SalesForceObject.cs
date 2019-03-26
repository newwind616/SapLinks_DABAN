using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SalesForceToDABAN
{
    public class SalesForceObject : BaseAction
    {
        public string company;
        public string _fileName;
        public string _filePath;
        public string _fileExt;

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
    }
}
