using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static Action<Vector2> OnPressed, OnHeld;
    public static Action OnReleased;
    public static Action<float,Vector3> OnPinchZoom;
    public RectTransform InputArea;
    public static Func<RectTransform> GetInputArea;

    private float initialPinchDistance;

    private void Awake()
    {
        GetInputArea += () =>
        {
            return InputArea;
        };
    }

    private void Update()
    {

#if UNITY_ANDROID && !UNITY_EDITOR

        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            float currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                initialPinchDistance = currentPinchDistance;
                return;
            }
            float pinchDifference = currentPinchDistance - initialPinchDistance;
            initialPinchDistance = currentPinchDistance;

            // midpoint between fingers
            Vector2 pinchCenter = (touch0.position + touch1.position) * 0.5f;

            OnPinchZoom?.Invoke(pinchDifference, pinchCenter);
            

        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnPressed?.Invoke(touch.position);
                    break;

                case TouchPhase.Moved:
                    OnHeld?.Invoke(touch.position);
                    break;

                case TouchPhase.Ended:
                    OnReleased?.Invoke();
                    break;
            }
        }

#else

        if (Input.GetMouseButtonDown(0))
        {
            OnPressed?.Invoke(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnReleased?.Invoke();
        }

        if (Input.GetMouseButton(0))
        {
            OnHeld?.Invoke(Input.mousePosition);
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
            OnPinchZoom?.Invoke(Input.GetAxis("Mouse ScrollWheel"), Input.mousePosition);

#endif
    }
}