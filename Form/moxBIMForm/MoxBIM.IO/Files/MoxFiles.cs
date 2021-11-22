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

        public MoxGeometry ReadFileGeometry(string fullpath)
        {
            string MxFile = ChangeExtension(fullpath);

            var f = new FileInfo(MxFile);
            if (f == null || !f.Exists)
                throw new IOException("Arquivo [" + f.Name + "] não existe");

            MoxGeometry moxg = new MoxGeometry(f.Name);

            Encoding encoding = Encoding.ASCII;
            FileStream fs = new FileStream(f.FullName, FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(fs, encoding);
            try
            {
                if (rd != null)
                {
                    string l;
                    l = WhileNotEqual(ref rd, "GEOMETRY");
                    if (l.Length > 0)
                    {
                        WhileGeometry(ref rd, ref l, ref moxg);
                        if (l != "ENDSEC_GEOMETRY")
                            throw new IOException("[GEOMETRY] Inconsistência no arquivo de dados .mox");
                    }
                    else
                        throw new IOException("[GEOMETRY] Inconsistência no arquivo de dados .mox");
                }
            }
            catch (IOException ioe)
            {
                moxg = null;
                throw new IOException(ioe.Message);
            }
            
            return moxg;
        }

        private void WhileGeometry(ref StreamReader rd, ref string l, ref MoxGeometry moxg)
        {
            l = WhileNotEqual(ref rd, "FILE_NAME");
            if (l.Length > 12)
            {
                l = l.Substring(10).Trim(' ', ';', ':');
                moxg.FileName = l.Substring(1,l.Length-2);
            }
            l = WhileNotEqual(ref rd, "ONEMETRE");
            if (l.Length > 0)
            {
                l = l.Substring(10).Trim(' ', ';', ':');
                if (float.TryParse(l.Substring(0).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float _r))
                    moxg.OneMetre = _r;
                else
                    moxg.OneMetre = 1f;
            }
            l = WhileNotEqual(ref rd, "");
            
            WhileEntity(ref rd, ref l, ref moxg);
        }

        private void WhileEntity (ref StreamReader rd, ref string l, ref MoxGeometry moxg)
        {
            while (l.Length > 5 && l.Substring(0, 6) == "ENTITY")
            {
                if (rd.EndOfStream) break;
                int label = 0;
                int parent = 0;
                string type = "Standard";
                MoxMaterial material = new MoxMaterial();
                List<float[]> points = new List<float[]>();
                List<int> index = new List<int>();

                l = WhileNotEqual(ref rd, "LABEL");
                if (l.Length > 0)
                {
                    l = l.Substring(6).Trim(' ', ';', ':');
                    if (!int.TryParse(l, out label))
                        throw new IOException("[LABEL] Inconsistência no arquivo de dados .mox.");

                }
                else
                    throw new IOException("[LABEL] Inconsistência no arquivo de dados .mox.");
                l = WhileNotEqual(ref rd, "LABEL_PARENT");
                if (l.Length > 0)
                {
                    l = l.Substring(13).Trim(' ', ';', ':');
                    if (!int.TryParse(l, out parent))
                        throw new IOException("[PARENT] Inconsistência no arquivo de dados .mox.");
                }
                else
                    throw new IOException("[PARENT] Inconsistência no arquivo de dados .mox.");
                l = WhileNotEqual(ref rd, "TYPE");
                if (l.Length > 7)
                {
                    l = l.Substring(5).Trim(' ', ';', ':');
                    type = l.Substring(1, l.Length - 2);
                }
                else
                    throw new IOException("[TYPE] Inconsistência no arquivo de dados .mox.");

                l = WhileNotEqual(ref rd, "COLOR");
                if (l.Length > 0)
                {
                    l = l.Substring(6).Trim(' ', ';', ':');
                    material = GetMaterial(l);
                }
                else
                    throw new IOException("[COLOR] Inconsistência no arquivo de dados .mox.");

                l = WhileNotEqual(ref rd, "");
                if (l.Length > 0)
                {
                    if (l == "POINTS")
                    {
                        GetPoints(ref rd, ref l, ref points);
                        if (l.Substring(0, 5) == "INDEX")
                        {
                            GetIndex(l, ref index);
                            l = WhileNotEqual(ref rd, "ENDSEC_POINTS");
                        }
                        else
                            throw new IOException("[INDEX] Inconsistência no arquivo de dados .mox.");
                        if (l != "ENDSEC_POINTS")
                            throw new IOException("[ENDSEC_POINTS] Inconsistência no arquivo de dados .mox.");

                    }
                    moxg.AddEntity(new MoxEntity(moxg.FileName, label, parent, type, material, points, index));
                    
                    l = WhileNotEqual(ref rd, "");
                    if (l != "ENDSEC_ENTITY")
                        throw new IOException("[ENDSEC_ENTITY] Inconsistência no arquivo de dados .mox.");
                    l = WhileNotEqual(ref rd, "");
                }
                else
                    throw new IOException("[ENTITY] Inconsistência no arquivo de dados .mox.");
            }
        }

        private static void GetIndex (string l, ref List<int> index)
        {
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
                if (i > 0) index.Add(i);
            }
        }

        private static void GetPoints (ref StreamReader rd, ref string l, ref List<float[]> points)
        {
            l = WhileNotEqual(ref rd, "");
            while (l.Length > 1 && l.Substring(0,2) == "p[" && l.IndexOf(':') > 0 )
            {
                if (rd.EndOfStream) break;
                float[] p = GetLinePoints(l.Substring(l.IndexOf(':')+2));
                points.Add(p);
                l = WhileNotEqual(ref rd, "");
            }
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

        private static MoxMaterial GetMaterial (string m)
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
                m = m.Substring( m.IndexOf("NAME: ") + 6).Trim(' ', ';', ':', ',');
                if (m.IndexOf(',') >= 0 && m.Substring(0,m.IndexOf(',')).Length > 2)
                    name = m.Substring(1, m.IndexOf(',') - 3);
                else
                    name = m.Substring(1,m.Length - 2);
            }

            return new MoxMaterial(new MoxColor(R, G, B, A), name);
        }

        private static string WhileNotEqual(ref StreamReader rd, string val)
        {
            string l = "";
            val = val.Trim(' ', ';', ':');
            do
            {
                if (rd.EndOfStream) break;
                l = rd.ReadLine().Trim(' ', ';', ':');
                if (val.Length > 0)
                {
                    if (l.Length > val.Length)
                    {
                        string lx = l.Substring(0, val.Length);
                        if (lx == val)
                            break;
                    }
                    else
                    if (l == val)
                        break;
                    if (rd.EndOfStream && l != val) return "";
                }
            } while (l == "" && !rd.EndOfStream);
            return l;
        }

        public bool WriteFileGeometry(string fullpath, string otherpath)
        {
            var f = new FileInfo(@fullpath);
            
            var geo = FindGeometry(f.Name);
            if (geo == null)
                throw new IOException("Geometria [" + f.Name + "] não encontrada");

            string MxFile;
            if (otherpath == null)
                MxFile = ChangeExtension(f.FullName);
            else
                MxFile = ChangeExtension(otherpath);

            Encoding encoding = Encoding.ASCII;
            FileStream fs = new FileStream(MxFile, FileMode.Create, FileAccess.Write);
            using (StreamWriter wt = new StreamWriter(fs, encoding))
            {
                wt.WriteLine("GEOMETRY;");
                wt.WriteLine();
                wt.WriteLine("FILE_NAME: \'" + @f.Name + "\';");
                wt.WriteLine("ONEMETRE: " + geo.OneMetre.ToString("R") + ";");

                foreach (var item in geo.Entities)
                {
                    wt.WriteLine();
                    wt.WriteLine("ENTITY;");
                    wt.WriteLine("LABEL: " + item.Label + ";");
                    wt.WriteLine("LABEL_PARENT: " + item.Parent + ";");
                    wt.WriteLine("TYPE: '" + item.Type + "';");
                    wt.WriteLine("COLOR: R" + item.Material.GetColor().R + ", G" + item.Material.GetColor().G + ", B" + item.Material.GetColor().B + ", A" + item.Material.GetColor().A + ", NAME: '" + item.Material.name + "';");
                    if (item.Points.Count > 0 && item.Index.Count > 0)
                    {
                        wt.WriteLine("POINTS;");
                        string sx = "";
                        string vir;
                        int ct = 1;
                        foreach (var itemp in item.Points)
                        {
                            sx = "p[" + ct.ToString() + "]: ";
                            int ctp = 0;
                            foreach (var itemf in itemp)
                            {
                                if (ctp > 0)
                                    vir = ",";
                                else
                                    vir = "";
                                sx = sx + vir + itemf.ToString("R");
                                ctp++;
                            }
                            ct++;
                            wt.WriteLine(sx + ";");
                        }

                        ct = 1;
                        sx = "";
                        foreach (var itemi in item.Index)
                        {
                            if (sx.Length > 0)
                                vir = ",";
                            else
                                vir = "";
                            sx = sx + vir + itemi.ToString();
                        }
                        wt.WriteLine("INDEX: " + sx + ";");
                        wt.WriteLine("ENDSEC_POINTS;");
                    }
                    wt.WriteLine("ENDSEC_ENTITY;");
                }
                wt.WriteLine();
                wt.WriteLine("ENDSEC_GEOMETRY;");
                return true;
            }
        }
    }
}

