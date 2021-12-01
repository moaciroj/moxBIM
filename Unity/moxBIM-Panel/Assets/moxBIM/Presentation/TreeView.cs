using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoxGraphics.Geometry;

public class TreeView : MonoBehaviour
{
    public GameObject moxmesh;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject AddMesh (MoxEntity ent, GameObject parent)
    {
        GameObject instance = Instantiate(moxmesh, parent.transform);
        instance.name = ent.Label.ToString();
        instance.GetComponent<MoxMesh>().Entity = ent;
        instance.GetComponent<MoxMesh>().Show();
        return instance;
    }
}
