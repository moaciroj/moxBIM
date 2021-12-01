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
        public MoxGeometry ReadFileGeometryMemory(string fullpath)
        {
            string MxFile = ChangeExtension(fullpath);

            var f = new FileInfo(MxFile);
            if (f == null || !f.Exists)
                throw new IOException("Arquivo [" + f.Name + "] não existe");

            MoxGeometry moxg = new MoxGeometry(f.Name);

            Encoding encoding = Encoding.ASCII;
            try
            {
                string[] lines = File.ReadAllLines(f.FullName, encoding);
                int posline = -1;

                if (lines.Length > 5)
                {
                    string l;
                    l = MemoryWhileNotEqual(ref lines, ref posline , "GEOMETRY");
                    if (l.Length > 0)
                    {
                        MemoryWhileGeometry(ref lines, ref posline, ref l, ref moxg);
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

        private void MemoryWhileGeometry(ref String[] lines, ref int posline, ref string l, ref MoxGeometry moxg)
        {
            l = MemoryWhileNotEqual(ref lines, ref posline, "FILE_NAME");
            if (l.Length > 12)
            {
                l = l.Substring(10).Trim(' ', ';', ':');
                moxg.FileName = l.Substring(1, l.Length - 2);
            }
            l = MemoryWhileNotEqual(ref lines, ref posline, "ONEMETRE");
            if (l.Length > 0)
            {
                l = l.Substring(10).Trim(' ', ';', ':');
                if (float.TryParse(l.Substring(0).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float _r))
                    moxg.OneMetre = _r;
                else
                    moxg.OneMetre = 1f;
            }
            l = MemoryWhileNotEqual(ref lines, ref posline, "");

            MemoryWhileEntity(ref lines, ref posline, ref l, ref moxg);
        }

        private void MemoryWhileEntity(ref string [] lines, ref int posline, ref string l, ref MoxGeometry moxg)
        {
            while (l.Length > 5 && l.Substring(0, 6) == "ENTITY")
            {
                if (posline >= lines.Length) break;
                int label = 0;
                int parent = 0;
                string type = "Standard";
                MoxMaterial material = new MoxMaterial();
                
                MoxMatrix3D transform = new MoxMatrix3D();
                transform.SetIdentify();
                
                List<MoxPoint3D> points = new List<MoxPoint3D>();
                List<int> index = new List<int>();

                l = MemoryWhileNotEqual(ref lines, ref posline, "LABEL");
                if (l.Length > 0)
                {
                    l = l.Substring(6).Trim(' ', ';', ':');
                    if (!int.TryParse(l, out label))
                        throw new IOException("[LABEL] Inconsistência no arquivo de dados .mox.");

                }
                else
                    throw new IOException("[LABEL] Inconsistência no arquivo de dados .mox.");
                l = MemoryWhileNotEqual(ref lines, ref posline, "LABEL_PARENT");
                if (l.Length > 0)
                {
                    l = l.Substring(13).Trim(' ', ';', ':');
                    if (!int.TryParse(l, out parent))
                        throw new IOException("[PARENT] Inconsistência no arquivo de dados .mox.");
                }
                else
                    throw new IOException("[PARENT] Inconsistência no arquivo de dados .mox.");
                l = MemoryWhileNotEqual(ref lines, ref posline, "TYPE");
                if (l.Length > 7)
                {
                    l = l.Substring(5).Trim(' ', ';', ':');
                    type = l.Substring(1, l.Length - 2);
                }
                else
                    throw new IOException("[TYPE] Inconsistência no arquivo de dados .mox.");

                l = MemoryWhileNotEqual(ref lines, ref posline, "COLOR");
                if (l.Length > 0)
                {
                    l = l.Substring(6).Trim(' ', ';', ':');
                    material = GetMaterial(l);
                }
                else
                    throw new IOException("[COLOR] Inconsistência no arquivo de dados .mox.");

                l = MemoryWhileNotEqual(ref lines, ref posline, "TRANSFORM");
                if (l.Length > 0)
                {
                    l = l.Substring(10).Trim(' ', ';', ':');
                    if (l.Length > 0 && transform.ByString(l, out var t))
                        transform = t ?? transform;
                }
                else
                    throw new IOException("[TRANSFORM] Inconsistência no arquivo de dados .mox.");

                l = MemoryWhileNotEqual(ref lines, ref posline, "");
                if (l.Length > 0)
                {
                    if (l == "POINTS")
                    {
                        GetPointsMemory(ref lines, ref posline, ref l, ref points);
                        if (l.Substring(0, 5) == "INDEX")
                        {
                            if (!GetIndex(l.Substring(7).Trim(' ', ';', ':'), ref index) || points.Count < 3)
                            {
                                points = null;
                                index = null;
                                transform.SetIdentify();
                            }
                            l = MemoryWhileNotEqual(ref lines, ref posline, "ENDSEC_POINTS");
                        }
                        else
                            throw new IOException("[INDEX] Inconsistência no arquivo de dados .mox.");
                        if (l != "ENDSEC_POINTS")
                            throw new IOException("[ENDSEC_POINTS] Inconsistência no arquivo de dados .mox.");

                    }
                    moxg.AddEntity(new MoxEntity(moxg.FileName, label, parent, type, material, points, index, transform));

                    l = MemoryWhileNotEqual(ref lines, ref posline, "");
                    if (l != "ENDSEC_ENTITY")
                        throw new IOException("[ENDSEC_ENTITY] Inconsistência no arquivo de dados .mox.");
                    l = MemoryWhileNotEqual(ref lines, ref posline, "");
                }
                else
                    throw new IOException("[ENTITY] Inconsistência no arquivo de dados .mox.");
            }
        }

        private static void GetPointsMemory(ref string[] lines, ref int posline, ref string l, ref List<MoxPoint3D> points)
        {
            l = MemoryWhileNotEqual(ref lines, ref posline, "");
            while (l.Length > 3 && l.Substring(0, 2) == "p:")
            {
                if (posline >= lines.Length) break;
                if(MoxPoint3D.StrToPoint(l.Substring(3), out MoxPoint3D p))
                    points.Add(p);
                l = MemoryWhileNotEqual(ref lines, ref posline, "");
            }
        }

        private static string MemoryWhileNotEqual(ref String [] lines, ref int posline, string val)
        {
            string l = "";
            int strlen = lines.Length;
            val = val.Trim(' ', ';', ':');
            do
            {
                posline++;
                if (posline >= strlen) break;
                l = lines[posline].Trim(' ', ';', ':');
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
                    if (posline >= strlen && l != val) return "";
                }

            } while (l == "" && posline < strlen);
            return l;
        }
    }
}

