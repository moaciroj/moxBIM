using System.Collections.Generic;
using UnityEngine;
using MoxGraphics.Geometry;

namespace MoxGraphics
{
    public partial class MoxGraphicsClass : MonoBehaviour
    {
        public GameObject Do3D(MoxGeometry g)
        {
            List<MoxEntity> lent = g.Entities;
            
            foreach (MoxEntity ent in lent)
            {
                MakeMesh(ent);
            }
            var obj = GameObject.Find(g.FileName);
            if (obj != null)
            {
                float scale = 1f;
                if (g.OneMetre >= 0)
                    scale = 1 / g.OneMetre;
                obj.transform.localScale = new Vector3(-scale, scale, scale);
            }
            return obj;
        }

        static GameObject MakeMesh(MoxEntity ent)
        {
            GameObject parent, gameobject;

            if (ent.Parent > 0)
                parent = GameObject.Find(ent.Parent.ToString());
            else
                parent = GameObject.Find(ent.File);
            if (parent == null)
                parent = new GameObject(ent.File);

            gameobject = new GameObject(ent.Label.ToString());

            gameobject.transform.parent = parent.transform;

            if (ent.Points == null && ent.Points.Count < 3)
                return gameobject;

            MeshRenderer meshRenderer = gameobject.AddComponent<MeshRenderer>();

            meshRenderer.sharedMaterial = GetUniqueMaterial(ent.Material.name, ent.Material.GetColor());

            MeshFilter meshFilter = gameobject.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();

            DoTrasnform(ent, ref gameobject, out var vertices, out var normals);

            Debug.Log(ent.Material.name);
            
            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            mesh.triangles = ent.Index.ToArray();

           //mesh.normals = normals;
            mesh.RecalculateNormals();

            mesh.Optimize();

            meshFilter.mesh = mesh;

            return gameobject;
        }

        static void DoTrasnform(MoxEntity ent, ref GameObject gameobject, out Vector3[] vertices, out Vector3[] normals)
        {
            vertices = new Vector3[ent.Points.Count];
            normals = new Vector3[ent.Points.Count];

            List<float[]> Lpts = ent.Points;
            if (Lpts != null && Lpts.Count > 0)
            {
                if (!ent.Transform.HasValue)
                {
                    for (int i = 0; i < Lpts.Count; i++)
                    {
                        vertices[i] = new Vector3(Lpts[i][1], Lpts[i][2], Lpts[i][0]);
                        normals[i] = new Vector3(Lpts[i][4], Lpts[i][5], Lpts[i][3]);
                    }
                }
                else
                {
                    var qtn = ent.Transform.Value.GetRotationQuaternion();
                    var quaternion = new Quaternion((float)qtn.X, (float)qtn.Y, (float)qtn.Z, (float)qtn.W);
                    MoxMatrix3D? matrix3D = ent.Transform.Value;

                    for (int i = 0; i < Lpts.Count; i++)
                    {
                        MoxPoint3D ptv = new MoxPoint3D(Lpts[i][0], Lpts[i][1], Lpts[i][2]);
                        var ptx = matrix3D.Value.Transform(ptv);
                        vertices[i] = new Vector3((float)ptx.Y, (float)ptx.Z, (float)ptx.X);

                        /*
                        MoxPoint3D ptn = new MoxPoint3D(Lpts[i][3], Lpts[i][4], Lpts[i][5]);
                        var ptr = matrix3D.Value.Transform(ptn);
                        normals[i] = new Vector3((float)ptr.Y, (float)ptr.Z, (float)ptr.X);
                        */
                    }
                }
            }
        }

        static Dictionary<string, Dictionary<MoxColor, UnityEngine.Material>> storedMaterials;

        static UnityEngine.Material GetUniqueMaterial(string Name , MoxColor color)
        {
            var shd = Shader.Find(Name);
            UnityEngine.Material mat;
            if (shd != null)
                mat = new UnityEngine.Material(shd);
            else
                mat = new UnityEngine.Material(Shader.Find("Standard"));

            if (storedMaterials == null) storedMaterials = new Dictionary<string,Dictionary<MoxColor, UnityEngine.Material>>();

            if (!storedMaterials.ContainsKey(Name))
            {
                Dictionary<MoxColor, UnityEngine.Material> newDict = new Dictionary<MoxColor, UnityEngine.Material>();
                storedMaterials.Add(Name, newDict);
            }
            
            if (!storedMaterials[Name].ContainsKey(color))
            {
                UnityEngine.Material clone = UnityEngine.Material.Instantiate(mat) as UnityEngine.Material;

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
