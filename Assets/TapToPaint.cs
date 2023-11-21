using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToPaint : MonoBehaviour
{
    Camera cam;
    RaycastHit info;
   

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out info, Mathf.Infinity))
            {
                if (info.collider.GetComponent<ObjectColor>().colored)
                    return;
                string clr1 = ColorUtility.ToHtmlStringRGBA(info.collider.gameObject.GetComponent<ObjectColor>().objClr);
                string clr2 = ColorUtility.ToHtmlStringRGBA(UIManager.chosenClr);
                if (clr1.Equals(clr2))
                {
                    info.collider.gameObject.GetComponent<ObjectColor>().colored = true;
                    info.collider.GetComponent<Renderer>().material.SetColor("_Color", UIManager.chosenClr);
                    info.collider.GetComponent<Outline>().enabled = false;
                }
            }
        }

    }
}
