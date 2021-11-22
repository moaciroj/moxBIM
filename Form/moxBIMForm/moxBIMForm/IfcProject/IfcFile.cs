using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;
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
                List<float[]> pts = null;
                List<int> idxs = null;

                //get ifc geometry
                var geometry = context.ShapeGeometry(instance);
                var data = ((IXbimShapeGeometryData)geometry).ShapeData;
                using (var stream = new MemoryStream(data))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var mesh = reader.ReadShapeTriangulation();
                        mesh.ToPointsWithNormalsAndIndices(out pts, out idxs);
                    }
                }
                mygeo.AddEntity(FileName, label, parent, type, material, pts, idxs);
            }
            return mygeo;
        }

        protected MoxMaterial GetIfcMaterial(XbimShapeInstance instance)
        {
            MoxColor colorifc = new MoxColor();
            string name = "Standard";

            var styleId = instance.StyleLabel;
            if (styleId > 0 && instance.HasStyle)
            {
                IfcMaterial ifcMaterial = new IfcMaterial();

                var sStyle = model.Instances[styleId] as e4.Interfaces.IIfcSurfaceStyle;
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
            
            return new MoxMaterial(colorifc, name);
        }
    }
}
