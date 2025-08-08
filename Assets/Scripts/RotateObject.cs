using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float RotSpeedX = 10f;
    public float RotSpeedY = 10f;

    Vector2 lastTappedVal;
    bool rotate;
    Vector3 orignalPos, originalRot;

    private void Start()
    {
        orignalPos = transform.position;
        originalRot = transform.eulerAngles;
    }

    private void OnEnable()
    {
        InputManager.OnPressed += ((mousePos) =>
        {
            lastTappedVal = mousePos;
        });

        InputManager.OnHeld += RotateObjectOnAxis;
        UIManager.ResetTransforms += ResetTransform;
    }

    private void OnDisable()
    {
        UIManager.ResetTransforms -= ResetTransform;
    }


    public void ResetTransform()
    {
        transform.position = orignalPos;
        transform.eulerAngles = originalRot;
    }


    void RotateObjectOnAxis(Vector2 rotateVal)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(UIManager.instance.area, lastTappedVal))
        {
            Vector2 moveDelta = rotateVal - lastTappedVal;

            float yaw = -moveDelta.x * RotSpeedX;   // Rotate left/right
            float pitch = -moveDelta.y * RotSpeedY; // Rotate up/down

            if (transform == null)
                return;

            // Rotate relative to the object's local axes
            transform.Rotate(Vector3.up, yaw, Space.World);  // Rotate around world Y-axis (left/right)
            transform.Rotate(Vector3.forward, pitch, Space.World); // Rotate around local X-axis (up/down)

            if (yaw != 0)
            {
                if (PlayerPrefs.GetInt("Tutorial", 0) == 0 && !rotate)
                {
                    rotate = true;
                    Invoke(nameof(ZoomTutorialActivate), 1.5f);
                }
            }

            lastTappedVal = rotateVal;
        }
    }
    

    void ZoomTutorialActivate()
    {
        TutorialController.InvokeNextEvent(TutorialController.cameraZoom);
    }
    
}