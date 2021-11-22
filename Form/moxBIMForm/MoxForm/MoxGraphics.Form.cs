using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MoxBIM.IO;
//using MoxMain;

namespace MoxForm
{
    public partial class MoxGraphicsForm : Form
    {

        private const Int32 MOX_OPENFILE = 0x003A;
        private const int   WM_COPYDATA  = 0x004A;
        private IntPtr MOX_ADDENTITY = new IntPtr(1);

        public MoxGraphicsForm()
        {
            
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            Boolean handled = false; m.Result = IntPtr.Zero; IntPtr type = m.WParam;
            if (m.Msg == WM_COPYDATA)
            {
                handled = true;
                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                Type mytype = cds.GetType();
                cds = (COPYDATASTRUCT)m.GetLParam(mytype);
                int datatyp = (int)cds.dwData;
                if (datatyp == (int)MoxEnums.MOX_SENDGEOMETRY)
                {
                    string fileName = Marshal.PtrToStringAnsi(cds.lpData);
                    AddText("Information received [" + fileName + "]\r\n");
                    SendEvent(type.ToInt32(), fileName);
                }
            }
            if (handled) DefWndProc(ref m); else base.WndProc(ref m);
        }

        private void btn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public void AddText (string s)
        {
            this.txt.Text += s;
        }

        public void ClearText()
        {
            this.txt.Text = "";
        }

        private void SendEvent(int wParam, string lParam)
        {
            //UnityMain.Instance_Main.OnMyEvent(wParam, lParam);
        }

        private void MoxGraphicsForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F4:
                    if (this.WindowState != FormWindowState.Normal)
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.BringToFront();
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Minimized;
                    }
                    break;
            }
        }
    }
}

