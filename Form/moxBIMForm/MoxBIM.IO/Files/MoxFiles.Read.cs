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
                moxg.FileName = l.Substring(1, l.Length - 2);
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

        private void WhileEntity(ref StreamReader rd, ref string l, ref MoxGeometry moxg)
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


        private static void GetPoints(ref StreamReader rd, ref string l, ref List<float[]> points)
        {
            l = WhileNotEqual(ref rd, "");
            while (l.Length > 1 && l.Substring(0, 2) == "p[" && l.IndexOf(':') > 0)
            {
                if (rd.EndOfStream) break;
                float[] p = GetLinePoints(l.Substring(l.IndexOf(':') + 2));
                points.Add(p);
                l = WhileNotEqual(ref rd, "");
            }
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
    }
}

