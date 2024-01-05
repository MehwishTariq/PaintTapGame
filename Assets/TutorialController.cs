using System;
using System.Collections;
using System.Collections.Generic;
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
    RectTransform originalPanel;
    public RectTransform tutorialPanel;

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
        originalPanel = UIManager.instance.area;

        UIManager.instance.area = tutorialPanel;
        tutorialPanel.gameObject.SetActive(true);
        rotation_point.SetActive(true);
    }

    void CameraZoomHint()
    {
        rotation_point.SetActive(false);
        zoom_point.SetActive(true);
    }

    void ColorSelectHint()
    {
        Transform image = UIManager.instance.scrollView.content.transform.GetChild(0);
        GameObject clr = Instantiate(image.gameObject, paintArea.transform);
        clr.transform.position = image.position;
        clr.GetComponent<Button>().onClick = image.GetComponent<Button>().onClick;
        zoom_point.SetActive(false);
        colorSelect_point.SetActive(true);
    }

    void TapHint()
    {
        GameManager.instance.cameraRef.ResetTransform();
        tempCanvas = Instantiate(canvas, GameManager.instance.levelManager.objsInlevel[0].transform);
        colorSelect_point.SetActive(false);
        tap_point.SetActive(true);
    }

    void Done()
    {
        tempCanvas.SetActive(false);
        tutorialPanel.gameObject.SetActive(false);
        UIManager.instance.area = originalPanel;
        PlayerPrefs.SetInt("Tutorial", 1);
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
