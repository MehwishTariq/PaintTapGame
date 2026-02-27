using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button PlayButton;
    public Button[] MainMenuButton;
    public Button PauseButton;
    public Button ResumeButton;
    public Button RestartButton;
    public Button ResetCameraButton;
    public Button VolumeButton;
    public Button NextLevelButton;
    public Button PlayAgainButton;
    public Button CloseButton;
    public Button InfoButton;

    private void Start()
    {
        CloseButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
        });
        InfoButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
        });
        PlayButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            EventManager.TriggerEvent(EventNames.OnPlay);
        });
        ResetCameraButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            EventManager.TriggerEvent(EventNames.OnCameraReset);
        });
        foreach (Button btn in MainMenuButton)
        {
            btn.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayClick();
                EventManager.TriggerEvent(EventNames.OnMainMenu);
            });
        }
        PauseButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            EventManager.TriggerEvent(EventNames.OnPauseLevel);
        });
        ResumeButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            EventManager.TriggerEvent(EventNames.OnResumeLevel);
        });
        RestartButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            EventManager.TriggerEvent(EventNames.OnRestartLevel);
        });
        VolumeButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            EventManager.TriggerEvent(EventNames.OnVolumeTrigger);
        });
        NextLevelButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            EventManager.TriggerEvent(EventNames.OnNextLevel);
        });
        PlayAgainButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            PlayerPrefs.SetInt(Utility.levelPref.ToString(), 1);
            EventManager.TriggerEvent(EventNames.OnResetGame);
            NextLevelButton.gameObject.SetActive(true);
        });
    }

    void OnEnable()
    {
        EventManager.SubscribeToEvent(EventNames.OnGameComplete, () =>
        {
            NextLevelButton.gameObject.SetActive(false);
            PlayAgainButton.gameObject.SetActive(true);
            PlayButton.gameObject.SetActive(false);
        });
    }
}
