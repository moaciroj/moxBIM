using System.Collections.Generic;

namespace MoxGraphics.Geometry
{
    public class MoxEntity
    {
        public string File { get; private set; }
        public string Type { get; private set; }
        public MoxMaterial Material { get; private set; }
        public int Label { get; private set; }
        public int Parent { get; private set; }
        public List<float[]> Points { get; private set; }
        public List<int> Index { get; private set; }

        public MoxEntity(string f, int l, int pr, string ty, MoxMaterial mat, List<float[]> p, List<int> id)
        {
            File = f;
            Label = l;
            Parent = pr;
            Type = ty;
            Material = mat;
            Points = p;
            Index = id;
        }
    }
}