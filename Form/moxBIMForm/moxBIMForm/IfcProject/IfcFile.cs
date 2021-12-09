using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using Xbim.Common.Federation;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Geometry.Engine.Interop;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.ModelGeometry.Scene;
using Xbim.Common.Step21;
using Xbim.Common.XbimExtensions;
using e4 = Xbim.Ifc4;
using e2x3 = Xbim.Ifc2x3;
using MoxBIM.IO;
using MoxGraphics.Geometry;

namespace MoxProject
{
    public class IFCFileClass
    {
        //Classes públicas
        //FileInfo
        public string @FileFullName { get; private set; }
        public string @FileName { get; private set; }
        public string @FilePath { get; private set; }
        public string @FileDir { get; private set; }
        public DateTime FileDate { get; private set; }
        public double FileSize { get; private set; }
        public IfcStore model { get; private set; }
        private Xbim3DModelContext context { get; set; }

        //Sucesso
        public bool Success { get; private set; }

        protected ILogger Logger { get; private set; }
        public static ILoggerFactory LoggerFactory { get; private set; }

        //Constructor
        public IFCFileClass(string s)
        {
            try
            {
                IfcStore.ModelProviderFactory.CreateProvider();
                Logger = null ?? XbimLogging.CreateLogger<IFCFileClass>();
                LoggerFactory = new LoggerFactory();
                XbimLogging.LoggerFactory = LoggerFactory;
                Logger.LogInformation("Host logger created.");
                model = null;
                Success = false;
                var f = new FileInfo(@s);
                if (f != null && f.Exists)
                {
                    FileFullName = f.FullName;
                    FileName = f.Name;
                    FilePath = f.Directory.FullName;
                    FileDir = f.DirectoryName;
                    FileDate = f.LastWriteTimeUtc;
                    FileSize = f.Length;
                    Success = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public MoxGeometry IFCFileOpen()
        {
            //Memory Model
            Success = false;

            #region Lendo em memória
            /*
            using (var model = new MemoryModel(ef4))
            {
                model.LoadStep21(FileFullName);
                var project = model.Instances.FirstOrDefault<IfcProject>();
                try
                {
                    Xbim3DModelContext context;
                    context = new Xbim3DModelContext(model);
                    context.CreateContext();
                    Success = true;
                    if (model.GeometryStore.IsEmpty)
                    {
                        Logger.LogWarning("Geometry is empty in: {filename}", this.FileName);
                        return;
                    }
                    var schema = model.SchemaVersion;
                    switch (schema)
                    {
                        case XbimSchemaVersion.Ifc2X3:
                            ShowIfc2x3Data();
                            break;
                        case XbimSchemaVersion.Ifc4:
                            ShowIfc4Data();
                            break;
                        case XbimSchemaVersion.Ifc4x1:
                            ShowIfc4Data();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            */
            #endregion

            model = IfcStore.Open(FileFullName);

            MoxGeometry geometry = new MoxGeometry(FileName, (float)model.ModelFactors.OneMeter);

            if (model.GeometryStore.IsEmpty)
            {
                // Create the geometry using the XBIM GeometryEngine
                try
                {
                    context = new Xbim3DModelContext(model);
                    context.CreateContext();
                    Success = true;
                    if (model.GeometryStore.IsEmpty)
                    {
                        Logger.LogWarning("Geometry is empty in: {filename}", this.FileName);
                        return null;
                    }
                    var schema = model.SchemaVersion;

                    geometry = GetIfcData();

                    //var g = GetIfcData2();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return geometry;
        }

        private MoxGeometry GetIfcData()
        {
            MoxGeometry mygeo = new MoxGeometry(FileName, (float)model.ModelFactors.OneMetre);

            var instances = context.ShapeInstances();
            foreach (var instance in instances)
            {
                int label = instance.IfcProductLabel;
                int parent = instance.InstanceLabel;
                string type = "Get ExpressType"; //Search express type here
                MoxMaterial material = GetIfcMaterial(instance);
                List<MoxPoint3D> pts = null;
                List<int> idxs = null;
                XbimMatrix3D? t = instance.Transformation;
                MoxMatrix3D? transform = null;
                if (t.HasValue)
                    transform = new MoxMatrix3D(t.Value.M11, t.Value.M12, t.Value.M13, t.Value.M14, t.Value.M21, t.Value.M22, t.Value.M23, t.Value.M24, t.Value.M31, t.Value.M32, t.Value.M33, t.Value.M34, t.Value.OffsetX, t.Value.OffsetY, t.Value.OffsetZ, t.Value.M44);
                
                //get ifc geometry
                var geometry = context.ShapeGeometry(instance);
                var data = ((IXbimShapeGeometryData)geometry).ShapeData;
                using (var stream = new MemoryStream(data))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var mesh = reader.ReadShapeTriangulation();
                        List<float[]> ptx;
                        mesh.ToPointsWithNormalsAndIndices(out ptx, out idxs);
                        if (ptx.Count > 2 && idxs.Count % 3 == 0)
                        {
                            pts = new List<MoxPoint3D>();
                            foreach (var item in ptx)
                                pts.Add(new MoxPoint3D((double)item[0], (double)item[1], (double)item[2]));
                        }
                        else
                        {
                            pts = null;
                            idxs = null;
                            transform = null;
                        }
                    }
                }
                mygeo.AddEntity(FileName, label, parent, type, material, pts, idxs, transform);
            }
            return mygeo;
        }

        #region Bkp GetIfcData lesstime
        /*
        private MoxGeometry GetIfcData2()
        {
            MoxGeometry mygeo = new MoxGeometry(FileName, (float)model.ModelFactors.OneMetre);
            using (model)
            using (var txn = model.GeometryStore.BeginRead())
            {
                int p = 0;
                foreach (IXbimShapeGeometryData geo in txn.ShapeGeometries)
                {
                    int label = geo.IfcShapeLabel;
                    int parent = geo.ShapeLabel;
                    string type = "Get ExpressType"; //Search express type here
                    MoxMaterial material = GetIfcMaterial(txn.ShapeInstances.ElementAt(p));
                    List<float[]> pts = null;
                    List<int> idxs = null;
                    using (var ms = new MemoryStream(geo.ShapeData))
                    using (var br = new BinaryReader(ms))
                    {
                        var v = br.ReadShapeTriangulation();
                        v.ToPointsWithNormalsAndIndices(out pts, out idxs);

                    }
                    mygeo.AddEntity(FileName, label, parent, type, material, pts, idxs);
                    p++;
                }
            }
            return mygeo;
        }
        */
        #endregion

        protected MoxMaterial GetIfcMaterial(XbimShapeInstance instance)
        {
            MoxColor colorifc = new MoxColor();
            string name = "Standard";
            if (instance != null)
            {
                var styleId = instance.StyleLabel;
                if (styleId > 0 && instance.HasStyle)
                {
                    IfcMaterial ifcMaterial = new IfcMaterial();
                    var sStyle = model.Instances[styleId] as e4.Interfaces.IIfcSurfaceStyle;
                    if (sStyle != null)
                    {
                        var texture = XbimTexture.Create(sStyle);
                        if (texture.ColourMap.Count > 0)
                        {
                            if (texture.ColourMap[0].Alpha <= 0)
                            {
                                texture.ColourMap[0].Alpha = 0.5f;
                                Logger.LogWarning("Fully transparent style #{styleId} forced to 50% opacity.", styleId);
                            }
                        }
                        texture.DefinedObjectId = styleId;
                        ifcMaterial.CreateMaterial(texture);

                        colorifc.R = (byte)(ifcMaterial._material.Red * 255f);
                        colorifc.G = (byte)(ifcMaterial._material.Green * 255f);
                        colorifc.B = (byte)(ifcMaterial._material.Blue * 255f);
                        colorifc.A = ifcMaterial._material.Alpha;

                        if (ifcMaterial.Description != null) name = ifcMaterial.Description;
                    }
                }
            }
            return new MoxMaterial(colorifc, name);
        }
    }
}
