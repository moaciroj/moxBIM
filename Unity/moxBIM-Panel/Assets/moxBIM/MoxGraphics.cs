using System;
using UnityEngine;
using MoxGraphics.Geometry;
using MoxBIM.IO;

namespace MoxGraphics
{
    public partial class MoxGraphicsClass : MonoBehaviour
    {
        MoxIOFile geometries = new MoxIOFile();

        public MoxGeometry currentgeometry { get; private set; }

        public GameObject moxw { get; private set; }

        void Start()
        {
            moxw = GameObject.Find("MoxWindowCanvas");
            currentgeometry = geometries.ReadFileGeometry(@"C:\Users\moaci\OneDrive\Área de Trabalho\Projeto AR Curso.mox");

            Do3D(currentgeometry);
            //PrintGeometry(currentgeometry);

            var gameobject = Resources.Load("moxMesh");
        }

        void Update()
        {
            
        }

        public bool AddGeometry(string lParam)
        {
            try
            {
                currentgeometry = geometries.ReadFileGeometry(lParam);
                ShowGeometry(currentgeometry);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ShowGeometry (MoxGeometry g)
        {
            if (g != null)
            {
                PrintGeometry(g);
                Do3D(g);
            }
        }
        private void PrintGeometry(MoxGeometry cgeo)
        {
            try
            {
                AddMoxWindow("** Geometry: " + cgeo.FileName + " **");
                AddMoxWindow("<< OneMetre: " + cgeo.OneMetre + " >>");
                AddMoxWindow();

                int tab = 8;
                for (int i = 0; i < cgeo.Entities.Count; i++)
                {
                    MoxEntity en = cgeo.Entities[i];
                    AddMoxWindow(Gsp(tab) + "> Entity [" + i.ToString() + "];");
                    AddMoxWindow(Gsp(tab * 2) + "- File:   " + en.File + ";");
                    AddMoxWindow(Gsp(tab * 2) + "- Label:  " + en.Label + ";");
                    AddMoxWindow(Gsp(tab * 2) + "- Parent: " + en.Parent + ";");
                    AddMoxWindow(Gsp(tab * 2) + "- Type: " + en.Type + ";");
                    MoxColor c = en.Material.GetColor();
                    AddMoxWindow(Gsp(tab * 2) + "- Color:  R[" + c.R.ToString() + "] G [" +
                                                                 c.G.ToString() + "] B [" +
                                                                 c.B.ToString() + "] A [" +
                                                                 c.A.ToString("R") + "] Name: " +
                                                                 en.Material.name + ";");
                    AddMoxWindow(Gsp(tab * 2) + "- Transform: " + en.Transform.ToString() + ";");
                    var pts = en.Points;
                    if (pts != null)
                    {
                        AddMoxWindow(Gsp(tab * 2) + "- Points: ");
                        for (int j = 0; j < pts.Count; j++)
                        {
                            AddMoxWindow(Gsp(tab * 3) + "p: " + pts[j].ToString() + ";");
                        }

                        AddMoxWindow(Gsp(tab * 2) + "- Index: " + en.IdxToString() + ";");
                    }
                    AddMoxWindow();
                }
            }
            catch (Exception ex)
            {
                AddMoxWindow("Erro: " + ex.Message);
                throw new Exception(ex.Message);
            }
            moxw.GetComponent<Canvas>().enabled = true;
            
        }

        private string Gsp(int x)
        {
            string s = "";
            for (int ii = 0; ii < x; ii++)
            {
                s += " ";
            }
            return s;
        }

        private void AddMoxWindow(string txt)
        {
            if (moxw != null)
            {
                moxw.GetComponent<MoxText>().AddLine(txt);
            }
        }

        private void AddMoxWindow()
        {
            if (moxw != null)
            {
                moxw.GetComponent<MoxText>().AddLine("");
            }
        }


        private void ClearMoxWindow()
        {
            if (moxw != null)
            {
                moxw.GetComponent<MoxText>().Clear();
            }
        }
    }
}
