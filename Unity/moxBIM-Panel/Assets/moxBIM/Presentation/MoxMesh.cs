using System.Collections.Generic;
using UnityEngine;
using MoxGraphics.Geometry;
using MoxGraphics;

public class MoxMesh: MonoBehaviour
{
    public MoxEntity Entity { get; set; }
    private bool show = false;
    private bool onetime = true;
    public void Show ()
    {
        show = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (show && onetime)
            if (Entity != null)
                DoMakeMesh(Entity);
        show = false;
        onetime = false;
    }

    public void Clone()
    {
        onetime = true;
        Show();
    }

    private GameObject DoMakeMesh(MoxEntity ent)
    {
        if (ent.Points == null && ent.Points.Count < 3)
            return gameObject;
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = MoxGraphicsClass.GetUniqueMaterial(ent.Material.name, ent.Material.GetColor());
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        DoTrasnform(ent, out var vertices);
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.triangles = ent.Index.ToArray();
        mesh.RecalculateNormals();
        mesh.Optimize();
        meshFilter.mesh = mesh;
        return gameObject;
    }

    private void DoTrasnform(MoxEntity ent, out Vector3[] vertices)
    {
        vertices = new Vector3[ent.Points.Count];

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
}
