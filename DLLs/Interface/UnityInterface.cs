﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class UnityInterface : MonoBehaviour
    {
        private bool Aguardar;
        private float AguardarValor;

        public void Start()
        {
            Aguardar = true;
            //Buscar aqui as pré-configurações
            AguardarValor = 2f;

            //Se não houver pré-configurações adotar e escrever o default
        }

        public void OnGUI()
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                if (Aguardar && Input.GetKeyDown(KeyCode.F))
                {
                    Aguardar = false;
                    UnityInterface_AbrirIFC();
                    StartCoroutine(WaitFor(AguardarValor));
                }
            }
        }

        IEnumerator WaitFor(float s)
        {
            yield return new WaitForSecondsRealtime(s);
            Aguardar = true;
        }

        //
        //*****************************************************************************
        //Funções externas
        //*****************************************************************************
        //
        //Função para abrir o arquivo IFC on TIME no Unity
        public static void UnityInterface_AbrirIFC() 
        {
            if (IFC.UnityIFC.Instance_IFC.UnityIFC_AddFile())
            {
                Debug.Log("Desenhar IFC no núcleo Graphics");
            }
        }
    }

}
