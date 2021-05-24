using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC
{
    public class IFCFileClass
    {
        //Classes públicas
        //FileInfo
        public string IFCFileFullName { get; private set; }
        public string IFCFileName { get; private set; }
        public string IFCFilePath { get; private set; }
        public string IFCFileDir { get; private set; }
        public DateTime IFCFileDate { get; private set; }
        public double IFCFileSize { get; private set; }
        //Sucesso
        public bool Success { get; private set; }
        public IFCClass Ifcclass { get; private set; }

        //Classes privadas

        //Constructor
        public IFCFileClass (string s)
        {
            Success = false;
            var f = new FileInfo(s);
            if (f != null && f.Exists)
            {
                IFCFileFullName = f.FullName;
                IFCFileName =     f.Name;
                IFCFilePath =     f.Directory.FullName;
                IFCFileDir =      f.DirectoryName;
                IFCFileDate =     f.LastWriteTimeUtc;
                IFCFileSize =     f.Length;

                Ifcclass = new IFCClass();

                Success = true;
            }
            Success = true;
        }
    }
}
