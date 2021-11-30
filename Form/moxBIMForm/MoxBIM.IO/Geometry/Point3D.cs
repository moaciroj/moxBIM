// Funcion Based in XbimPoint3D //
// https://github.com/xBimTeam/XbimEssentials/blob/master/Xbim.Common/Geometry/XbimPoint3D.cs
// End;

using System;
using System.Runtime.InteropServices;

namespace MoxGraphics.Geometry
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MoxPoint3D
    {
        const double Tolerance = 1e-9;
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public readonly static MoxPoint3D Zero;

        static MoxPoint3D()
        {
            Zero = new MoxPoint3D(0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public MoxPoint3D(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }



        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X, Y, Z);
        }



        public override bool Equals(object ob)
        {
            if (ob is MoxPoint3D)
            {
                MoxPoint3D v = (MoxPoint3D)ob;
                return (Math.Abs(X - v.X) < Tolerance && Math.Abs(Y - v.Y) < Tolerance && Math.Abs(Z - v.Z) < Tolerance);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }


        public static MoxPoint3D operator +(MoxPoint3D p, MoxVector3D v)
        {
            return Add(p, v);
        }
        /// <summary>
        /// Adds a XbimPoint3D structure to a XbimVector3D and returns the result as a XbimPoint3D structure.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static MoxPoint3D Add(MoxPoint3D p, MoxVector3D v)
        {
            return new MoxPoint3D(p.X + v.X,
                                    p.Y + v.Y,
                                    p.Z + v.Z
                                    );
        }

        public static MoxPoint3D Add(MoxPoint3D p, MoxPoint3D v)
        {
            return new MoxPoint3D(p.X + v.X,
                                    p.Y + v.Y,
                                    p.Z + v.Z
                                    );
        }

        public static MoxPoint3D operator *(MoxPoint3D p, MoxMatrix3D m)
        {
            return Multiply(p, m);
        }



        public static MoxPoint3D Multiply(MoxPoint3D p, MoxMatrix3D m)
        {
            var x = p.X;
            var y = p.Y;
            var z = p.Z;



            MoxPoint3D pRet = new MoxPoint3D(m.M11 * x + m.M21 * y + m.M31 * z + m.OffsetX,
                                   m.M12 * x + m.M22 * y + m.M32 * z + m.OffsetY,
                                   m.M13 * x + m.M23 * y + m.M33 * z + m.OffsetZ
                                  );
            if (m.IsAffine) return pRet;
            double affineRatio = x * m.M14 + y * m.M24 + z * m.M34 + m.M44;
            x = pRet.X / affineRatio;
            y = pRet.Y / affineRatio;
            z = pRet.Z / affineRatio;
            return new MoxPoint3D(x, y, z);
        }
        public static MoxVector3D operator -(MoxPoint3D a, MoxPoint3D b)
        {
            return Subtract(a, b);
        }
        public static MoxPoint3D operator -(MoxPoint3D a, MoxVector3D b)
        {
            return new MoxPoint3D(a.X - b.X,
                                    a.Y - b.Y,
                                    a.Z - b.Z);
        }
        public static MoxVector3D Subtract(MoxPoint3D a, MoxPoint3D b)
        {
            return new MoxVector3D(a.X - b.X,
                                    a.Y - b.Y,
                                    a.Z - b.Z);
        }
    }
}