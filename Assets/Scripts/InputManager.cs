using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static Action<Vector2> OnPressed, OnHeld;
    public static Action OnReleased;
    public static Action<float, Vector3> OnPinchZoom;
    public RectTransform InputArea;
    public static Func<RectTransform> GetInputArea;

    public static bool IsTouch;
    private float initialPinchDistance;
    public static bool IsPinching;
    private int previousTouchCount;

    private void Awake()
    {
        GetInputArea += () =>
        {
            return InputArea;
        };
    }

    void Update()
    {
        if (Input.touchSupported && Input.touchCount > 0)
        {
            HandleTouch();
            return; // do not run mouse path while touching
        }

        IsPinching = false;
        HandleMouse();
    }

    private void HandleTouch()
    {
        IsPinching = Input.touchCount >= 2;

        if (previousTouchCount == 1 && Input.touchCount == 2)
        {
            OnReleased?.Invoke(); // stop pan when second finger lands
        }

        if (previousTouchCount == 2 && Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            OnPressed?.Invoke(t.position); // new pan start point
        }

        if (Input.touchCount == 2)
        {
            float zoomSpeed = 0.01f;

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
            OnPinchZoom?.Invoke(pinchDifference * zoomSpeed, pinchCenter);
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
                    IsTouch = true;
                    OnHeld?.Invoke(touch.position);
                    break;

                case TouchPhase.Ended:
                    OnReleased?.Invoke();
                    break;
            }
        }
        previousTouchCount = Input.touchCount;
    }
    private void HandleMouse()
    {
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
            IsTouch = false;
            OnHeld?.Invoke(Input.mousePosition);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float zoomSpeed = 1f;
            OnPinchZoom?.Invoke(Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Input.mousePosition);
        }


    }
}