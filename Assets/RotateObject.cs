using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float speed= 0;
    Vector2 touchStartPos, rotationEuler;

    private void OnEnable()
    {
#if UNITY_EDITOR
        speed = 0.01f;
#else
        speed = 10f;
#endif
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
                rotationEuler.x -= -touchDelta.y;
                rotationEuler.y -= touchDelta.x;

                transform.localRotation = Quaternion.Euler(rotationEuler * speed);
                    // Move the cube if the screen has the finger moving.
                    break;
            }
        }

#else
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        rotationEuler.x += -vertical;
        rotationEuler.y -= horizontal;

        transform.localRotation = Quaternion.Euler(rotationEuler * speed);
#endif
    }
}
