using System.Collections;
using System.Windows.Forms;
using UnityEngine;
using MoxForm;

namespace MoxInterface
{
    public class MoxInterfaceClass : MonoBehaviour
    {
        public static MoxInterfaceClass Instance_Interface;
        private bool Aguardar;
        private float AguardarValor;
        public static string cmdInfo;

        public MoxGraphicsForm MyForm { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            //Abrir aqui as varíáveis do sistema
            //Abrir aqui os IFC que estão na pasta do projeto
            if (Instance_Interface != null && Instance_Interface != this)
            {
                GameObject.Destroy(Instance_Interface);
            }
            else
            {
                Instance_Interface = this;
            }
            //Buscar aqui as pré-configurações
            Aguardar = true;
            AguardarValor = 0.05f;
            cmdInfo = "";
            //Instanciar um projeto vazio
            //Verificar se há existente

            MyForm = new MoxGraphicsForm();
            MyForm.Show();
            MyForm.WindowState = FormWindowState.Minimized;
        }

        private void OnDestroy()
        {
           MyForm.Close();
        }

        void OnGUI()
        {
            if (Aguardar && Input.GetKeyDown(KeyCode.F4))
            {
                Aguardar = false;
                MyForm.WindowState = FormWindowState.Normal;
                MyForm.BringToFront();
                StartCoroutine(WaitFor(AguardarValor));
            }
        }

        IEnumerator WaitFor(float s)
        {
            yield return new WaitForSecondsRealtime(s);
            Aguardar = true;
        }

        #region Open File with File Browser Pró
        /*
        /// <summary>
        /// Função para abrir o arquivo IFC on TIME no Unity 
        /// </summary>
        private void UnityInterface_AbrirIFC()
        {
            string path = FileBrowser.Instance.OpenSingleFile("ifc");
            if(path.Length > 0)
            {
                UnityMain.Main_AddFileProject(path);
            }
        }
        */
        #endregion
    }
}