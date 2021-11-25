using System;
using MoxGraphics.Geometry;

namespace MoxBIM.IO
{
    public partial class MoxIOFile
    {
        public MoxGeometries geometrylist { get; private set; }

        public MoxIOFile()
        {
            geometrylist = new MoxGeometries();
        }

        public void AddGeometry (MoxGeometry g)
        {
            geometrylist.AddGeometry(g);
        }
        
        public MoxGeometry FindGeometry (string s)
        {
            return geometrylist.geometries.Find(x => x.FileName == s);
        }
    }
}

