using System;
using System.Runtime.InteropServices;
using System.Text;
using MoxBIM.IO;

namespace MoxGraphics
{
    public class MoxGraphicsClass
    {
        private IntPtr MOX_ADDENTITY = new IntPtr(1);
        private const uint WM_COPYDATA = 0x004A;

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
                
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        public void SendToUnity( [MarshalAs(UnmanagedType.LPStr)] string moxfile)
        {
            IntPtr resp = IntPtr.Zero; 
            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            try
            {
                var HWND = FindWindow(null, "MyForm-moxBIM");

                byte[] b = Encoding.Unicode.GetBytes(moxfile);
                IntPtr p = Marshal.AllocHGlobal(b.Length);
                Marshal.Copy(b, 0, p, b.Length);
                
                cds.dwData = (IntPtr)MoxEnums.MOX_SENDGEOMETRY;
                cds.lpData = p;
                cds.cbData = b.Length;

                if (HWND != IntPtr.Zero)  resp = SendMessage(HWND, WM_COPYDATA, MOX_ADDENTITY, ref cds);

            }
            catch
            {
                //Do here
            }
            finally
            {
                if (cds.lpData != IntPtr.Zero)
                   Marshal.FreeHGlobal(cds.lpData);
            }
        }
    }
}
