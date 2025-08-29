using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float RotSpeedX = 10f;
    public float RotSpeedY = 10f;

    public Vector2 lastTappedVal;
    bool rotate;
    Vector3 orignalPos, originalRot;
    RectTransform RotateArea;

    private void Start()
    {
        orignalPos = transform.position;
        originalRot = transform.eulerAngles;
        ChangeRotateState(true);
    }

    private void OnEnable()
    {
        InputManager.OnPressed += ((mousePos) =>
        {
            lastTappedVal = mousePos;
        });

        EventManager.SubscribeToEvent(EventNames.OnCameraReset, ResetTransform);
        EventManager.SubscribeToEvent<bool>(EventNames.RotateStateChange, ChangeRotateState);
        RotateArea = InputManager.GetInputArea();
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeFromEvent(EventNames.OnCameraReset, ResetTransform);
        EventManager.UnsubscribeFromEvent<bool>(EventNames.RotateStateChange, ChangeRotateState);

        InputManager.OnPressed -= ((mousePos) =>
        {
            lastTappedVal = mousePos;
        });

    }

    void ChangeRotateState(bool enable)
    {
        if (enable)
        {
            InputManager.OnHeld += RotateObjectOnAxis;
        }
        else
        {
            InputManager.OnHeld -= RotateObjectOnAxis;
        }
    }

    public void ResetTransform()
    {
        transform.position = orignalPos;
        transform.eulerAngles = originalRot;
        lastTappedVal = Vector2.one;
    }

    private float yawAngle;
    private float pitchAngle; 
    private Quaternion targetRotation = Quaternion.identity;

    void RotateObjectOnAxis(Vector2 rotateVal)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(RotateArea, lastTappedVal))
        {
            Vector2 moveDelta = rotateVal - lastTappedVal;

            yawAngle = -moveDelta.x * RotSpeedY * Time.deltaTime;  
            //pitchAngle = -moveDelta.y * RotSpeedX * Time.deltaTime;  

            Quaternion yawRotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
            //Quaternion pitchRotation = Quaternion.AngleAxis(pitchAngle, Vector3.right);

            // Apply to targetRotation (order matters: local first, then world)
            targetRotation = yawRotation * targetRotation;// * pitchRotation;

            // Assign
            transform.rotation = targetRotation;

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