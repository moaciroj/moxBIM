using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MoxBIM.IO;
using MoxMain;
using System.Text;

namespace MoxForm
{
    public partial class MoxGraphicsForm : Form
    {

        private const int WM_COPYDATA = 0x004A;
        private IntPtr MOX_ADDENTITY = new IntPtr(1);

        public MoxGraphicsForm()
        {

            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            Boolean handled = false; m.Result = IntPtr.Zero; IntPtr wParam = m.WParam;
            if (m.Msg == WM_COPYDATA && (wParam == MOX_ADDENTITY))
            {
                try
                {
                    m.Result = new IntPtr(1);
                    handled = true;
                    COPYDATASTRUCT cds = new COPYDATASTRUCT();
                    Type mytype = cds.GetType();
                    cds = (COPYDATASTRUCT)m.GetLParam(mytype);
                    int datatyp = (int)cds.dwData;
                    if (datatyp > 0)
                    {
                        byte[] b = new byte[cds.cbData];
                        Marshal.Copy(cds.lpData, b, 0, cds.cbData);
                        string fileName = Encoding.Unicode.GetString(b);
                        AddText("Opem Geometry [" + fileName + "]\r\n");
                        SendEvent((int)wParam, datatyp, fileName);
                    }
                }
                catch(Exception e)
                {
                    AddText("Error: " + e.Message + "\r\n");
                    base.WndProc(ref m);
                }
            }
            if (handled) 
                DefWndProc(ref m); 
            else 
                base.WndProc(ref m);
        }

        private void btn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public void AddText(string s)
        {
            this.txt.Text += s;
        }

        public void ClearText()
        {
            this.txt.Text = "";
        }

        private void SendEvent(int wParam, int type, string lParam)
        {
            UnityMain.Instance_Main.OnMyEvent(wParam, type ,lParam);
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

