// Funcion Based in XbimVector3D //
// https://github.com/xBimTeam/XbimEssentials/blob/master/Xbim.Common/Geometry/XbimVector3D.cs
// End;

using System;
using System.Runtime.InteropServices;

namespace MoxGraphics.Geometry
{
    public interface IVector3D
    {
        double X { get; }
        double Y { get; }
        double Z { get; }
        bool IsInvalid();
    }
}
