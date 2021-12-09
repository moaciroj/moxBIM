using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Ifc;

namespace MoxProject
{
    public class IfcMaterial
    {
        public XbimColour _material { get; private set; }
        public string _description { get; private set; }
        public bool IsTransparent { get; private set; }

        // empty constructor
        public IfcMaterial()
        {
        }

        public IfcMaterial(XbimColour colour)
        {
            _material = MaterialFromColour(colour);
            _description = "Colour " + colour;
            IsTransparent = colour.IsTransparent;
        }


        public void CreateMaterial(XbimTexture texture)
        {
            if (texture.ColourMap.Count > 1)
            {
                List<XbimColour> _materialg = new List<XbimColour>();
                var descBuilder = new StringBuilder();
                descBuilder.Append("Texture");

                var transparent = true;
                foreach (var colour in texture.ColourMap)
                {
                    if (!colour.IsTransparent)
                        transparent = false; //only transparent if everything is transparent
                    descBuilder.AppendFormat(" {0}", colour);
                    _materialg.Add(colour);
                }
                _material = GetAverageMaterial(_materialg);
                _description = descBuilder.ToString();
                IsTransparent = transparent;
            }
            else if (texture.ColourMap.Count == 1)
            {
                var colour = texture.ColourMap[0];
                _material = MaterialFromColour(colour);
                _description = "Texture " + colour;
                IsTransparent = colour.IsTransparent;
            }
        }

        private XbimColour GetAverageMaterial(List<XbimColour> g)
        {
            float R = 0f;
            float G = 0f;
            float B = 0f;

            foreach (var c in g)
            {
                R += c.Red;
                G += c.Green;
                B += c.Blue;
            }

            R = (int)(R / g.Count);
            G = (int)(G / g.Count);
            B = (int)(B / g.Count);

            return MaterialFromColour(new XbimColour(R, G, B, 1, 1, 1, 1, 0, 0));
        }

        /// <summary>
        /// Obsolete, please use constructor instead. 17 May 2017
        /// </summary>
        /// <param name="colour"></param>
        [Obsolete]
        public void CreateMaterial(XbimColour colour)
        {
            _material = MaterialFromColour(colour);
            _description = "Colour " + colour;
            IsTransparent = colour.IsTransparent;
        }

        private XbimColour MaterialFromColour(XbimColour colour)
        {
            /*
            var col = Color.FromScRgb(colour.Alpha, colour.Red, colour.Green, colour.Blue);
            Brush brush = new SolidColorBrush(col);

            // build material
            Material mat;
            if (colour.SpecularFactor > 0)
                mat = new SpecularMaterial(brush, colour.SpecularFactor * 100);
            else if (colour.ReflectionFactor > 0)
                mat = new  EmissiveMaterial(brush);
            else
                mat = new DiffuseMaterial(brush);
            */
            return colour;
        }

        public string Description => _description;
        public bool IsCreated => _material != null;
    }
}
