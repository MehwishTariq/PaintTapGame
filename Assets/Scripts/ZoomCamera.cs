using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public float zoomSpeed = 0.5f;
    public Transform particle_fx;
    Camera cam;
    Vector3 orignalPos, originalRot;
    bool zoom;
    RectTransform RotateArea;

    private void Start()
    {
        cam = GetComponent<Camera>();
        orignalPos = transform.position;
        originalRot = transform.eulerAngles;
        RotateArea = InputManager.GetInputArea();

    }

    private void OnEnable()
    {
#if MOBILE_INPUT
        zoomSpeed = 150f;
#else
        zoomSpeed = 20f;
#endif

        InputManager.OnScroll += Zoom;
        EventManager.SubscribeToEvent(EventNames.OnCameraReset, ResetTransform);
    }

    private void OnDisable()
    {
        InputManager.OnScroll -= Zoom;
        EventManager.UnsubscribeFromEvent(EventNames.OnCameraReset, ResetTransform);
    }

    public void ResetTransform()
    {
        transform.position = orignalPos;
        transform.eulerAngles = originalRot;
    }

    void Zoom(float scrollVal, Vector3 mousePos)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(RotateArea, mousePos))
        {
            float zoomAmount = scrollVal * zoomSpeed;

            if (zoomAmount > 0)
            {
                Ray pos = cam.ScreenPointToRay(mousePos);
                TargetZoom(Mathf.Abs(zoomAmount), pos);
            }
            else
            {
                ZoomOut(Mathf.Abs(zoomAmount));
            }

            if (zoomAmount != 0)
            {
                if (PlayerPrefs.GetInt(Utility.Tutorial, 0) == 0 && !zoom)
                {
                    zoom = true;
                    Invoke(nameof(ColorSelectTutorial), 1.5f);
                }
            }
        }
    }

    void ColorSelectTutorial()
    {
        TutorialController.InvokeNextEvent(TutorialController.colorSelect);
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
