using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    Camera cam;
    public float speed = 3f;
    Vector2 touchStartPos;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    Vector2 touchDelta = touch.position - touchStartPos;
                    transform.eulerAngles += new Vector3(touchDelta.y, touchDelta.x, 0) * speed * Time.deltaTime;
                    // Move the cube if the screen has the finger moving.
                    break;
            }
        }
        
#else
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.eulerAngles += new Vector3(vertical,horizontal, 0) * speed * Time.deltaTime;
#endif
    }
}
