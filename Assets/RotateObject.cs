using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float zoomSpeed = 0.5f;
    public float minZoom = 1f;
    public float maxZoom = 5f;

    private Vector2 touchStartPos;
    private Vector3 rotationEuler;
    private float initialPinchDistance;
    

    private void OnEnable()
    {
#if !UNITY_EDITOR
        rotationSpeed = 10f;
        zoomSpeed = 20f;
#else
        rotationSpeed = 100f;
        zoomSpeed = 10f;
#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR
        
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            float currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                initialPinchDistance = currentPinchDistance;
            }

            float pinchDifference = currentPinchDistance - initialPinchDistance;
            float zoomAmount = pinchDifference * zoomSpeed * Time.deltaTime;

            if (RectTransformUtility.RectangleContainsScreenPoint(UIManager.instance.area, touch0.position))
            {
                // Apply zoom to the object, clamped between minZoom and maxZoom
                Vector3 newScale = Vector3.ClampMagnitude(transform.localScale + new Vector3(zoomAmount, zoomAmount, zoomAmount), maxZoom);
                newScale = Vector3.Max(newScale, Vector3.one * minZoom);
                transform.localScale = newScale;

                initialPinchDistance = currentPinchDistance;
            }
        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    Vector2 touchDelta = touch.position - touchStartPos;

                    // Calculate the rotation angles based on touch input
                    float horizontalRotation = -touchDelta.x * rotationSpeed * Time.deltaTime;
                    float verticalRotation = touchDelta.y * rotationSpeed * Time.deltaTime;

                    if (RectTransformUtility.RectangleContainsScreenPoint(UIManager.instance.area, touch.position))
                    {
                        // Rotate the object locally
                        transform.Rotate(Vector3.up, horizontalRotation, Space.World);
                        transform.Rotate(Vector3.forward, verticalRotation, Space.World);

                        touchStartPos = touch.position; // Update the touch start position
                    }
                    break;
            }
        }
#else

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the rotation angles based on input
        float horizontalRotation = horizontalInput * rotationSpeed * Time.deltaTime;
        float verticalRotation = verticalInput * rotationSpeed * Time.deltaTime;
        // Rotate the object locally
        transform.Rotate(Vector3.up, horizontalRotation, Space.World);
        transform.Rotate(Vector3.forward, verticalRotation, Space.World);

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        float zoomAmount = scrollWheel * zoomSpeed * Time.deltaTime;
        Vector3 newScale = Vector3.ClampMagnitude(transform.localScale + new Vector3(zoomAmount, zoomAmount, zoomAmount), maxZoom);
        newScale = Vector3.Max(newScale, Vector3.one * minZoom);
        transform.localScale = newScale;
#endif
    }
}