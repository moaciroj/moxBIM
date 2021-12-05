using System;
using UnityEngine;
using MoxGraphics.Geometry;
using MoxBIM.IO;
using System.Collections.Generic;

namespace MoxGraphics
{
    public partial class MoxGraphicsClass : MonoBehaviour
    {
        private MoxIOFile geometries = new MoxIOFile();

        private int MaxLinesMoxText = 100;
        private List<GameObject> ListMoxText = new List<GameObject>();

        public MoxGeometry currentgeometry { get; private set; }

        private GameObject moxw { get; set; }

        void Start()
        {
            moxw = GameObject.Find("MoxWindowCanvas");

            AddMoxWindow("> Sistema moxBIM iniciado;");
            AddMoxWindow();

            /*
            currentgeometry = geometries.ReadFileGeometry(@"C:\Users\moaci\OneDrive\Área de Trabalho\Projeto AR Curso.mox");
            Do3D(currentgeometry);
            PrintGeometry(currentgeometry);
            */
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
                AddMoxWindow("> Geometry: " + cgeo.FileName + ";");
                AddMoxWindow("> OneMetre: " + cgeo.OneMetre + ";");
                AddMoxWindow("> Entities: " + cgeo.Entities.Count + ";");
                AddMoxWindow();
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

        public void AddMoxWindow(string txt)
        {
            if (moxw != null)
            {
                if (ListMoxText.Count > MaxLinesMoxText)
                    Destroy(ListMoxText[0]);
                var go = moxw.GetComponent<MoxText>().AddLine(txt);
                ListMoxText.Add(go);
            }
        }

        public void AddMoxWindow()
        {
            AddMoxWindow("");
        }

        public void ClearMoxWindow()
        {
            foreach (GameObject item in ListMoxText)
            {
                Destroy(item);
            }
        }
    }
}
