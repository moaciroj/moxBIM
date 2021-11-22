using System;
using System.Runtime.InteropServices;

namespace MoxBIM.IO
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;    // Any value the sender chooses.  Perhaps its main window handle?
        [MarshalAs(UnmanagedType.I4)]
        public int cbData;       // The count of bytes in the message.
        public IntPtr lpData;    // The address of the message.
    }
}