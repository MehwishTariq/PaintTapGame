using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float RotSpeedX = 10f;
    public float RotSpeedY = 10f;

    Vector2 lastTappedVal;
    bool rotate;
    Vector3 orignalPos, originalRot;
    RectTransform RotateArea;

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
        EventManager.SubscribeToEvent(EventNames.OnCameraReset, ResetTransform);
        RotateArea = InputManager.GetInputArea();
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeFromEvent(EventNames.OnCameraReset, ResetTransform);
        InputManager.OnPressed -= ((mousePos) =>
        {
            lastTappedVal = mousePos;
        });

        InputManager.OnHeld -= RotateObjectOnAxis;
    }


    public void ResetTransform()
    {
        transform.position = orignalPos;
        transform.eulerAngles = originalRot;
    }

    private float yawAngle;
    private float pitchAngle;

    void RotateObjectOnAxis(Vector2 rotateVal)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(RotateArea, lastTappedVal))
        {
            Vector2 moveDelta = rotateVal - lastTappedVal;

            yawAngle -= moveDelta.x * RotSpeedY * Time.deltaTime;  // Horizontal
            pitchAngle += moveDelta.y * RotSpeedX * Time.deltaTime;  // Vertical

            // Build rotation using quaternions to avoid gimbal lock
            Quaternion rotation = Quaternion.Euler(-pitchAngle, yawAngle, 0f);
            transform.rotation = rotation;

            if (Mathf.Abs(moveDelta.x) > 0.01f && PlayerPrefs.GetInt(Utility.Tutorial, 0) == 0 && !rotate)
            {
                rotate = true;
                Invoke(nameof(ZoomTutorialActivate), 1.5f);
            }

            lastTappedVal = rotateVal;
        }
    }
    

    void ZoomTutorialActivate()
    {
        TutorialController.InvokeNextEvent(TutorialController.cameraZoom);
    }
    
}