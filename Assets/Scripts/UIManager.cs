using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Dictionary<Color, int> colorsCount;

    public TextMeshProUGUI coins;
    int levelNo = 0;
    public const string levelPref = "LevelNo";
    private void Awake()
    {
        instance = this;
        colorsCount = new Dictionary<Color, int>();
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
        LevelManager.save?.Invoke(GameManager.Level_No);
        
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
        LevelManager.save?.Invoke(GameManager.Level_No);
        Application.Quit();
    }

    public void SetColor(Image img)
    {
        chosenClr = img.color;
        ObjectColor.onColorSelected?.Invoke(chosenClr);
        
    }

    GameObject tempContent;
    public void FillColors()
    {
        if(tempContent !=null)
            Destroy(tempContent);
        tempContent = Instantiate(content, scrollView.viewport);
        scrollView.content = tempContent.GetComponent<RectTransform>();
        foreach (Color x in colorsCount.Keys)
        {
            GameObject y = Instantiate(paintImg, tempContent.transform);
            y.GetComponent<Image>().color = x;
            y.GetComponentInChildren<Text>().text = colorsCount[x].ToString();

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
}
