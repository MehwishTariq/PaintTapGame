using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColor : MonoBehaviour
{
    public Color objClr;
    public bool colored;

    [ContextMenu("GetCOlor")]
    public void GetColorFromMaterial()
    {
        objClr = gameObject.GetComponent<Renderer>().material.GetColor("_Color");
    }
}
