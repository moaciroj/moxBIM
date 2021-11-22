using System.Collections.Generic;

namespace MoxGraphics.Geometry
{
    // Start is called before the first frame update
   
    public class MoxGeometries
    {
        public List<MoxGeometry> geometries { get; private set; }

        public MoxGeometries ()
        {
            geometries = new List<MoxGeometry>();
        }

        public void AddGeometry (MoxGeometry g)
        {
            geometries.Add(g);
        }

        public MoxGeometry FindGeometry (MoxGeometry g)
        {
            var resp = geometries.Find(x => x == g);
            return resp;
        }
    }

    public class MoxGeometry
    {
        public string FileName { get; set; }
        public float OneMetre { get; set; }

        public List<MoxEntity> Entities = new List<MoxEntity>();
        
        public MoxGeometry (string s)
        {
            FileName = s;
        }
        public MoxGeometry(string s, float m)
        {
            FileName = s;
            OneMetre = m;
        }

        public void AddEntity(MoxEntity e)
        {
            Entities.Add(e);
        }

        public void AddEntity(string f, int l, int pr, string ty, MoxMaterial mat, List<float[]> p, List<int> id)
        {
            var _ent = new MoxEntity(f, l, pr, ty, mat, p, id);
            AddEntity(_ent);
        }
    }
}