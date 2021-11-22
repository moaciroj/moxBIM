using System.Collections.Generic;

namespace MoxGraphics.Geometry
{
    public class MoxMaterial
    {
        private MoxColor color { get; set; }
        public string name { get; private set; }

        public MoxMaterial(MoxColor c, string s )
        {
            color = c;
            name = s ?? "Standard";
        }

        public MoxMaterial(MoxColor c)
        {
            color = c;
            name = "Standard";
        }

        public MoxMaterial(byte  r, byte g, byte b, float a)
        {
            color = new MoxColor(r,g,b,a);
            name = "Standard";
        }

        public MoxMaterial(byte r, byte g, byte b)
        {
            color = new MoxColor(r, g, b);
            name = "Standard";
        }

        public MoxMaterial()
        {
            color = new MoxColor();
        }

        public void SetColor (MoxColor c) { color = c; }

        public MoxColor GetColor()
        {
            return color;
        }

    }
}