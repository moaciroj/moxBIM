
namespace MoxMain
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menu = new System.Windows.Forms.MenuStrip();
            this.arquivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MnuAddIFC = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuDelIFC = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MnuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.testeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel3 = new System.Windows.Forms.Panel();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mTv = new MoxTree();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.standardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.mProp = new MoxProperties();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.MoxTab = new System.Windows.Forms.TabControl();
            this.tabProp = new System.Windows.Forms.TabPage();
            this.tabQuant = new System.Windows.Forms.TabPage();
            this.tabObj = new System.Windows.Forms.TabPage();
            this.tabType = new System.Windows.Forms.TabPage();
            this.tabMat = new System.Windows.Forms.TabPage();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel4 = new System.Windows.Forms.Panel();
            this.PanelUnity = new System.Windows.Forms.Panel();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panel5 = new System.Windows.Forms.Panel();
            this.TextLog = new System.Windows.Forms.TextBox();
            this.MenuTxt = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ClearTxt = new System.Windows.Forms.ToolStripMenuItem();
            this.DlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.menu.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.MoxTab.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.MenuTxt.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arquivoToolStripMenuItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(800, 24);
            this.menu.TabIndex = 0;
            this.menu.Text = "menuStrip1";
            // 
            // arquivoToolStripMenuItem
            // 
            this.arquivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuNewProject,
            this.toolStripSeparator1,
            this.MnuAddIFC,
            this.MnuDelIFC,
            this.toolStripSeparator2,
            this.MnuClose,
            this.testeToolStripMenuItem});
            this.arquivoToolStripMenuItem.Name = "arquivoToolStripMenuItem";
            this.arquivoToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.arquivoToolStripMenuItem.Text = "File";
            // 
            // MnuNewProject
            // 
            this.MnuNewProject.Name = "MnuNewProject";
            this.MnuNewProject.Size = new System.Drawing.Size(138, 22);
            this.MnuNewProject.Text = "New Project";
            this.MnuNewProject.Click += new System.EventHandler(this.MnuNewProject_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(135, 6);
            // 
            // MnuAddIFC
            // 
            this.MnuAddIFC.Name = "MnuAddIFC";
            this.MnuAddIFC.Size = new System.Drawing.Size(138, 22);
            this.MnuAddIFC.Text = "Append IFC";
            this.MnuAddIFC.Click += new System.EventHandler(this.MnuAddIFC_Click);
            // 
            // MnuDelIFC
            // 
            this.MnuDelIFC.Name = "MnuDelIFC";
            this.MnuDelIFC.Size = new System.Drawing.Size(138, 22);
            this.MnuDelIFC.Text = "Remove IFC";
            this.MnuDelIFC.Click += new System.EventHandler(this.MnuDelIFC_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(135, 6);
            // 
            // MnuClose
            // 
            this.MnuClose.Name = "MnuClose";
            this.MnuClose.Size = new System.Drawing.Size(138, 22);
            this.MnuClose.Text = "Close";
            this.MnuClose.Click += new System.EventHandler(this.MnuClose_Click);
            // 
            // testeToolStripMenuItem
            // 
            this.testeToolStripMenuItem.Name = "testeToolStripMenuItem";
            this.testeToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.statusStrip2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 428);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(800, 22);
            this.panel3.TabIndex = 4;
            // 
            // statusStrip2
            // 
            this.statusStrip2.Location = new System.Drawing.Point(0, 0);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(800, 22);
            this.statusStrip2.TabIndex = 0;
            this.statusStrip2.Text = "statusStrip2";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.mTv);
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 404);
            this.panel1.TabIndex = 5;
            // 
            // mTv
            // 
            this.mTv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTv.Location = new System.Drawing.Point(0, 0);
            this.mTv.Name = "mTv";
            this.mTv.Size = new System.Drawing.Size(200, 382);
            this.mTv.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 382);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(200, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.standardToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(67, 20);
            this.toolStripDropDownButton1.Text = "Standard";
            // 
            // standardToolStripMenuItem
            // 
            this.standardToolStripMenuItem.Name = "standardToolStripMenuItem";
            this.standardToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.standardToolStripMenuItem.Text = "Standard";
            // 
            // splitter1
            // 
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter1.Location = new System.Drawing.Point(200, 24);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 404);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.mProp);
            this.panel2.Controls.Add(this.splitter4);
            this.panel2.Controls.Add(this.MoxTab);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(619, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(181, 404);
            this.panel2.TabIndex = 7;
            // 
            // mProp
            // 
            this.mProp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mProp.Location = new System.Drawing.Point(0, 73);
            this.mProp.Name = "mProp";
            this.mProp.Size = new System.Drawing.Size(181, 331);
            this.mProp.TabIndex = 3;
            // 
            // splitter4
            // 
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter4.Location = new System.Drawing.Point(0, 70);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(181, 3);
            this.splitter4.TabIndex = 2;
            this.splitter4.TabStop = false;
            // 
            // MoxTab
            // 
            this.MoxTab.Controls.Add(this.tabProp);
            this.MoxTab.Controls.Add(this.tabQuant);
            this.MoxTab.Controls.Add(this.tabObj);
            this.MoxTab.Controls.Add(this.tabType);
            this.MoxTab.Controls.Add(this.tabMat);
            this.MoxTab.Dock = System.Windows.Forms.DockStyle.Top;
            this.MoxTab.Location = new System.Drawing.Point(0, 0);
            this.MoxTab.Multiline = true;
            this.MoxTab.Name = "MoxTab";
            this.MoxTab.SelectedIndex = 0;
            this.MoxTab.Size = new System.Drawing.Size(181, 70);
            this.MoxTab.TabIndex = 1;
            // 
            // tabProp
            // 
            this.tabProp.Location = new System.Drawing.Point(4, 44);
            this.tabProp.Name = "tabProp";
            this.tabProp.Padding = new System.Windows.Forms.Padding(3);
            this.tabProp.Size = new System.Drawing.Size(173, 22);
            this.tabProp.TabIndex = 0;
            this.tabProp.Text = "Properties";
            this.tabProp.UseVisualStyleBackColor = true;
            // 
            // tabQuant
            // 
            this.tabQuant.Location = new System.Drawing.Point(4, 44);
            this.tabQuant.Name = "tabQuant";
            this.tabQuant.Padding = new System.Windows.Forms.Padding(3);
            this.tabQuant.Size = new System.Drawing.Size(173, 22);
            this.tabQuant.TabIndex = 1;
            this.tabQuant.Text = "Quantities";
            this.tabQuant.UseVisualStyleBackColor = true;
            // 
            // tabObj
            // 
            this.tabObj.Location = new System.Drawing.Point(4, 44);
            this.tabObj.Name = "tabObj";
            this.tabObj.Padding = new System.Windows.Forms.Padding(3);
            this.tabObj.Size = new System.Drawing.Size(173, 22);
            this.tabObj.TabIndex = 2;
            this.tabObj.Text = "Object";
            this.tabObj.UseVisualStyleBackColor = true;
            // 
            // tabType
            // 
            this.tabType.Location = new System.Drawing.Point(4, 44);
            this.tabType.Name = "tabType";
            this.tabType.Size = new System.Drawing.Size(173, 22);
            this.tabType.TabIndex = 3;
            this.tabType.Text = "Type";
            this.tabType.UseVisualStyleBackColor = true;
            // 
            // tabMat
            // 
            this.tabMat.Location = new System.Drawing.Point(4, 44);
            this.tabMat.Name = "tabMat";
            this.tabMat.Size = new System.Drawing.Size(173, 22);
            this.tabMat.TabIndex = 4;
            this.tabMat.Text = "Materials";
            this.tabMat.UseVisualStyleBackColor = true;
            // 
            // splitter2
            // 
            this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(616, 24);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 404);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.PanelUnity);
            this.panel4.Controls.Add(this.splitter3);
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(203, 24);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(413, 404);
            this.panel4.TabIndex = 9;
            // 
            // PanelUnity
            // 
            this.PanelUnity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelUnity.Location = new System.Drawing.Point(0, 0);
            this.PanelUnity.Name = "PanelUnity";
            this.PanelUnity.Size = new System.Drawing.Size(413, 236);
            this.PanelUnity.TabIndex = 2;
            this.PanelUnity.Resize += new System.EventHandler(this.PanelUnity_Resize);
            // 
            // splitter3
            // 
            this.splitter3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter3.Location = new System.Drawing.Point(0, 236);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(413, 3);
            this.splitter3.TabIndex = 1;
            this.splitter3.TabStop = false;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.TextLog);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 239);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(413, 165);
            this.panel5.TabIndex = 0;
            // 
            // TextLog
            // 
            this.TextLog.ContextMenuStrip = this.MenuTxt;
            this.TextLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextLog.Location = new System.Drawing.Point(0, 0);
            this.TextLog.Multiline = true;
            this.TextLog.Name = "TextLog";
            this.TextLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextLog.Size = new System.Drawing.Size(413, 165);
            this.TextLog.TabIndex = 0;
            this.TextLog.WordWrap = false;
            // 
            // MenuTxt
            // 
            this.MenuTxt.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClearTxt});
            this.MenuTxt.Name = "contextMenuStrip1";
            this.MenuTxt.Size = new System.Drawing.Size(102, 26);
            // 
            // ClearTxt
            // 
            this.ClearTxt.Name = "ClearTxt";
            this.ClearTxt.Size = new System.Drawing.Size(101, 22);
            this.ClearTxt.Text = "Clear";
            this.ClearTxt.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // DlgOpen
            // 
            this.DlgOpen.FileName = "openFileDialog1";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.menu);
            this.MainMenuStrip = this.menu;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "moxBIMForm";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.Deactivate += new System.EventHandler(this.FormMain_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.MoxTab.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.MenuTxt.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem arquivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MnuNewProject;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MnuAddIFC;
        private System.Windows.Forms.ToolStripMenuItem MnuDelIFC;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem MnuClose;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem standardToolStripMenuItem;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox TextLog;
        private System.Windows.Forms.OpenFileDialog DlgOpen;
        private System.Windows.Forms.Panel PanelUnity;
        private System.Windows.Forms.ToolStripMenuItem testeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip MenuTxt;
        private System.Windows.Forms.ToolStripMenuItem ClearTxt;
        private System.Windows.Forms.TabControl MoxTab;
        private System.Windows.Forms.TabPage tabProp;
        private System.Windows.Forms.TabPage tabQuant;
        private System.Windows.Forms.TabPage tabObj;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.TabPage tabType;
        private System.Windows.Forms.TabPage tabMat;
        private MoxProperties mProp;
        private MoxTree mTv;
    }
}

