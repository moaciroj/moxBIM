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

        /*
        public void AddEntity (MoxGeometry g, MoxEntity e)
        {
            var resp = geometrylist.FindGeometry(g);
            if (resp == null)
            {
                resp = new MoxGeometry(e.File);
                geometrylist.AddGeometry(resp);
            }
            resp.AddEntity(e);
        }
        */

        public MoxGeometry FindGeometry (string s)
        {
            return geometrylist.geometries.Find(x => x.FileName == s);
        }
    }
}

