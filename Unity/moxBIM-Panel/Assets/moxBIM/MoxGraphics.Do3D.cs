using System.Collections.Generic;
using UnityEngine;
using MoxGraphics.Geometry;
using System.Collections;
using CielaSpike;
using UnityEditor;

namespace MoxGraphics
{
    public partial class MoxGraphicsClass : MonoBehaviour
    {
        public GameObject Do3D(MoxGeometry g)
        {
            List<MoxEntity> lent = g.Entities;

            GameObject treeview = GameObject.Find("TreeView");

            foreach (MoxEntity ent in lent)
            {
                GameObject parent, gameobject;

                if (ent.Parent > 0)
                    parent = GameObject.Find(ent.Parent.ToString());
                else
                    parent = GameObject.Find(ent.File);
                
                if (parent == null)
                {
                    parent = new GameObject(ent.File);
                    parent.transform.parent = treeview.transform;
                }

                gameobject = treeview.GetComponent<TreeView>().AddMesh(ent, parent);
                //gameobject = MakeMesh(ent, parent);

                Undo.RegisterCreatedObjectUndo(gameobject, "Created Object");

                
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

        IEnumerator WaitFor(float s)
        {
            yield return new WaitForSecondsRealtime(s);
        }

        private GameObject MakeMesh(MoxEntity ent, GameObject parent)
        {
            GameObject gameobject = new GameObject(ent.Label.ToString());
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
            mesh.RecalculateNormals();
            mesh.Optimize();
            meshFilter.mesh = mesh;
            return gameobject;
        }

        private void DoTrasnform(MoxEntity ent, ref GameObject gameobject, out Vector3[] vertices, out Vector3[] normals)
        {
            vertices = new Vector3[ent.Points.Count];
            normals = new Vector3[ent.Points.Count];

            List<MoxPoint3D> Lpts = ent.Points;
            if (Lpts != null && Lpts.Count > 0)
            {
                if (!ent.Transform.HasValue)
                {
                    for (int i = 0; i < Lpts.Count; i++)
                        vertices[i] = new Vector3((float)Lpts[i].Y, (float)Lpts[i].Z, (float)Lpts[i].X);
                }
                else
                {
                    var qtn = ent.Transform.Value.GetRotationQuaternion();
                    var quaternion = new Quaternion((float)qtn.X, (float)qtn.Y, (float)qtn.Z, (float)qtn.W);
                    MoxMatrix3D? matrix3D = ent.Transform.Value;

                    for (int i = 0; i < Lpts.Count; i++)
                    {
                        MoxPoint3D ptv = new MoxPoint3D(Lpts[i].X, Lpts[i].Y, Lpts[i].Z);
                        var ptx = matrix3D.Value.Transform(ptv);
                        vertices[i] = new Vector3((float)ptx.Y, (float)ptx.Z, (float)ptx.X);
                    }
                }
            }
        }

        private static Dictionary<string, Dictionary<MoxColor, UnityEngine.Material>> storedMaterials;

        public static Material GetUniqueMaterial(string Name , MoxColor color)
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
