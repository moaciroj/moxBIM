using System.Windows.Forms;
using Xbim.Ifc.ViewModels;

namespace MoxMain
{
    public class MoxNode : TreeNode
    {
        public IXbimViewModel MoxVmodel { get; set; }

        public MoxNode(IXbimViewModel m, string n) : base (n)
        {
            MoxVmodel = m;
            if (m != null)
                Name = m.Name;
            else
                Name = n ?? "";
        }
    }
}
