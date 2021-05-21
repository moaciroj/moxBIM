using System;
using UnityEngine;
using UnityEngine.UI;

namespace IFC
{
    public class UnityIFC : MonoBehaviour
    {
        private IFCProject IfcC;

        public static bool UnityIFC_Read = false;

        public static UnityIFC Instance_IFC { get; private set; }

        //Fazer aqui a classe IFC
        void Start()
        {
            IfcC = new IFCProject();
            //Abrir aqui as varíáveis do sistema
            //Abrir aqui os IFC que estão na pasta do projeto
            if (Instance_IFC != null && Instance_IFC != this)
            {
                GameObject.Destroy(Instance_IFC);
            }
            else
            {
                Instance_IFC = this;
            }
        }

        public bool UnityIFC_AddFile()
        {
            UnityIFC_Read = IfcC.IFCProject_AddFile();
            return UnityIFC_Read;
        }
    }
}
