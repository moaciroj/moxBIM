using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        private static bool GetIndex(string csv, ref List<int> index)
        {
            int val;
            var numberStyle = NumberStyles.AllowThousands |
                              NumberStyles.AllowExponent;
            index = csv.Split(',')
                       .Select(m => { int.TryParse(m, numberStyle, CultureInfo.InvariantCulture, out val); return val; })
                       .ToList();
            if (index.Count % 3 != 0) return false;
            return true;
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

