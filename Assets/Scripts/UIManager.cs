using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public ScrollRect scrollView;
    public GameObject paintImg,content;
    public static Color chosenClr;
    public GameObject completePanel, mainMenuPanel, InGamePanel, levelSelectionPanel,loading, pausePanel;
    public List<Color> colorsSet { get; set; }
    public Image ProgressBar;

    public TextMeshProUGUI CoinText;
    int levelNo = 0;
    bool colorSelectDone;
    static int coloredPercent;

    private void Start()
    {
        AudioManager.Instance.PlayMusic();        
    }

    void OnEnable()
    {
        EventManager.SubscribeToEvent(EventNames.OnComplete, LevelComplete);
        EventManager.SubscribeToEvent(EventNames.OnCompleteUI, Complete);
        EventManager.SubscribeToEvent(EventNames.OnPlay, Play);
        EventManager.SubscribeToEvent(EventNames.OnMainMenu, GotoMM);
        EventManager.SubscribeToEvent(EventNames.OnPauseLevel, Pause);
        EventManager.SubscribeToEvent(EventNames.OnResumeLevel, Resume);
        EventManager.SubscribeToEvent(EventNames.OnRestartLevel, Restart);
        EventManager.SubscribeToEvent(EventNames.OnColorFill, TrackProgress);
        EventManager.SubscribeToEvent(EventNames.OnNextLevel, NextLevel);
        EventManager.SubscribeToEvent(EventNames.OnOpenLevel, () => StartCoroutine(OpenLevel()));
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeFromEvent(EventNames.OnComplete, LevelComplete);
        EventManager.UnsubscribeFromEvent(EventNames.OnCompleteUI, Complete);
        EventManager.UnsubscribeFromEvent(EventNames.OnPlay, Play);
        EventManager.UnsubscribeFromEvent(EventNames.OnMainMenu, GotoMM);
        EventManager.UnsubscribeFromEvent(EventNames.OnPauseLevel, Pause);
        EventManager.UnsubscribeFromEvent(EventNames.OnResumeLevel, Resume);
        EventManager.UnsubscribeFromEvent(EventNames.OnRestartLevel, Restart);
        EventManager.UnsubscribeFromEvent(EventNames.OnColorFill, TrackProgress);
        EventManager.UnsubscribeFromEvent(EventNames.OnNextLevel, NextLevel);
        EventManager.UnsubscribeFromEvent(EventNames.OnOpenLevel, () => StartCoroutine(OpenLevel()));
    }

    void Restart()
    {
        SaveLoadManager<LevelSaveData>.Delete("Level" + GameManager.Level_No);
        EventManager.TriggerEvent(EventNames.OnPlay);
    }

    void Pause()
    {
        pausePanel.SetActive(true);
    }

    void Resume()
    {
        pausePanel.SetActive(false);
    }

    void LevelComplete()
    {
        InGamePanel.SetActive(false);
        pausePanel.SetActive(false);
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayWinSound();
        int coins = PlayerPrefs.GetInt(Utility.Coins, 0);
        CoinText.text = coins.ToString();
        PlayerPrefs.SetInt(Utility.Coins, PlayerPrefs.GetInt(Utility.Coins, 0) + 100);
        int levelsOpened = PlayerPrefs.GetInt(Utility.levelPref, 1);
        if(levelsOpened < GameManager.instance.levels.Count - 1)
            PlayerPrefs.SetInt(Utility.levelPref.ToString(), levelsOpened + 1);
        else
            PlayerPrefs.SetInt(Utility.levelPref.ToString(), 1);

    }

    void TrackProgress()
    {
        coloredPercent++;
        int totalObjs = GameManager.instance.levelManager.objsInlevel.Count;
        ProgressBar.fillAmount = (float)coloredPercent / (float)totalObjs;
    }

    void GotoMM()
    {
        AudioManager.Instance.PlayClick();
        InGamePanel.SetActive(false);
        completePanel.SetActive(false);
        pausePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    void Play()
    {
        loading.SetActive(true);
        pausePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        InGamePanel.SetActive(false);
    }

    IEnumerator OpenLevel()
    {
        coloredPercent = 0;
        ProgressBar.fillAmount = 0;
        yield return new WaitForSeconds(2f);
        FillColors();
        InGamePanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        loading.SetActive(false);

        yield return new WaitForSeconds(1f);
        if (PlayerPrefs.GetInt(Utility.Tutorial, 0) == 0)
            TutorialController.InvokeNextEvent(TutorialController.colorSelect);

    }
    
    void Complete()
    {
        completePanel.SetActive(true);
    }

    void NextLevel()
    {
        AudioManager.Instance.PlayMusic();
        InGamePanel.SetActive(false);
        completePanel.SetActive(false);
        levelNo = PlayerPrefs.GetInt(Utility.levelPref, 1);
        loading.SetActive(true);
        EventManager.TriggerEvent(EventNames.OnPlay);
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
        EventManager.TriggerEvent<string>(EventNames.OnColorSelect, colorName);
    }

    GameObject tempContent;
    public void FillColors()
    {
        if(tempContent !=null)
            Destroy(tempContent);

        tempContent = Instantiate(content, scrollView.viewport);
        scrollView.content = tempContent.GetComponent<RectTransform>();

        List<MaterialData> colorsCount = MaterialCreator.GetColorsDictionary();
        List<UpdateColor> updatepallete = new();
        foreach (MaterialData mat in colorsCount)
        {
            GameObject colorPalette = Instantiate(paintImg, tempContent.transform);
            Color clr = MaterialCreator.GetColorFromName(mat.ColorName);
            colorPalette.GetComponent<Image>().color = clr;
            if(mat.ColorCount == 0)
            {
                updatepallete.Add(colorPalette.GetComponent<UpdateColor>());
            }
            else
                colorPalette.GetComponent<UpdateColor>().txt.text = mat.ColorCount.ToString();

            colorPalette.GetComponent<Button>().onClick.RemoveAllListeners();
            colorPalette.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetColor(colorPalette.GetComponent<Image>());
                if (PlayerPrefs.GetInt(Utility.Tutorial, 0) == 0)
                {
                    if (!colorSelectDone)
                    {
                        colorSelectDone = true;
                        TutorialController.InvokeNextEvent(TutorialController.cameraZoom);
                    }
                    else
                    {
                        TutorialController.InvokeNextEvent(TutorialController.tapOnObj);
                    }
                }
            });
        }
        
        foreach(var colorBox in updatepallete)
        {
            colorBox.SetCompletedColors();
        }
    }

}
