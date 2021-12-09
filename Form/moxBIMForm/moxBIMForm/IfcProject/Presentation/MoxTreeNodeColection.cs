using System.Windows.Forms;

namespace MoxMain
{
    public partial class MoxTree : TreeView
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