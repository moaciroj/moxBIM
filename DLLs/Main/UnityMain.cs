using System;
using UnityEngine;
using UnityEngine.UI;
using Interface;
using IFC;

namespace Main
{
    public class UnityMain : MonoBehaviour
    {
        public static UnityInterface Instance_Interface { get; private set; }
        public static UnityIFC       Instance_IFC { get; private set; }
        public static UnityMain      Instance_Main { get; private set; }

        //Guardar a instancia da classe UnityMain
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

        public void Start()
        {
            //Instanciar a Classe IFC
            Instance_IFC = gameObject.AddComponent<UnityIFC>();
            //Ultima instancia Classe Interface
            Instance_Interface = gameObject.AddComponent<UnityInterface>();
            Debug.Log("moxBIM carregado!");
        }
    }
}
