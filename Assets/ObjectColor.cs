using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColor : MonoBehaviour
{
    public Color objClr;
    public bool colored;
    public static Action<Color> onColorSelected;

    void HighLightOnColorSelect(Color clr)
    {
        if (clr.Equals(objClr))
        {

        }
    }

    [ContextMenu("GetCOlor")]
    public void GetColorFromMaterial()
    {
        objClr = gameObject.GetComponent<Renderer>().material.GetColor("_Color");
    }
}
