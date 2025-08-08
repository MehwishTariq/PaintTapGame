using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public ScrollRect scrollView;
    public GameObject paintImg,content;
    public static Color chosenClr;
    public RectTransform area;
    public GameObject completePanel, mainMenuPanel, InGamePanel, levelSelectionPanel,loading;
    public List<Color> colorsSet { get; set; }

    public TextMeshProUGUI coins;
    int levelNo = 0;
    public const string levelPref = "LevelNo";

    public static Action ResetTransforms;

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic();
        
    }
    void OnEnable()
    {
        levelNo = PlayerPrefs.GetInt(levelPref.ToString(), 1);    
    }
    public void GotoMM()
    {
        AudioManager.Instance.PlayClick();
        InGamePanel.SetActive(false);
        completePanel.SetActive(false);
        mainMenuPanel.SetActive(true);        
    }

    public void Play()
    {
        AudioManager.Instance.PlayClick();
        loading.SetActive(true);
        GameManager.instance.CreateLevel(levelNo);
        //levelSelectionPanel.SetActive(true);
        //mainMenuPanel.SetActive(false);
    }

    public IEnumerator OpenLevel()
    {
        yield return new WaitForSeconds(1f);
        InGamePanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        loading.SetActive(false);
        //levelSelectionPanel.SetActive(false);
        if (PlayerPrefs.GetInt("Tutorial", 0) == 0)
            TutorialController.InvokeNextEvent(TutorialController.cameraRot);
    }

    public void NextLevel()
    {
        AudioManager.Instance.PlayMusic();
        AudioManager.Instance.PlayClick();
        InGamePanel.SetActive(false);
        completePanel.SetActive(false);
        GameManager.instance.TurnParticlesOff();
        PlayerPrefs.SetInt(levelPref.ToString(), PlayerPrefs.GetInt(levelPref, 1) + 1);
        levelNo = PlayerPrefs.GetInt(levelPref, 1);
        loading.SetActive(true);
        if(levelNo == 5)
        {
            levelNo = 1;
            PlayerPrefs.SetInt(levelPref.ToString(), levelNo);
        }
        GameManager.instance.CreateLevel(levelNo);
        //levelSelectionPanel.SetActive(true);
    }

    public void Quit()
    {
        AudioManager.Instance.PlayClick();
        Application.Quit();
    }

    public void SetColor(Image img)
    {
        chosenClr = img.color;
        string colorName = MaterialCreator.GetColorName(chosenClr);
        ObjectColor.onColorSelected?.Invoke(colorName);
        
    }

    GameObject tempContent;
    public void FillColors()
    {
        if(tempContent !=null)
            Destroy(tempContent);

        tempContent = Instantiate(content, scrollView.viewport);
        scrollView.content = tempContent.GetComponent<RectTransform>();

        List<MaterialData> colorsCount = MaterialCreator.GetColorsDictionary();

        foreach (MaterialData mat in colorsCount)
        {
            GameObject y = Instantiate(paintImg, tempContent.transform);
            if(ColorUtility.TryParseHtmlString(mat.ColorName, out Color clr))
                y.GetComponent<Image>().color = clr;

            y.GetComponentInChildren<Text>().text = mat.ColorCount.ToString();

            y.GetComponent<Button>().onClick.RemoveAllListeners();
            y.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetColor(y.GetComponent<Image>());
                if (PlayerPrefs.GetInt("Tutorial", 0) == 0)
                {
                    TutorialController.InvokeNextEvent(TutorialController.tapOnObj);
                }
            });
        }
    }

    public void ResetObjects()
    {
        AudioManager.Instance.PlayClick();
        ResetTransforms?.Invoke();
    }
}
