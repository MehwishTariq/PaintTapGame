using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColor : MonoBehaviour
{
    public Color objClr;
    public bool colored;
    public static Action<Color> onColorSelected;

    private void OnEnable()
    {
        onColorSelected += HighlightObject;
    }

    private void OnDisable()
    {
        onColorSelected -= HighlightObject;
    }

    void HighlightObject(Color clr)
    {
        GetComponent<Outline>().enabled = false;
        if (clr.Equals(objClr))
        {
            GetComponent<Outline>().enabled = true;
        }
    }

    [ContextMenu("SetOutlineColor")]
    void SetOutlineColor()
    {
        GetComponent<Outline>().OutlineColor = objClr;
    }

    [ContextMenu("GetCOlor")]
    public void GetColorFromMaterial()
    {
        objClr = gameObject.GetComponent<Renderer>().material.GetColor("_Color");
    }
}
