using System.Collections;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public float zoomSpeed;
    public float moveSpeed;
    bool zoomed;
    Camera cam;
    Vector3 orignalPos, originalRot;
    bool zoomTutorial, panTutorial, rotationTutorial;
    Vector2 lastTappedVal;
    Level objInview;
    float currentDist, distance, halfDist;
    public float safeDistance = 0.3f;

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
        EventManager.SubscribeToEvent<Level>(EventNames.OnObjectSet, SetObjectInView);
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
        EventManager.UnsubscribeFromEvent<Level>(EventNames.OnObjectSet, SetObjectInView);
    }

    public void ResetTransform()
    {
        transform.position = orignalPos;
        transform.eulerAngles = originalRot;
        zoomed = false;
        EventManager.TriggerEvent(EventNames.RotateStateChange, false);
    }

    void SetObjectInView(Level LevelObject)
    {
        objInview = LevelObject;
        distance = (transform.position - objInview.transform.position).sqrMagnitude;
        halfDist = distance / 2;
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
            Bounds bounds = objInview.LevelObjectBounds;

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
        EventManager.TriggerEvent<SizeData>(EventNames.OnChangeParticleSize, new(zoomVal,false));

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
    

    void TargetZoom(float zoomVal, Ray ray)
    {
        Vector3 targetPos;

        RaycastHit hit;
        // Bit shift the index of the layer (2) to get a bit mask
        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 2.
        // But instead we want to collide against everything except layer 2. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        if (Physics.SphereCast(ray.origin,0.5f, ray.direction, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 desiredPos = Vector3.MoveTowards(transform.position, hit.point, zoomVal);

            float distToHit = Vector3.Distance(desiredPos, hit.point);
            if (distToHit < safeDistance)
            {
                desiredPos = hit.point - ray.direction.normalized * safeDistance;
            }

            targetPos = desiredPos;
        }
        else
        {
            Vector3 hitPoint = ray.origin + ray.direction;
            Transform nearPoint = objInview.GetNearestPoint(hitPoint);
            Vector3 desiredPos = Vector3.MoveTowards(transform.position, nearPoint.position, zoomVal);

            //Clamp: keep some distance from the near point
            float distToPoint = Vector3.Distance(desiredPos, nearPoint.position);

            if (distToPoint < safeDistance)
            {
                Vector3 dir = (desiredPos - nearPoint.position).normalized;
                desiredPos = nearPoint.position + dir * safeDistance;
            }

            targetPos = desiredPos;
        }

        EventManager.TriggerEvent<SizeData>(EventNames.OnChangeParticleSize, new(zoomVal,true));

        transform.position = targetPos;

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
