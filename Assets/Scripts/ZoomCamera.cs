using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public float zoomSpeed;
    public float moveSpeed;
    bool zoomed;
    public Transform particle_fx;
    Camera cam;
    Vector3 orignalPos, originalRot;
    bool zoomTutorial;
    RectTransform RotateArea;
    Vector2 lastTappedVal;
    Collider objInview;

    private void Start()
    {
        cam = GetComponent<Camera>();
        orignalPos = transform.position;
        originalRot = transform.eulerAngles;
        RotateArea = InputManager.GetInputArea();

    }

    private void OnEnable()
    {
        InputManager.OnPinchZoom += Zoom;
        InputManager.OnHeld += PanCamera;
        InputManager.OnPressed += (pressedVal) =>
        {
            lastTappedVal = pressedVal;
        };
        EventManager.SubscribeToEvent(EventNames.OnCameraReset, ResetTransform);
    }

    private void OnDisable()
    {
        InputManager.OnPinchZoom -= Zoom;
        InputManager.OnHeld -= PanCamera;
        InputManager.OnPressed -= (pressedVal) =>
        {
            lastTappedVal = pressedVal;
        };
        EventManager.UnsubscribeFromEvent(EventNames.OnCameraReset, ResetTransform);
    }

    public void ResetTransform()
    {
        transform.position = orignalPos;
        transform.eulerAngles = originalRot;
    }

    void Zoom(float zoomVal, Vector3 mousePos)
    {
        //if (RectTransformUtility.RectangleContainsScreenPoint(RotateArea, mousePos))
        //{
            float zoomAmount = zoomVal * zoomSpeed;

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
                if (PlayerPrefs.GetInt(Utility.Tutorial, 0) == 0 && !zoomTutorial)
                {
                    zoomTutorial = true;
                    Invoke(nameof(ColorSelectTutorial), 1.5f);
                }
            }
        //}
    }

    void ColorSelectTutorial()
    {
        TutorialController.InvokeNextEvent(TutorialController.colorSelect);
    }

    void PanCamera(Vector2 moveVal)
    {
        if (!zoomed)
            return;

        
        Vector2 moveDelta = moveVal - lastTappedVal;
        
        Vector3 right = transform.right;   // camera’s right direction
        Vector3 up = transform.up;      // camera’s up direction

        Vector3 move = (right * moveDelta.x + up * moveDelta.y) * moveSpeed * Time.deltaTime;
        transform.position += move;

        if (objInview != null)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
            Bounds bounds = objInview.bounds;

            if (!GeometryUtility.TestPlanesAABB(planes, bounds))
            {
                // Revert movement if object goes out of view
                transform.position -= move;
            }
        }
        lastTappedVal = moveVal;
    }


    void ZoomOut(float zoomVal)
    {
        transform.position = Vector3.MoveTowards(transform.position, orignalPos, zoomVal);
        particle_fx.localScale = Vector3.MoveTowards(particle_fx.localScale, Vector3.one * 2, zoomVal);

        if(zoomed && transform.position.Equals(orignalPos))
        {
            zoomed = false;
            EventManager.TriggerEvent(EventNames.RotateStateChange, true);
        }
    }

    void TargetZoom(float zoomVal, Ray ray)
    {        
        RaycastHit hit;
        // Bit shift the index of the layer (2) to get a bit mask
        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 2.
        // But instead we want to collide against everything except layer 2. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        if (Physics.SphereCast(ray.origin,0.5f, ray.direction, out hit, Mathf.Infinity, layerMask))
        //if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, layerMask))
        {
            transform.position = Vector3.MoveTowards(transform.position, hit.point, zoomVal);
            particle_fx.localScale = Vector3.MoveTowards(particle_fx.localScale, Vector3.one * 0.5f, zoomVal);
            if(objInview == null)
                objInview = hit.collider.transform.root.GetComponent<Collider>();
        }
        else
        {
            Vector3 hitPoint = ray.origin + ray.direction;
            Vector3 nearPoint = GameManager.instance.levelManager.GetNearestPoint(hitPoint);
            transform.position = Vector3.MoveTowards(transform.position, nearPoint, zoomVal);
            if (objInview == null)
                objInview = hit.collider.transform.root.GetComponent<Collider>();
        }

        if (!zoomed)
        {
            zoomed = true;
            EventManager.TriggerEvent(EventNames.RotateStateChange, false);
        }
    }

}
