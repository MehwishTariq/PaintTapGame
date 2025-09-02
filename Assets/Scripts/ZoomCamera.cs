using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public float zoomSpeed;
    public float moveSpeed;
    bool zoomed;
    public Transform particle_fx;
    Camera cam;
    Vector3 orignalPos, originalRot;
    bool zoomTutorial, panTutorial, rotationTutorial;
    Vector2 lastTappedVal;
    Collider objInview;

    private void Start()
    {
        cam = GetComponent<Camera>();
        orignalPos = transform.position;
        originalRot = transform.eulerAngles;
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
        zoomed = false;
        EventManager.TriggerEvent(EventNames.RotateStateChange, false);
    }

    void Zoom(float zoomVal, Vector3 mousePos)
    {
        
        
        float zoomAmount = zoomVal * zoomSpeed;

        if (zoomAmount > 0)
        {
            if (TutorialController.TutorialStages != TutorialStages.ZoomIn &&
            TutorialController.TutorialStages < TutorialStages.Done)
                return;

            Ray pos = cam.ScreenPointToRay(mousePos);
            TargetZoom(Mathf.Abs(zoomAmount), pos);
                
        }
        else
        {
            if (TutorialController.TutorialStages != TutorialStages.ZoomOut &&
            TutorialController.TutorialStages < TutorialStages.Done)
                return;

            ZoomOut(Mathf.Abs(zoomAmount));
        }
    }

    void ColorTapTutorial()
    {
        TutorialController.InvokeNextEvent(TutorialController.tapOnObj);
    }
    void ColorSelectTutorial()
    {
        TutorialController.InvokeNextEvent(TutorialController.colorSelect);
    }

    void PanCamera(Vector2 moveVal)
    {
        if (!zoomed ||
            TutorialController.TutorialStages != TutorialStages.Pan &&
            TutorialController.TutorialStages < TutorialStages.Done)
            return;

        
        if (objInview != null)
        {

            Vector2 moveDelta = moveVal - lastTappedVal;

            Vector3 right = transform.right;
            Vector3 up = transform.up;

            Vector3 move = (right * moveDelta.x + up * moveDelta.y) * moveSpeed * Time.deltaTime;
            transform.position -= move;

            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
            Bounds bounds = objInview.bounds;

            if (!GeometryUtility.TestPlanesAABB(planes, bounds))
            {
                transform.position += move;
            }

            if (move != Vector3.zero)
            {
                if (PlayerPrefs.GetInt(Utility.Tutorial, 0) == 0 && !panTutorial)
                {
                    panTutorial = true;
                    Invoke(nameof(ColorSelectTutorial), 1.5f);
                }
            }

        }
        lastTappedVal = moveVal;
    }


    void ZoomOut(float zoomVal)
    {
        transform.position = Vector3.MoveTowards(transform.position, orignalPos, zoomVal);
        particle_fx.localScale = Vector3.MoveTowards(particle_fx.localScale, Vector3.one * 2, zoomVal);

        if (objInview != null)
        {
            currentDist = (transform.position - objInview.transform.position).sqrMagnitude;

            if (zoomed && currentDist > halfDist)
            {
                if (PlayerPrefs.GetInt(Utility.Tutorial, 0) == 0 && !rotationTutorial)
                {
                    rotationTutorial = true;
                    TutorialController.InvokeNextEvent(TutorialController.cameraRot);
                }
                zoomed = false;
                EventManager.TriggerEvent(EventNames.RotateStateChange, false);
            }
        }
    }
    
    float currentDist, distance, halfDist;

    void TargetZoom(float zoomVal, Ray ray)
    {        
        RaycastHit hit;
        // Bit shift the index of the layer (2) to get a bit mask
        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 2.
        // But instead we want to collide against everything except layer 2. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        if (Physics.SphereCast(ray.origin,0.5f, ray.direction, out hit, Mathf.Infinity, layerMask))
        {
            transform.position = Vector3.MoveTowards(transform.position, hit.point, zoomVal);
            particle_fx.localScale = Vector3.MoveTowards(particle_fx.localScale, Vector3.one * 0.5f, zoomVal);
            if (objInview == null)
            {
                objInview = hit.collider.transform.root.GetComponent<Collider>();
                distance = (transform.position - objInview.transform.position).sqrMagnitude;
                halfDist = distance / 2;
            }
        }
        else
        {
            Vector3 hitPoint = ray.origin + ray.direction;
            Transform nearPoint = GameManager.instance.levelManager.GetNearestPoint(hitPoint);
            transform.position = Vector3.MoveTowards(transform.position, nearPoint.position, zoomVal);
            if (objInview == null)
            {
                objInview = nearPoint.root.GetComponent<Collider>();
                distance = (transform.position - objInview.transform.position).sqrMagnitude;
                halfDist = distance / 2;
            }
        }

        currentDist = (transform.position - objInview.transform.position).sqrMagnitude;

        if (!zoomed)
        {
            if (currentDist < halfDist)
            {
                zoomed = true;
                if (PlayerPrefs.GetInt(Utility.Tutorial, 0) == 0 && !zoomTutorial)
                {
                    zoomTutorial = true;
                    Invoke(nameof(ColorTapTutorial), 1.5f);
                }
                EventManager.TriggerEvent(EventNames.RotateStateChange, true);
            }
        }
    }

}
