using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float zoomSpeed = 0.5f;
    public float minZoom = 1f;
    public float maxZoom = 5f;
    public Transform particle_fx;

    private Vector2 touchStartPos;
    private float initialPinchDistance;
    public Transform target { get; set; }

    Camera cam;
    Vector3 orignalPos, originalRot;

    float clockTime = 1f;

    private void Start()
    {
        cam = GetComponent<Camera>();
        orignalPos = transform.position;
        originalRot = target.eulerAngles;
        
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        rotationSpeed = 100f;
        zoomSpeed = 150f;
#else
        rotationSpeed = 5f;
        zoomSpeed = 0.5f;
#endif
    }
    bool rotate, zoom;
    

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
                if (zoomAmount > 0)
                {
                    Ray pos = cam.ScreenPointToRay(touch0.position);
                    TargetZoom(Mathf.Abs(zoomAmount), pos);
                }
                else
                {
                    ZoomOut(Mathf.Abs(zoomAmount));
                }

                if (PlayerPrefs.GetInt("Tutorial", 0) == 0 && !zoom)
                {
                    clockTime -= Time.deltaTime;
                    if (clockTime <= 0)
                    {
                        clockTime = 1;
                        zoom = true;
                        TutorialController.InvokeNextEvent(TutorialController.colorSelect);
                    }
                }
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
                        //transform.RotateAround(target.position, Vector3.forward, verticalRotation);

                        // Rotate the object locally
                        //transform.RotateAround(target.position, -Vector3.up, horizontalRotation);

                        //target.Rotate(-Vector3.forward, verticalRotation, Space.World);
                        target.Rotate(Vector3.up, horizontalRotation, Space.World);
                        if( PlayerPrefs.GetInt("Tutorial",0) == 0 && !rotate)
                        {
                            clockTime -= Time.deltaTime;
                            if (clockTime <= 0)
                            {
                                clockTime = 1;
                                rotate = true;
                                TutorialController.InvokeNextEvent(TutorialController.cameraZoom);
                            }
                        }
                    }
                    touchStartPos = touch.position; // Update the touch start position
                    break;
            }
        }
#else

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the rotation angles based on input
        float horizontalRotation = horizontalInput * rotationSpeed * Time.deltaTime;
        float verticalRotation = verticalInput * rotationSpeed * Time.deltaTime;

        
        //target.Rotate(-Vector3.forward, verticalRotation, Space.World);
        target.Rotate(Vector3.up, horizontalRotation, Space.World);

       // transform.RotateAround(target.position,Vector3.forward, verticalRotation);
       // transform.RotateAround(target.position, -Vector3.up, horizontalRotation);
        
        if(horizontalRotation != 0)
        {
            if( PlayerPrefs.GetInt("Tutorial",0) == 0 && !rotate)
            {
                clockTime -= Time.deltaTime;
                if (clockTime <= 0)
                {
                    clockTime = 1;
                    rotate = true;
                    TutorialController.InvokeNextEvent(TutorialController.cameraZoom);
                }
            }
        }
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        float zoomAmount = scrollWheel * zoomSpeed * Time.deltaTime;


        if (zoomAmount > 0)
        {
            Ray pos = cam.ScreenPointToRay(Input.mousePosition);
            TargetZoom(Mathf.Abs(zoomAmount), pos);
        }
        else
        {
            ZoomOut(Mathf.Abs(zoomAmount));
        }
        
        if(zoomAmount != 0)
        {
            if (PlayerPrefs.GetInt("Tutorial", 0) == 0 && !zoom)
            {
                clockTime -= Time.deltaTime;
                if (clockTime <= 0)
                {
                    clockTime = 1;
                    zoom = true;
                    TutorialController.InvokeNextEvent(TutorialController.colorSelect);
                }
            }
        }
#endif
    }


    public void ResetTransform()
    {
        AudioManager.Instance.PlayClick();
        transform.position = orignalPos;
        target.eulerAngles = originalRot;
    }

    void ZoomOut(float scrollVal)
    {
        transform.position = Vector3.MoveTowards(transform.position, orignalPos, scrollVal);
        particle_fx.localScale = Vector3.MoveTowards(particle_fx.localScale, Vector3.one * 2, scrollVal);
    }

    void TargetZoom(float scrollVal, Ray ray)
    {
        RaycastHit hit;
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, layerMask))
        {
            transform.position = Vector3.MoveTowards(transform.position, hit.point, scrollVal);
            particle_fx.localScale = Vector3.MoveTowards(particle_fx.localScale, Vector3.one * 0.5f, scrollVal);
        }
        else
        {
            Vector3 hitPoint = ray.origin + ray.direction;
            Vector3 nearPoint = GameManager.instance.levelManager.GetNearestPoint(hitPoint);
            transform.position = Vector3.MoveTowards(transform.position, nearPoint, scrollVal);
        }
    }

}
