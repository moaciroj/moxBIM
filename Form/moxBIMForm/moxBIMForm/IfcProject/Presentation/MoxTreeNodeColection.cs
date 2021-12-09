using System.Windows.Forms;

namespace MoxMain
{
    public partial class MoxTreeView : TreeView
    {
        public new TreeNodeCollection Nodes
        {
            get
            {
                return base.Nodes;
            }

            set
            {

            }
        }
    }
}