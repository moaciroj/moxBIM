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
                    if(item.Transform.HasValue)
                        wt.WriteLine("TRANSFORM: " + item.Transform.Value.DoString() + ";");
                    else
                        wt.WriteLine("TRANSFORM: ;");
                    if (item.Points.Count > 0 && item.Index.Count > 0)
                    {
                        wt.WriteLine("POINTS;");
                        foreach (var itemp in item.Points)
                            wt.WriteLine("p: " + itemp.ToString() + ";");
                        wt.WriteLine("INDEX: " + item.IdxToString() + ";");
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

