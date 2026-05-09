using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public ScrollRect scrollView;
    public GameObject paintImg, content;
    public static Color chosenClr;
    public GameObject completePanel, mainMenuPanel, InGamePanel, levelSelectionPanel, loading, pausePanel;
    public Image ProgressBar;
    public Image[] Stars;
    public TMP_Text Timer;
    public Image LevelRefImage;
    public List<Sprite> LevelRenders;

    UpdateColor selectedColorBox;
    bool colorSelectDone;
    int totalObjs;

    private void Start()
    {
        AudioManager.Instance.PlayMusic();
    }

    void OnEnable()
    {
        EventManager.SubscribeToEvent(EventNames.OnComplete, LevelComplete);
        EventManager.SubscribeToEvent<int>(EventNames.OnCompleteUI, Complete);
        EventManager.SubscribeToEvent(EventNames.OnPlay, Play);
        EventManager.SubscribeToEvent(EventNames.OnMainMenu, GotoMM);
        EventManager.SubscribeToEvent(EventNames.OnPauseLevel, Pause);
        EventManager.SubscribeToEvent(EventNames.OnResumeLevel, Resume);
        EventManager.SubscribeToEvent(EventNames.OnRestartLevel, Restart);
        EventManager.SubscribeToEvent<int>(EventNames.OnColorFill, TrackProgress);
        EventManager.SubscribeToEvent(EventNames.OnNextLevel, NextLevel);
        EventManager.SubscribeToEvent(EventNames.OnOpenLevel, () => StartCoroutine(OpenLevel()));
        EventManager.SubscribeToEvent<float>(EventNames.OnTimeUpdate, SetTime);
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeFromEvent(EventNames.OnComplete, LevelComplete);
        EventManager.UnsubscribeFromEvent<int>(EventNames.OnCompleteUI, Complete);
        EventManager.UnsubscribeFromEvent(EventNames.OnPlay, Play);
        EventManager.UnsubscribeFromEvent(EventNames.OnMainMenu, GotoMM);
        EventManager.UnsubscribeFromEvent(EventNames.OnPauseLevel, Pause);
        EventManager.UnsubscribeFromEvent(EventNames.OnResumeLevel, Resume);
        EventManager.UnsubscribeFromEvent(EventNames.OnRestartLevel, Restart);
        EventManager.UnsubscribeFromEvent<int>(EventNames.OnColorFill, TrackProgress);
        EventManager.UnsubscribeFromEvent(EventNames.OnNextLevel, NextLevel);
        EventManager.UnsubscribeFromEvent(EventNames.OnOpenLevel, () => StartCoroutine(OpenLevel()));
        EventManager.UnsubscribeFromEvent<float>(EventNames.OnTimeUpdate, SetTime);
    }

    void Restart()
    {
        SaveLoadManager<LevelSaveData>.Delete("Level" + GameManager.Level_No);
        EventManager.TriggerEvent(EventNames.OnPlay);
        SetTime(0);
    }

    void Pause()
    {
        pausePanel.SetActive(true);
        GameManager.instance.levelManager.levelStarted = false;

        GameManager.instance.Sdk.GamePlayStopEvent();
    }

    void Resume()
    {
        pausePanel.SetActive(false);
        GameManager.instance.levelManager.levelStarted = true;

        GameManager.instance.Sdk.GamePlayStartEvent();
    }

    void LevelComplete()
    {
        pausePanel.SetActive(false);
        AudioManager.Instance.PlayWinSound();
    }

    IEnumerator AnimateStars(int stars)
    {
        // Reset all stars first
        for (int i = 0; i < Stars.Length; i++)
        {
            Stars[i].DOKill(); // important
            Stars[i].fillAmount = 0;
            Stars[i].transform.localScale = Vector3.zero;
        }

        yield return null; // one frame safety

        for (int i = 0; i < stars && i < Stars.Length; i++)
        {
            var star = Stars[i];
            star.fillAmount = 0;

            star.transform.localScale = Vector3.zero;

            yield return DOTween.Sequence()
                .Append(star.DOFillAmount(1, 0.8f))
                .Join(star.transform.DOScale(1f, 0.8f).SetEase(Ease.OutBack))
                .WaitForCompletion();

            AudioManager.Instance.PlayStarPop();
        }
    }

    void TrackProgress(int objsColored)
    {
        float progress = (float)objsColored / totalObjs;
        ProgressBar.DOFillAmount(progress, 0.3f).SetEase(Ease.InOutSine).SetDelay(0.03f);
    }

    void GotoMM()
    {
        AudioManager.Instance.PlayClick();
        InGamePanel.SetActive(false);
        completePanel.SetActive(false);
        pausePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        GameManager.instance.levelManager.levelStarted = false;

        GameManager.instance.Sdk.GamePlayStopEvent();
    }

    void Play()
    {
        SetTime(0);
        loading.SetActive(true);
        pausePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        InGamePanel.SetActive(false);

        GameManager.instance.Sdk.GamePlayStartEvent();
    }

    IEnumerator OpenLevel()
    {
        ProgressBar.fillAmount = 0;
        LevelRefImage.sprite = LevelRenders[GameManager.Level_No - 1];
        yield return new WaitForSeconds(2f);
        FillColors();
        InGamePanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        loading.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        if (TutorialController.TutorialStages == TutorialStages.None)
        {
            TutorialController.TutorialStages = TutorialStages.Select_1;
            TutorialController.InvokeNextEvent(TutorialController.colorSelect);
        }

        GameManager.instance.levelManager.levelStarted = true;
        totalObjs = GameManager.instance.levelManager.objsInlevel.Count;
    }

    void Complete(int starGained)
    {
        completePanel.SetActive(true);
        int levelsOpened = PlayerPrefs.GetInt(Utility.levelPref, 1);
        if (levelsOpened < GameManager.instance.levels.Count)
            PlayerPrefs.SetInt(Utility.levelPref.ToString(), levelsOpened + 1);
        else
        {
            EventManager.TriggerEvent(EventNames.OnGameComplete);
        }

        StartCoroutine(AnimateStars(starGained));

        GameManager.instance.Sdk.GamePlayStopEvent();
    }

    void NextLevel()
    {
        GameManager.instance.Sdk.GamePlayStartEvent();

        AudioManager.Instance.PlayMusic();
        InGamePanel.SetActive(false);
        completePanel.SetActive(false);
        loading.SetActive(true);
        EventManager.TriggerEvent(EventNames.OnPlay);
        GameManager.instance.levelManager.levelStarted = false;
        //levelSelectionPanel.SetActive(true);
    }

    void SetTime(float currentTime)
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        Timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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


    public void FillColors()
    {
        List<MaterialData> colorsCount = MaterialCreator.GetColorsDictionary();
        List<UpdateColor> updatepallete = new();

        int existingChildren = content.transform.childCount;
        int required = colorsCount.Count;

        // Remove extra child objects if we have more than needed
        for (int i = required; i < existingChildren; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < required; i++)
        {
            GameObject colorPalette;

            // Reuse if child exists
            if (i < existingChildren)
            {
                colorPalette = content.transform.GetChild(i).gameObject;
            }
            else
            {
                // Otherwise instantiate
                colorPalette = Instantiate(paintImg, content.transform);
            }

            MaterialData mat = colorsCount[i];
            Color clr = MaterialCreator.GetColorFromName(mat.ColorName);

            UpdateColor colorBox = colorPalette.GetComponent<UpdateColor>();
            Button colorBtn = colorPalette.GetComponent<Button>();

            colorBox.paintBox.color = clr;
            colorBox.stroke.DOFade(0, 0f);
            colorBox.paintBox.material = MaterialCreator.GetMaterialFromColor(mat.ColorName, true);

            if (mat.ColorCount == 0)
                updatepallete.Add(colorBox);
            else
                colorBox.txt.text = mat.ColorCount.ToString();

            colorBox.ResetState();

            // Set Button behaviour
            colorBtn.onClick.RemoveAllListeners();
            colorBtn.onClick.AddListener(() =>
            {
                if (selectedColorBox != null)
                    selectedColorBox.DisableHighlights();

                selectedColorBox = colorBox;
                SetColor(colorBox.paintBox);
                colorBox.HighlightBox();

                if (TutorialController.TutorialStages == TutorialStages.Select_1)
                {
                    TutorialController.InvokeNextEvent(TutorialController.cameraZoomIn);
                }
                else if (TutorialController.TutorialStages == TutorialStages.Select_2)
                {
                    TutorialController.TutorialStages = TutorialStages.Paint_2;
                    TutorialController.InvokeNextEvent(TutorialController.tapOnObj);
                }
            });
        }

        foreach (var colorBox in updatepallete)
        {
            colorBox.SetCompletedColors();
        }
    }

}
