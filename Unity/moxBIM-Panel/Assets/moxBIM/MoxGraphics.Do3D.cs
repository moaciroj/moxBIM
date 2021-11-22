using System.Collections.Generic;
using UnityEngine;
using MoxGraphics.Geometry;

namespace MoxGraphics
{
    public partial class MoxGraphicsClass : MonoBehaviour
    {
        public float width = 1;
        public float height = 1;

        public void Do3D(MoxGeometry g)
        {
            List<MoxEntity> lent = g.Entities;
            
            foreach (MoxEntity ent in lent)
            {
                GameObject parent, entity;

                if (ent.Parent > 0)
                    parent = GameObject.Find(ent.Parent.ToString());
                else
                    parent = GameObject.Find(ent.File);
                if (parent == null)
                    parent = new GameObject(ent.File);

                entity = new GameObject(ent.Label.ToString());

                entity.transform.parent = parent.transform;

                //Create entity geometry

                if (ent.Points == null && ent.Points.Count < 3)
                    break;

                MeshRenderer meshRenderer = entity.AddComponent<MeshRenderer>();
                
                meshRenderer.sharedMaterial = GetUniqueMaterial(ent.Material.name, ent.Material.GetColor());

                MeshFilter meshFilter = entity.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[ent.Points.Count];
                Vector3[] normals = new Vector3[ent.Points.Count];

                for (int i = 0; i < ent.Points.Count; i++)
                {
                    vertices[i] = new Vector3(ent.Points[i][0], ent.Points[i][2], ent.Points[i][1]);
                    normals[i] = new Vector3(ent.Points[i][3], ent.Points[i][5], ent.Points[i][4]);
                }

                mesh.vertices = vertices;

                mesh.triangles = ent.Index.ToArray();

                mesh.normals = normals;

                Vector2[] uvs = new Vector2[vertices.Length];

                for (int i = 0; i < uvs.Length; i++)
                {
                    uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
                }
                mesh.uv = uvs;

                meshFilter.mesh = mesh;
            }

            var obj = GameObject.Find(g.FileName);

            if (obj != null)
            {
                float scale = 1f;
                if (g.OneMetre >= 0)
                    scale = 1 / g.OneMetre;
                obj.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        Dictionary<string, Dictionary<MoxColor, Material>> storedMaterials;

        Material GetUniqueMaterial(string Name , MoxColor color)
        {
            var shd = Shader.Find(Name);
            Material mat;
            if (shd != null)
                mat = new Material(shd);
            else
                mat = new Material(Shader.Find("Standard"));

            if (storedMaterials == null) storedMaterials = new Dictionary<string,Dictionary<MoxColor, Material>>();

            if (!storedMaterials.ContainsKey(Name))
            {
                Dictionary<MoxColor, Material> newDict = new Dictionary<MoxColor, Material>();
                storedMaterials.Add(Name, newDict);
            }
            
            if (!storedMaterials[Name].ContainsKey(color))
            {
                Material clone = Material.Instantiate(mat) as Material;

                if (color.R < 0 || color.R > 255) color.R = 255; 
                if (color.G < 0 || color.G > 255) color.G = 255; 
                if (color.B < 0 || color.B > 255) color.B = 255;
                clone.color = new Color32((byte)color.R , (byte)color.G, (byte)color.B, (byte)(color.A * 255));
                storedMaterials[Name].Add(color, clone);
            }
            return storedMaterials[Name][color];
        }
    }
}
