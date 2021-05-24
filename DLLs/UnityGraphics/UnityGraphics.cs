using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Draw
{
    public class UnityGraphics : MonoBehaviour
    {
        public static UnityGraphics Instance_Graphics { get; private set; }

        //Fazer aqui a classe IFC
        void Start()
        {
            //Abrir aqui as varíáveis do sistema
            //Abrir aqui os IFC que estão na pasta do projeto
            if (Instance_Graphics != null && Instance_Graphics != this)
            {
                GameObject.Destroy(Instance_Graphics);
            }
            else
            {
                Instance_Graphics = this;
            }
        }

        private static IFC.IFCFileClass UnityGraphics_GetLastIFC()
        {
            IFC.IFCFileClass LastIFCFileClass;
            LastIFCFileClass = IFC.UnityIFC.Instance_IFC.UnitIFC_GetLastIFC();
            return LastIFCFileClass;
        }

        public static void UnityGraphics_DrawLastModel()
        {
            IFC.IFCFileClass LastIFCFileClass = UnityGraphics_GetLastIFC();
            if(LastIFCFileClass != null)
            {
                Debug.Log("Draw IFC");
            }
            //Desenhar aqui o último IFC
            
        }
    }
}
