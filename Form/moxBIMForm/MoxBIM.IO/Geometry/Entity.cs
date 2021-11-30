using System.Collections.Generic;

namespace MoxGraphics.Geometry
{
    public class MoxEntity
    {
        public string File { get; private set; }
        public int Label { get; private set; }
        public int Parent { get; private set; }
        public string Type { get; private set; }
        public MoxMaterial Material { get; private set; }
        public MoxMatrix3D? Transform { get; private set; }
        public List<float[]> Points { get; private set; }
        public List<int> Index { get; private set; }

        public MoxEntity(string f, int l, int pr, string ty, MoxMaterial mat, List<float[]> p, List<int> id, MoxMatrix3D? trans)
        {
            File = f;
            Label = l;
            Parent = pr;
            Type = ty;
            Material = mat;
            Transform = trans;
            Points = p;
            Index = id;
        }
    }
}