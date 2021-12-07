using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc.ViewModels;
using Xbim.Ifc4.Interfaces;
using Xbim.Common;
using MoxProject;

namespace MoxMain
{
    public partial class MoxTreeView : TreeView
    {
        public MoxProjectClass TvProject { get; set; }

        public MoxNode CurrentNode { get; private set; }

        private Dictionary<string, IEnumerable<IXbimViewModel>> HierarchyView { get; set; }
        public Dictionary<IXbimViewModel, IEnumerable<MoxNode>> MoxVNodes { get; private set; }
        public Dictionary<int, IEnumerable<MoxNode>> MoxLabelNodes { get; private set; }

        public MoxTreeView()
        {
            InitializeComponent();
            New();
        }

        private void New()
        {
            saveTreeState(this);
            this.Nodes.Clear();
            HierarchyView = new Dictionary<string, IEnumerable<IXbimViewModel>>();
            MoxVNodes = new Dictionary<IXbimViewModel, IEnumerable<MoxNode>>();
            MoxLabelNodes = new Dictionary<int, IEnumerable<MoxNode>>();
        }

        public void AddTreeViewModel (MoxProjectClass p)
        {
            TvProject = p;
            var fl = p.LastIfcFile;
            if (fl != null)
            {
                if (!HierarchyView.ContainsKey(fl.FileName))
                {
                    ViewModel(fl.FileName, fl.model);
                    if (HierarchyView.ContainsKey(fl.FileName))
                    {
                        var file = HierarchyView[fl.FileName];
                        var node = new MoxNode(null, fl.FileName);
                        this.Nodes.Add(node);
                        foreach (var view in file)
                        {
                            var nnode = new MoxNode(view, view.Name);
                            node.Nodes.Add(nnode);
                            AddDict(view, nnode);
                            RecursivePopulate(view, nnode);
                        }
                    }
                }
            }
        }

        public void ShowAllData() 
        {
            New();
            if (TvProject != null && TvProject.IFCFileList.Count > 0)
            {
                foreach (var f in TvProject.IFCFileList)
                {
                    ViewModel(f.FileName, f.model);
                }

                PopulateAllMoxTreeview();
            }
            restoreTreeState(this);
        }


        private void PopulateAllMoxTreeview()
        {
            foreach (var file in HierarchyView)
            {
                var node = new MoxNode(null, file.Key);
                this.Nodes.Add(node);
                foreach (var view in file.Value)
                {
                    var nnode = new MoxNode(view, view.Name);
                    node.Nodes.Add(nnode);
                    AddDict(view, nnode);
                    RecursivePopulate(view, nnode);
                }
            }
        }

        private void RecursivePopulate(IXbimViewModel g, MoxNode n)
        {
            if (g != null && g.Children.Any())
            {
                foreach (var item in g.Children)
                {
                    var node = new MoxNode(item, item.Name);
                    n.Nodes.Add(node);
                    AddDict(item, node);
                    RecursivePopulate(item, node);
                }
            }
        }

        private void AddDict(IXbimViewModel v, MoxNode n)
        {
            if (MoxVNodes.ContainsKey(v))
            {
                MoxVNodes[v] = MoxVNodes[v].Append(n);
            }
            else
            {
                var l = new List<MoxNode>();
                l.Add(n);
                MoxVNodes.Add(v, l);
            }


            if (MoxLabelNodes.ContainsKey(v.EntityLabel))
            {
                MoxLabelNodes[v.EntityLabel] = MoxLabelNodes[v.EntityLabel].Append(n);
            }
            else
            {
                var l = new List<MoxNode>();
                l.Add(n);
                MoxLabelNodes.Add(v.EntityLabel, l);
            }
        }

        private void MoxTreeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            MoxNode myNode = (MoxNode)e.Node as MoxNode;
            CurrentNode = myNode;
        }

        private void ViewModel(string f, IModel Model)
        {
            //Models
            var project = Model.Instances.OfType<IIfcProject>().FirstOrDefault();
            if (project != null)
            {
                var svList = new List<MoxGroupsModel>();
                svList.Add(new MoxGroupsModel(project, XbimViewType.SpatialStructure));
                AddHierachy(f, svList);
            }

            //Groups
            IEnumerable list = Enumerable.Empty<MoxGroupsModel>();
            if (Model != null)
            {
                MoxGroupsModel v = new MoxGroupsModel(Model, XbimViewType.Groups);
                if (v.Children.Any())
                {
                    var glist = new List<MoxGroupsModel>();
                    glist.Add(v);
                    AddHierachy(f, glist);
                }
            }
        }

        private void AddHierachy (string f, IEnumerable<MoxGroupsModel> m)
        {
            if (HierarchyView.ContainsKey(f))
            {
                if (m != null)
                {
                    if (HierarchyView[f] == null)
                        HierarchyView[f] = m;
                    else
                        HierarchyView[f] = HierarchyView[f].Concat(m);
                }
            }
            else
                HierarchyView.Add(f, m);
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
