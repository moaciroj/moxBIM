using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Xbim.Ifc.ViewModels;
using Xbim.Ifc4.Interfaces;
using MoxProject;


namespace MoxMain
{
    public partial class MoxTreeView : TreeView
    {

        public MoxProjectClass TvP{ get; set; }

        public MoxTreeView()
        {
            InitializeComponent();
        }

        public new TreeNodeCollection Nodes
        {
            get
            {
                return base.Nodes;
            }
        }


        public void SetTreeViewModel (MoxProjectClass p)
        {
            TvP = p;
        }


        public void ShowAllData() 
        {
            this.Nodes.Clear();

            if (TvP != null && TvP.IFCFileList.Count > 0)
            {
                foreach (var f in TvP.IFCFileList)
                {
                    this.Nodes.Add(f.FileName);
                    var project = f.model.Instances.OfType<IIfcProject>().FirstOrDefault();
                    if (project != null)
                    {
                        List<SpatialViewModel> svList = new List<SpatialViewModel>();
                        foreach (var item in project.SpatialStructuralElements)
                        {
                            var sv = new SpatialViewModel(item, null);
                            svList.Add(sv);
                        }

                        //Não sei pra que que serve mas repeti ... 
                        //Deve ser para deixar a leitura mais lenta ...
                        foreach (var child in svList)
                        {
                            LazyLoadAll(child);
                        }
                    }
                }
            }
        }

        private void LazyLoadAll(IXbimViewModel parent)
        {
            foreach (var child in parent.Children)
            {
                LazyLoadAll(child);
            }
        }



        #region Preserve Restore
        private Dictionary<string, bool> treeState_dic = new Dictionary<string, bool>();

        public Dictionary<string, bool> TreeState_dic
        {
            get { return treeState_dic; }
            set { treeState_dic = value; }
        }

        private bool _preseveTreeState;

        public bool PreseveTreeState
        {
            get { return _preseveTreeState; }
            set { _preseveTreeState = value; }
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        public void RestoreTreeState()
        {
            if (this._preseveTreeState)
                restoreTreeState(this);
        }

        private void restoreTreeState(TreeView tree)
        {
            for (int i = 0; i < tree.Nodes.Count; i++)
            {
                if (tree.Nodes[i].Text != null)
                {
                    if (treeState_dic.ContainsKey(tree.Nodes[i].Text + "[" + i.ToString() + "]"))
                    {
                        if (treeState_dic[tree.Nodes[i].Text + "[" + i.ToString() + "]"])
                            tree.Nodes[i].Expand();
                        else
                            tree.Nodes[i].Collapse();
                    }
                    restoreTreeState_level2(tree.Nodes[i], tree.Nodes[i].Text + "[" + i.ToString() + "]");
                }
            }
        }

        private void restoreTreeState_level2(TreeNode tNode, string parentDicKey)
        {
            for (int i = 0; i < tNode.Nodes.Count; i++)
            {
                if (tNode.Nodes[i].Text != null)
                {
                    if (treeState_dic.ContainsKey(tNode.Nodes[i].Text + "[" + i.ToString() + "]" + (tNode.Nodes[i].Parent != null ? "_" + parentDicKey : "")))
                    {
                        if (treeState_dic[tNode.Nodes[i].Text + "[" + i.ToString() + "]" + (tNode.Nodes[i].Parent != null ? "_" + parentDicKey : "")])
                            tNode.Nodes[i].Expand();
                        else
                            tNode.Nodes[i].Collapse();
                    }
                    restoreTreeState_level2(tNode.Nodes[i], tNode.Nodes[i].Text + "[" + i.ToString() + "]" + (tNode.Nodes[i].Parent != null ? "_" + parentDicKey : ""));
                }
            }
        }

        public void SaveTreeState()
        {
            if (!this._preseveTreeState)
                treeState_dic = null;
            else
            {
                treeState_dic = new Dictionary<string, bool>();
                if (this.Nodes.Count != 0)
                    saveTreeState(this);
            }
        }

        private void saveTreeState(TreeView tree)
        {
            TreeNodeCollection nodesColl;
            nodesColl = tree.Nodes;
            for (int i = 0; i < nodesColl.Count; i++)
            {
                if (nodesColl[i].Text != null)
                {
                    treeState_dic[nodesColl[i].Text + "[" + i.ToString() + "]"] = nodesColl[i].IsExpanded;
                    saveTreeState_level2(nodesColl[i], nodesColl[i].Text + "[" + i.ToString() + "]");
                }
            }
        }

        private void saveTreeState_level2(TreeNode tNode, string parentDicKey)
        {
            TreeNodeCollection nodesColl;
            nodesColl = tNode.Nodes;
            for (int i = 0; i < nodesColl.Count; i++)
            {
                if (nodesColl[i].Text != null)
                {
                    treeState_dic[nodesColl[i].Text + "[" + i.ToString() + "]" + (nodesColl[i].Parent != null ? "_" + parentDicKey : "")] = nodesColl[i].IsExpanded;
                    saveTreeState_level2(nodesColl[i], nodesColl[i].Text + "[" + i.ToString() + "]" + (nodesColl[i].Parent != null ? "_" + parentDicKey : ""));
                }
            }
        }
        #endregion
    }
}
