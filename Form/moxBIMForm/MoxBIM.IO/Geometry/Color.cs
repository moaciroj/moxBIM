using System.Runtime.InteropServices;

namespace MoxGraphics.Geometry
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MoxColor
    {
        public short R { get; set; }
        public short G { get; set; }
        public short B { get; set; }
        public float A { get; set; }

        public MoxColor()
        {
            R = -1; G = -1; B = -1; A = -1f;
        }
        public MoxColor (short r, short g, short b, float a)
        {
            R = r; G = g; B = b; A = a;
        }

        public MoxColor(short r, short g, short b)
        {
            R = r; G = g; B = b; A = -1f;
        }
    }
}