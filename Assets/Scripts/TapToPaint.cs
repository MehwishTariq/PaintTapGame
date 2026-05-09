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

        RectTransform rawImageRect = InputManager.GetInputArea();
        // 1. Get the local point on the UI element
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImageRect, pos, null, out var localPoint);

        // 2. Convert local point to a 0-1 range (Viewport Space)
        float x = (localPoint.x / rawImageRect.rect.width) + 0.5f;
        float y = (localPoint.y / rawImageRect.rect.height) + 0.5f;

        // 3. Cast the ray from your Render Camera
        Ray ray = cam.ViewportPointToRay(new Vector3(x, y, 0));

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        if (Physics.Raycast(ray, out info, Mathf.Infinity, layerMask))
        {
            if (info.collider.GetComponent<ObjectColor>().CheckIfCorrectColor(info.point))
            {
                AudioManager.Instance.PlayColorDone();
                if (TutorialController.TutorialStages == TutorialStages.Paint_1)
                {
                    TutorialController.InvokeNextEvent(TutorialController.cameraPan);
                }

                else if (TutorialController.TutorialStages == TutorialStages.Paint_2)
                {
                    TutorialController.InvokeNextEvent(TutorialController.cameraZoomOut);
                }
            }
        }
    }

}
