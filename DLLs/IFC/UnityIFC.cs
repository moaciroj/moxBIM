using System;
using UnityEngine;
using UnityEngine.UI;

namespace IFC
{
    public class UnityIFC : MonoBehaviour
    {
        private IFCClass UIfc;

        public bool UnitIFC_Read = false;
        
        //Fazer aqui a classe IFC
        void Start()
        {
            UIfc = new IFCClass();
            //Abrir aqui as varíáveis do sistema

            //Abrir aqui os IFC que estão na pasta do projeto
        }

        public void UnityIFC_AddFile()
        {
            UnitIFC_Read = UIfc.IFCClass_AddFile();
            if (UnitIFC_Read)
            {
               
                Debug.Log("Entrou na UnitAbrir");
            }
        }
    }
}
