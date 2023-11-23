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
        if (clr.Equals(objClr) && !colored)
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

    [ContextMenu("SETALLCOLORS")]
    public void SetOriginalColor()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", objClr);
    }

    [ContextMenu("SETALLCOLORSWHITE")]
    public void SetWhiteColor()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(209,209,209,255));
    }
}
