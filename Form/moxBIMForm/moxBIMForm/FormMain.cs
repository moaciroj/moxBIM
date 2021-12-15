using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using MoxProject;
using MoxGraphics;
using System.Threading;
using MoxBIM.IO;

namespace MoxMain
{
    public partial class FormMain : Form
    {
        #region Embebed Window
        private static IntPtr unityHWND = IntPtr.Zero;
        private Process process { get; set; }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        #endregion

        private IntPtr MOX_ADDENTITY = new IntPtr(1);
        private IntPtr MOX_DELENTITY = new IntPtr(2);
        private const uint WM_COPYDATA = 0x004A;
        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        public static List<MoxProjectClass> ProjectList = new List<MoxProjectClass>();

        public static MoxProjectClass CurrentProject { get; private set; }

        public static bool ChkVerboseIsChecked { get; set; }

        public static selectedTab SelectedTab { get; set; }

        int idOwner = 0;
        string PrjName = "Novo projeto";

        MoxProperties mProp = new MoxProperties();

        public FormMain(ILogger logger = null)
        {
            InitializeComponent();
            // 
            // mTv
            // 
            this.mTv = new MoxTree();
            panelTree.Controls.Add(mTv);
            mTv.Dock = System.Windows.Forms.DockStyle.Fill;
            mTv.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.MoxTreeView_AfterSelect);
            mTv.Name = "mTv";

            ChkVerboseIsChecked = false;
            ChkVerbose.Checked = false;
            SelectedTab = selectedTab.tabProp;

            if (CurrentProject == null)
            {
                MnuAddIFC.Enabled = false;
                MnuDelIFC.Enabled = false;
            }
            #region Embebed Window
            /*
            try
            {
                
                process = new Process();
                process.StartInfo.FileName = "moxBIM-Panel.exe";
                //process.StartInfo.Arguments = "-parentHWND " + PanelUnity.Handle.ToInt32() + " " + Environment.CommandLine;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();
                process.WaitForInputIdle();
                // Doesn't work for some reason ?!
                unityHWND = process.Handle;


                //EnumChildWindows(PanelUnity.Handle, WindowEnum, IntPtr.Zero);
                
                AddTextLine("Unity HWND1: 0x" + unityHWND1.ToString("X8"));
                AddTextLine("Unity HWND2: 0x" + unityHWND2.ToString("X8"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to Child.exe.");
            }
            */
            #endregion
        }

        private void MnuAddIFC_Click(object sender, EventArgs e)
        {

            DlgOpen.Multiselect = false;
            DlgOpen.Title = "Selecionar arquivos *.ifc";
            DlgOpen.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //filtra para exibir somente arquivos de imagens
            DlgOpen.Filter = "IFC Files (*.ifc)|*.ifc";
            DlgOpen.CheckFileExists = true;
            DlgOpen.CheckPathExists = true;
            DlgOpen.FilterIndex = 1;
            DlgOpen.RestoreDirectory = true;
            DlgOpen.ReadOnlyChecked = false;
            DlgOpen.ShowReadOnly = true;

            if (DlgOpen.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (CurrentProject.AddFile(DlgOpen.FileName))
                    {
                        AddTextLine("Abriu o projeto: " + CurrentProject.LastIfcFile.FileFullName);
                        mTv.AddTreeViewModel(CurrentProject);
                    }
                    else
                    {
                        AddTextLine("Não foi possível abrir o arquivo: " + DlgOpen.FileName);
                    }
                }
                catch (Exception ex)
                {
                    AddTextLine(ex.Message);
                }

            }
        }
        
        private void AddText(string s)
        {
            TextLog.Text += s;
        }
        public void AddTextLine(string s)
        {
            AddText("\r\n" + s);
        }

        public void NewLine()
        {
            AddText("\r\n");
        }

        public void NewLine(int rep)
        {
            for (int i = 0; i < rep; i++)
               AddText("\r\n");
        }

        public void ClearText()
        {
            TextLog.Text = "";
        }

        private void MnuNewProject_Click(object sender, EventArgs e)
        {
            ClearText();
            CurrentProject = new MoxProjectClass(PrjName, idOwner);
            AddTextLine("Projeto aberto: " + CurrentProject.ProjectName);

            ProjectList.Add(CurrentProject);
            MnuAddIFC.Enabled = true;

            if (CurrentProject.LastIfcFile == null)
                MnuDelIFC.Enabled = false;
        }

        private void MnuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //process.Kill();
        }

        private void MnuDelIFC_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not done!");
        }

        #region Embebed Unity Window
        private void ActivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            unityHWND = hwnd;
            ActivateUnityWindow();
            return 0;
        }

        private void PanelUnity_Resize(object sender, EventArgs e)
        {
            MoveWindow(unityHWND, 0, 0, panelUnity.Width, panelUnity.Height, true);
            ActivateUnityWindow();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            #region Embebed Window
            /*
            try
            {
                
                
                process.CloseMainWindow();

                Thread.Sleep(1000);
                while (!process.HasExited)
                    process.Kill();
            }
            catch (Exception)
            {

            }
            */
            #endregion
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            ActivateUnityWindow();
        }

        private void FormMain_Deactivate(object sender, EventArgs e)
        {
            DeactivateUnityWindow();
        }
        #endregion

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearText();
        }

        private void MoxTab_Enter(object sender, EventArgs e)
        {
            switch (SelectedTab)
            {
                case selectedTab.tabObj:
                    tabObj.Select();
                    break;
                case selectedTab.tabType:
                    tabType.Select();
                    break;
                case selectedTab.tabProp:
                    tabProp.Select();
                    break;
                case selectedTab.tabQuant:
                    tabQuant.Select();
                    break;
                case selectedTab.tabMat:
                    tabMat.Select();
                    break;
            }
        }

        private void MoxTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (MoxTab.SelectedIndex)
            {
                case 0:
                    SelectedTab = selectedTab.tabProp;
                    break;
                case 1:
                    SelectedTab = selectedTab.tabQuant;
                    break;
                case 2:
                    SelectedTab = selectedTab.tabObj;
                    break;
                case 3:
                    SelectedTab = selectedTab.tabType;
                    break;
                case 4:
                    SelectedTab = selectedTab.tabMat;
                    break;
            }
            mProp.SetTabValues(SelectedTab);
            PropertyGrid.Refresh();
        }

        public void SetEntityProperty(MoxNode m)
        {
            if (m.MoxVmodel != null && m.MoxVmodel.Entity != null)
                this.mProp.Entity = m.MoxVmodel.Entity;
            else
                this.mProp.Clear();
            PropertyGrid.Refresh();
        }

        private void MoxTreeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            MoxNode myNode = (MoxNode)e.Node as MoxNode;
            mTv.CurrentNode = myNode;
            SetEntityProperty(myNode);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            PropertyGrid.SelectedObject = mProp;
        }
    }
}


