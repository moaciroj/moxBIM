using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.IO;
using System.IO;
using UnityEngine.UI;

public class MoxText : MonoBehaviour
{
    public Transform contentwindow;

    public GameObject textmox;

    private bool Aguardar = true;
    private float AguardarValor = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (Aguardar && Input.GetKeyDown(KeyCode.F3))
        {
            GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
            Aguardar = false;
            StartCoroutine(WaitFor(AguardarValor));
        }
    }

    IEnumerator WaitFor(float s)
    {
        yield return new WaitForSecondsRealtime(s);
        Aguardar = true;
    }

    public void AddLine(string l)
    {
        var txt = Instantiate(textmox, contentwindow);
        txt.GetComponent<Text>().text = l;
    }

    public void Clear()
    {
        var l = textmox.GetComponents<MoxText>();
        foreach (var item in l)
        {
            Destroy(item);
        }
    }
}
