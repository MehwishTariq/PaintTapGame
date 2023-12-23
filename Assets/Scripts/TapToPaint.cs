using System;
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
    Ray ray;

    private void Update()
    {
#if !UNITY_EDITOR
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = cam.ScreenPointToRay(touch.position);
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 2;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            if (Physics.Raycast(ray, out info, Mathf.Infinity,layerMask))
            {
                if (info.collider.GetComponent<ObjectColor>().colored)
                    return;
                string clr1 = ColorUtility.ToHtmlStringRGBA(info.collider.gameObject.GetComponent<ObjectColor>().objClr);
                string clr2 = ColorUtility.ToHtmlStringRGBA(UIManager.chosenClr);
                if (!info.collider.gameObject.GetComponent<ObjectColor>().colored)
                {
                    if (clr1.Equals(clr2))
                    {
                        info.collider.gameObject.GetComponent<ObjectColor>().colored = true;
                        ObjectColor.onColored?.Invoke(UIManager.chosenClr);
                        info.collider.GetComponent<Renderer>().material.SetColor("_Color", UIManager.chosenClr);
                        info.collider.GetComponent<Outline>().enabled = false;
                        LevelManager.checkLevel?.Invoke(info.collider.gameObject.GetComponent<ObjectColor>().colored);
                        AudioManager.Instance.PlayColorDone();
                    }
                }
            }
        }
#else

        if (Input.GetMouseButtonDown(0))
        {
           Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 2;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            if (Physics.Raycast(ray, out info, Mathf.Infinity,layerMask))
            {
                if (info.collider.GetComponent<ObjectColor>().colored)
                    return;
                string clr1 = ColorUtility.ToHtmlStringRGBA(info.collider.gameObject.GetComponent<ObjectColor>().objClr);
                string clr2 = ColorUtility.ToHtmlStringRGBA(UIManager.chosenClr);
                if (!info.collider.gameObject.GetComponent<ObjectColor>().colored)
                {
                    if (clr1.Equals(clr2))
                    {
                        ColorChange.changePos?.Invoke(info.point);
                        info.collider.gameObject.GetComponent<ObjectColor>().colored = true;
                        ObjectColor.onColored?.Invoke(UIManager.chosenClr);
                        info.collider.GetComponent<Renderer>().material.SetColor("_Color", UIManager.chosenClr);
                        info.collider.GetComponent<Outline>().enabled = false;
                        info.collider.GetComponent<Renderer>().material.SetTexture("_MainTex", null);
                        LevelManager.checkLevel?.Invoke(info.collider.gameObject.GetComponent<ObjectColor>().colored);
                        AudioManager.Instance.PlayColorDone();
                    }
                }
            }
        }
#endif
    }
}
