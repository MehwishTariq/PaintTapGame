using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToPaint : MonoBehaviour
{
    Camera cam;
    RaycastHit info;
    Ray ray;
    bool panDone;

    private void Start()
    {
        cam = Camera.main;
    }
    private void OnEnable()
    {
        InputManager.OnPressed += PaintOnTap;
    }

    private void OnDisable()
    {
        InputManager.OnPressed -= PaintOnTap;
    }


    private void PaintOnTap(Vector2 pos)
    {
        if (TutorialController.TutorialStages == TutorialStages.ZoomIn)
            return;

        ray = cam.ScreenPointToRay(pos);

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        if (Physics.Raycast(ray, out info, Mathf.Infinity,layerMask))
        {
            if(info.collider.GetComponent<ObjectColor>().CheckIfCorrectColor(info.point))
            {
                AudioManager.Instance.PlayColorDone();
                if (PlayerPrefs.GetInt(Utility.Tutorial, 0) == 0)
                {
                    if (!panDone)
                    {
                        panDone = true;
                        TutorialController.InvokeNextEvent(TutorialController.cameraPan);
                    }
                    else
                    {
                        TutorialController.InvokeNextEvent(TutorialController.cameraZoom);
                    }
                }
            }
        }
    }

}
