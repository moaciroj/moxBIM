using System;

namespace IFC
{
    public class IFCProject
    {
        public static bool IFCError { get; set; }

        public IFCProject()
        {
            IFCError = false;
        }


        public bool IFCProject_AddFile()
        {
            IFCError = true;
            return IFCError;
        }

    }
}
