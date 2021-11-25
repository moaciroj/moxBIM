using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MoxGraphics.Geometry;

namespace MoxBIM.IO
{
    public partial class MoxIOFile
    {
        private static string ChangeExtension(string @f)
        {
            if (f == null) return "";
            if (f.IndexOf('.') > 0)
            {
                char[] arr = f.ToCharArray();
                Array.Reverse(arr);
                f = new string(arr);
                f = f.Substring(f.IndexOf('.')+1);
                arr = f.ToCharArray();
                Array.Reverse(arr);
                f = new string(arr);
            }
            return f + ".mox";
        }

        private static float[] GetLinePoints(string l)
        {
            int pos = 0;
            float[] px = new float[6] { 0f, 0f, 0f, 0f, 0f, 0f };
            while (l.Length > 0 && pos < 6)
            {
                float r = 0f;
                if (l.IndexOf(',') >= 0)
                {
                    if (float.TryParse(l.Substring(0, l.IndexOf(',')).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float _r))
                        r = _r;
                    l = l.Substring(l.IndexOf(',') + 1).Trim(' ', ';', ':');
                }
                else
                {
                    if (float.TryParse(l.Substring(0).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float _r))
                        r = _r;
                    l = "";
                }
                px[pos] = r;
                pos++;
            }
            return px;
        }


        private static void GetIndex(string l, ref List<int> index)
        {
            l = l.Substring(7);
            while (l != null && l.Length > 0)
            {
                int i = -1;
                if (l.IndexOf(',') >= 0)
                {
                    if (int.TryParse(l.Substring(0, l.IndexOf(',')).Trim(), out int _i))
                        i = _i;
                    l = l.Substring(l.IndexOf(',') + 1).Trim(' ', ';', ':');
                }
                else
                {
                    if (int.TryParse(l.Substring(0).Trim(), out int _i))
                        i = _i;
                    l = "";
                }
                if (i >= 0) index.Add(i);
            }
        }

        private static MoxMaterial GetMaterial(string m)
        {
            short R = -1; short G = -1; short B = -1; float A = -1f; string name = "Standard";
            if (m.IndexOf('R') >= 0 && m.IndexOf(',') >= 0)
            {
                int posi = m.IndexOf('R') + 1;
                if (m.IndexOf(',') >= 0)
                {
                    int len = m.IndexOf(',') - 1;
                    string str = m.Substring(posi, len).Trim(' ', ';', ':', ',');
                    if (short.TryParse(str, out short _r))
                        R = _r;
                    m = m.Substring(m.IndexOf(',') + 1).Trim();
                }
                else
                {
                    if (short.TryParse(m.Substring(posi), out short _r))
                        R = _r;
                }
            }
            if (m.IndexOf('G') >= 0)
            {
                int posi = m.IndexOf('G') + 1;
                if (m.IndexOf(',') >= 0)
                {
                    int len = m.IndexOf(',') - 1;
                    string str = m.Substring(posi, len).Trim(' ', ';', ':', ',');
                    if (short.TryParse(str, out short _g))
                        G = _g;
                    m = m.Substring(m.IndexOf(',') + 1).Trim();
                }
                else
                {
                    if (short.TryParse(m.Substring(posi), out short _g))
                        G = _g;
                }
            }
            if (m.IndexOf('B') >= 0)
            {
                int posi = m.IndexOf('B') + 1;
                if (m.IndexOf(',') >= 0)
                {
                    int len = m.IndexOf(',') - 1;
                    string str = m.Substring(posi, len).Trim(' ', ';', ':', ',');
                    if (short.TryParse(str, out short _b))
                        B = _b;
                    m = m.Substring(m.IndexOf(',') + 1).Trim();
                }
                else
                {
                    if (short.TryParse(m.Substring(posi), out short _b))
                        B = _b;
                }
            }
            if (m.IndexOf('A') >= 0)
            {
                int posi = m.IndexOf('A') + 1;
                if (m.IndexOf(',') >= 0)
                {
                    int len = m.IndexOf(',') - 1;
                    string str = m.Substring(posi, len).Trim(' ', ';', ':', ',');
                    if (float.TryParse(str, out float _a))
                        A = _a;
                    m = m.Substring(m.IndexOf(',') + 1).Trim();
                }
                else
                {
                    if (float.TryParse(m.Substring(posi), out float _a))
                        A = _a;
                }
            }

            if (m.IndexOf("NAME: ") >= 0 && m.Substring(m.IndexOf("NAME: ")).Length > 7)
            {
                m = m.Substring(m.IndexOf("NAME: ") + 6).Trim(' ', ';', ':', ',');
                if (m.IndexOf(',') >= 0 && m.Substring(0, m.IndexOf(',')).Length > 2)
                    name = m.Substring(1, m.IndexOf(',') - 3);
                else
                    name = m.Substring(1, m.Length - 2);
            }

            return new MoxMaterial(new MoxColor(R, G, B, A), name);
        }

    }
}

