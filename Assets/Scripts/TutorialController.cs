using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public static Action cameraRot;
    public static Action cameraZoom;
    public static Action colorSelect;
    public static Action tapOnObj, done;
    public GameObject canvas;
    GameObject tempCanvas;
    public GameObject rotation_point, zoom_point, colorSelect_point, tap_point, paintArea;
    public RectTransform tutorialPanel;
    public ScrollRect ColorsScroller;

    private void OnEnable()
    {
        cameraRot += CameraRotationHint;
        cameraZoom += CameraZoomHint;
        colorSelect += ColorSelectHint;
        tapOnObj += TapHint;
        done += Done;
       
    }

    public static void InvokeNextEvent(Action _event)
    {
        _event?.Invoke();
    }

    void CameraRotationHint()
    {
        rotation_point.SetActive(true);
    }

    void CameraZoomHint()
    {
        rotation_point.SetActive(false);
        zoom_point.SetActive(true);
    }

    void ColorSelectHint()
    {
        Transform image = ColorsScroller.content.GetChild(0);
        GameObject clr = Instantiate(image.gameObject, paintArea.transform);
        clr.transform.position = image.position;
        clr.GetComponent<Button>().onClick = image.GetComponent<Button>().onClick;
        zoom_point.SetActive(false);
        colorSelect_point.SetActive(true);
    }

    void TapHint()
    {
        EventManager.TriggerEvent(EventNames.OnCameraReset);
        tempCanvas = Instantiate(canvas, GameManager.instance.levelManager.objsInlevel[0].transform);
        colorSelect_point.SetActive(false);
        tap_point.SetActive(true);
    }

    void Done()
    {
        tempCanvas.SetActive(false);
        tutorialPanel.gameObject.SetActive(false);
        PlayerPrefs.SetInt(Utility.Tutorial, 1);
        tap_point.SetActive(false);
    }

    private void Update()
    {
        if (tempCanvas != null)
        {
            tempCanvas.transform.LookAt(
            new Vector3(GameManager.instance.cameraRef.transform.position.x, tempCanvas.transform.position.y, GameManager.instance.cameraRef.transform.position.z));
        }
    }
}
