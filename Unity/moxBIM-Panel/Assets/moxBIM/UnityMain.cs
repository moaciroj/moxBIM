using System;
using UnityEngine;
using MoxInterface;
using MoxGraphics;
using MoxBIM.IO;

namespace MoxMain
{
    public class UnityMain : MonoBehaviour
    {
        public string moxMessage { get; set; }

        public static UnityMain Instance_Main { get; private set; }
        public MoxInterfaceClass Main_Instance_Interface { get; private set; }
        public MoxGraphicsClass Main_Instance_Graphics { get; private set; }

        /// <summary>
        /// Guardar a instancia da classe UnityMain 
        /// </summary>
        public void Awake()
        {
            if (Instance_Main != null && Instance_Main != this)
            {
                GameObject.Destroy(Instance_Main);
            }
            else
            {
                Instance_Main = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            moxMessage = "Sistema moxBIM iniciado;";
            //Instanciar a Classe IFC
            Main_Instance_Graphics = gameObject.AddComponent<MoxGraphicsClass>();
            //Ultima instancia Classe Interface
            Main_Instance_Interface = gameObject.AddComponent<MoxInterfaceClass>();
            Debug.Log("Sistema moxBIM iniciado;");
        }

        void OnGUI()
        {
            if (moxMessage != null)
                GUI.Label(new Rect(10, 10, 400, 20), moxMessage);
        }

        /// <summary>
        /// Método para criar um projeto novo
        /// </summary>
        /// <param name="s">Nome do projeto</param>
        /// <param name="id">id do usuário proprietário</param>
        /// <returns></returns>
        public static bool Main_NewProject()
        {
            return true;
        }

        /// <summary>
        /// Função para adicionar um arquivo IFC no projeto
        /// </summary>
        /// <param name="s">Nome do arquivo IFC</param>
        /// <returns></returns>
        public static bool Main_AddFileProject(string s)
        {
            return true;
        }

        public bool OnMyEvent(int wParam, int type, string lParam)
        {
            bool resp = false;
            if (wParam > 0)
            {
                switch (type)
                {
                    case (int)MoxEnums.MOX_SENDGEOMETRY:
                        resp = AddGeometry(lParam);
                        break;
                }
            }
            return resp;
        }

        private bool AddGeometry(string lParam)
        {
            try
            {
                return Main_Instance_Graphics.AddGeometry(lParam);
            }
            catch(Exception e)
            {
                string s = "Erro: " + e.Message;
                Debug.Log(s);
            }
            return false;
        }
        
    }
}
