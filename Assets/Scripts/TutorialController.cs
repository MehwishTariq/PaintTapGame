using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public static TutorialStages TutorialStages;
    public static Action cameraRot;
    public static Action cameraPan;
    public static Action cameraZoomIn;
    public static Action cameraZoomOut;
    public static Action colorSelect;
    public static Action tapOnObj, done;
    public GameObject canvas;
    GameObject tempCanvas;
    public GameObject panning_point, rotation_point, zoom_point, colorSelect_point, tap_point, paintImg;
    public RectTransform tutorialPanel;
    public ScrollRect ColorsScroller;
    public bool Select, Paint;

    private void OnEnable()
    {
        cameraPan += CameraPanningHint;
        cameraRot += CameraRotationHint;
        cameraZoomIn += CameraZoomInHint;
        cameraZoomOut += CameraZoomOutHint;
        colorSelect += ColorSelectHint;
        tapOnObj += TapHint;
        done += Done;    
    
        EventManager.SubscribeToEvent(EventNames.OnPlay, SetTutorialStage);
    }

    void SetTutorialStage()
    {
        if (PlayerPrefs.GetInt(Utility.Tutorial) == 1)
            TutorialStages = TutorialStages.Done;
        else
        {
            TutorialStages = TutorialStages.None;
            tutorialPanel.gameObject.SetActive(true);
        }
    }

    public static void InvokeNextEvent(Action _event)
    {
        _event?.Invoke();
    }

    void CameraPanningHint()
    {
        if (tempCanvas != null)
            Destroy(tempCanvas);

        TutorialStages = TutorialStages.Pan;
        rotation_point.SetActive(false);
        tap_point.SetActive(false);
        panning_point.SetActive(true);
    }

    void CameraRotationHint()
    {
        TutorialStages = TutorialStages.Rotate;
        rotation_point.SetActive(true);
        zoom_point.SetActive(false);
    }

    void CameraZoomInHint()
    {
        TutorialStages = TutorialStages.ZoomIn;
        ZoomUI();
    }

    void ZoomUI()
    {
        tap_point.SetActive(false);
        colorSelect_point.SetActive(false);
        zoom_point.SetActive(true);
    }
    void CameraZoomOutHint()
    {
        TutorialStages = TutorialStages.ZoomOut;
        ZoomUI();

        if (tempCanvas != null)
                Destroy(tempCanvas);

        zoom_point.GetComponent<Animation>().Play("CameraZoomOut");
    }

    GameObject clrBox;

    void ColorSelectHint()
    {
        Transform image = ColorsScroller.content.GetChild(0);
        if(clrBox != null)
            Destroy(clrBox);

        clrBox = Instantiate(image.gameObject, paintImg.transform);
        clrBox.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        clrBox.GetComponent<Button>().onClick = image.GetComponent<Button>().onClick;
        colorSelect_point.SetActive(true);
        panning_point.SetActive(false);
    }

    int index = 0;

    void TapHint()
    {
        tempCanvas = Instantiate(canvas, GameManager.instance.levelManager.objsInlevel[index].transform);
        colorSelect_point.SetActive(false);
        tap_point.SetActive(true);
        zoom_point.SetActive(false);
        index++;
    }

    void Done()
    {
        TutorialStages = TutorialStages.Done;
        tutorialPanel.gameObject.SetActive(false);
        PlayerPrefs.SetInt(Utility.Tutorial, 1);
        rotation_point.SetActive(false);
        EventManager.TriggerEvent(EventNames.OnSaveLevel);
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
