using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float RotSpeedX = 10f;
    public float RotSpeedY = 10f;

    public Vector2 lastTappedVal;
    bool rotate;
    Vector3 orignalPos, originalRot;
    RectTransform RotateArea;
    bool disableRotate;

    private void Start()
    {
        orignalPos = transform.position;
        originalRot = transform.eulerAngles;
    }

    private void OnEnable()
    {
        InputManager.OnPressed += OnPressedInput;
        InputManager.OnHeld += RotateObjectOnAxis;

        EventManager.SubscribeToEvent(EventNames.OnCameraReset, ResetTransform);
        EventManager.SubscribeToEvent<bool>(EventNames.RotateStateChange, ChangeRotateState);
        RotateArea = InputManager.GetInputArea();
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeFromEvent(EventNames.OnCameraReset, ResetTransform);
        EventManager.UnsubscribeFromEvent<bool>(EventNames.RotateStateChange, ChangeRotateState);

        InputManager.OnPressed -= OnPressedInput;
        InputManager.OnHeld -= RotateObjectOnAxis;

    }

    private void OnPressedInput(Vector2 pos)
    {
        lastTappedVal = pos;
    }

    void ChangeRotateState(bool enable)
    {
        disableRotate = enable;
    }

    public void ResetTransform()
    {
        transform.position = orignalPos;
        transform.eulerAngles = originalRot;
        targetRotation = Quaternion.identity;
    }

    private float yawAngle;
    private float pitchAngle;
    private Quaternion targetRotation = Quaternion.identity;

    void RotateObjectOnAxis(Vector2 rotateVal)
    {
        if (InputManager.IsPinching)
        {
            lastTappedVal = rotateVal;
            return;
        }

        if (disableRotate ||
            TutorialController.TutorialStages < TutorialStages.Rotate)
        {
            lastTappedVal = rotateVal;
            return;
        }
        
        if (RectTransformUtility.RectangleContainsScreenPoint(RotateArea, lastTappedVal))
        {
            Vector2 moveDelta = rotateVal - lastTappedVal;

            yawAngle = -moveDelta.x * RotSpeedY * Time.deltaTime;
            //pitchAngle = -moveDelta.y * RotSpeedX * Time.deltaTime;  

            Quaternion yawRotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
            //Quaternion pitchRotation = Quaternion.AngleAxis(pitchAngle, Vector3.right);

            targetRotation = yawRotation * targetRotation;// * pitchRotation;

            // Assign
            transform.rotation = targetRotation;

            if (Mathf.Abs(moveDelta.x) > 0.01f && TutorialController.TutorialStages == TutorialStages.Rotate)
            {
                rotate = true;
                Invoke(nameof(TutorialDone), 1.5f);
            }

            lastTappedVal = rotateVal;
        }
    }


    void TutorialDone()
    {
        TutorialController.InvokeNextEvent(TutorialController.done);
    }

}