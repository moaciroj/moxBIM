using System;



namespace IFC
{
    public class IFCClass
    {
        public static bool IFCError { get; set; }

        public IFCClass()
        {
            IFCError = false;
        }


        public bool IFCClass_AddFile()
        {
            IFCError = true;
            return IFCError;
        }

    }
}
