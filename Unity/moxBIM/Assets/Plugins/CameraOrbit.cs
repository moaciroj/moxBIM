using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraOrbit : MonoBehaviour
{
    //Public
    [Header("Global")]
    public int CursorStyle;         //Estilo de cursor  (0 none) (1 Mira +) (2 Mira x) (3 Mira Dot)
    public int CamStyle;            //Estilo de câmera  (1 Fly) (2 Third Person) (4 First Person)(8)(16)(32)(64)(128) somatório
    public List<int> CamLst = new List<int>() { 1, 2, 4, 8, 16, 32, 64, 128 }; //Lista completa de câmeras
    public int CamStyleList;        //Somatório com as possíveis câmeras
    public int CamStyleLast;        //Ultima câmera setada
    [Space(10)]
    [Header("Distance")]
    public float CamMinDistance;
    public float CamMaxDistance;
    public float CamDistanceSmooth; //Sensibilidade
    [Space(10)]
    [Header("Look")]
    public float CamY_Smooth;       //Sensibilidade
    public float CamX_Smooth;       //Sensibilidade
    public float CamMinHeight;
    public float CamMaxHeight;
    public float CamPlayerOverHead;
    public float PlayerHeightLook;

    //Private
    private float CamDistance;
    private float LastDistance;
    private float CamRotX;
    private float CamRotY;
    private float CamHeight;


    private void Awake()
    {
        //Set System Variables
        //Distância da câmera
        CamDistance = 2f;
        LastDistance = 2f;

        //Mínima distância da Câmara
        CamMinDistance = -1;

        //Distância máxima da Câmara
        CamMaxDistance = 10f;

        //Altura máxima da câmera
        CamMaxHeight = 5f;

        //Altura dos olhos do jogador
        PlayerHeightLook = 1.45f;

        //Desnível da câmera sobre os olhos do player
        CamPlayerOverHead = 0.5f;

        //Sensibilidade do Scroll
        CamDistanceSmooth = 0.5f;

        //Sensibilidade da câmera em Y
        CamY_Smooth = 1f;

        //Sensibilidade da câmera em X
        CamY_Smooth = 2f;

        //Estilo da câmera
        CamStyle = 1;
        CamStyleList = 7;
        CamStyleLast = 0; //Forçar primeira errada

        //Estilo do cursor
        CursorStyle = 2;
    }

    private void Start()
    {
        SetCursor(true, CursorStyle);
    }

    private void Update()
    {
        if (CamStyleList != CamStyleLast)
        {
            List<int> LstCam = new List<int>() { 1, 2, 4, 8, 16, 32, 64, 128 }; //Lista completa de câmeras
            int Cam = CamStyleList;
            CamLst.Reverse();
            foreach (int i in CamLst)
            {
                if (i <= Cam)
                {
                    Cam -= i;
                }
                else
                {
                    LstCam.Remove(i);
                }
            }
            CamStyleLast = CamStyleList;
            CamLst = LstCam;
        }

        if (!CamLst.Contains(CamStyle)) { CamStyle = CamLst[0]; }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                int pos = CamLst.IndexOf(CamStyle);
                pos -= 1;
                if (pos < 0) { pos = CamLst.Count - 1; }
                CamStyle = CamLst[pos];
            }
            else
            {
                int pos = CamLst.IndexOf(CamStyle);
                pos += 1;
                if (pos > (CamLst.Count - 1)) { pos = 0; }
                CamStyle = CamLst[pos];
            }
        }

        switch (CamStyle)
        {
            case 1: //Sem o third person com câmera livre
                //Travando a câmera na pessoa
                CamDistance = -0.3f;
                //Ocultando o third persons
                CamHeight = PlayerHeightLook + CamPlayerOverHead;

                CamRotY = 0;
                CamRotY = 0;
                SetCursor(true, 0);
                break;
            case 2: // Se for Third Person no chão com câmera travada em X e Livre em Y
                CamDistance = LastDistance;
                //get data mouse;
                CursorStyle = 0;
                float x = Input.GetAxisRaw("Mouse X");
                float y = -Input.GetAxisRaw("Mouse Y");
                float Scroll = -Input.mouseScrollDelta.y;

                //apply y (rotate x)
                CamRotY += x * CamX_Smooth;
                //apply x (rotate y)
                CamRotX += y * CamY_Smooth;

                //Calculate
                CamDistance += Scroll * CamDistanceSmooth;
                CamDistance = Mathf.Clamp(CamDistance, CamMinDistance, CamMaxDistance);
                CamHeight = PlayerHeightLook + CamPlayerOverHead + ((CamMaxHeight - (PlayerHeightLook + CamPlayerOverHead)) / CamMaxDistance) * CamDistance;
                LastDistance = CamDistance;
                SetCursor(false, CursorStyle);
                break;
            case 4: // Se for Third Person Na Primeira pessoa
                //Travando a câmera na pessoa
                CamDistance = -0.3f;
                CamHeight = PlayerHeightLook;
                //Ocultando o third persons
                SetCursor(true, CursorStyle);
                break;
        }

    }

    private void LateUpdate()
    {
        //Rotacionar a camera no eixo X (Olhar para cima e para Baixo)
       
        transform.localPosition = new Vector3(0, CamHeight, -CamDistance);
        
        switch (CamStyle)
        {
            case 2: // Se for Third Person no chão com câmera travada em X e Livre em Y
                transform.localRotation = Quaternion.Euler(CamRotX, 0, 0);
                //Rotacionar o third person no eixo Y (Olhar para os lados)


                //Substituir o W e Seta Up 
                break;
        }
       
    }

    public void SetCursor(bool Visivel, int Style)
    {
        switch (Style)
        {
            case 0:
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case 1:
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
        Cursor.visible = Visivel;
    }

    
}

