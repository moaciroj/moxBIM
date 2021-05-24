using System;
using System.Collections.Generic;

namespace IFC
{
    public class IFCProjectClass
    {
        public List<IFCFileClass> IFCProjectList = new List<IFCFileClass>();
        public IFCFileClass IFCProject_GetLastIFC { get; private set; }
        public bool IFCProjectError { get; private set; }
        public bool IFCLastError { get; private set; }
     
        public IFCProjectClass()
        {
            IFCProjectError = true;
            IFCLastError = true;
            IFCProject_GetLastIFC = null;
        }

        public bool IFCProject_AddFile(String fileifcname)
        {
            if (fileifcname == "")
            {
                IFCLastError = false;
                return false;
            }
            IFCFileClass ifcnewfile = new IFCFileClass(fileifcname);
            if (ifcnewfile.Success) 
            {
                IFCProjectList.Add(ifcnewfile);
                IFCProject_GetLastIFC = ifcnewfile;
                IFCLastError = false;
            }
            else
            {
                IFCLastError = true;
            }
            if (IFCProjectList.Count > 0) { IFCProjectError = true; } else { IFCProjectError = true; }
            return ifcnewfile.Success;
        }
    }
}
