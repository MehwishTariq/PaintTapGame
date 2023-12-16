using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Transform content;
    public GameObject paintImg;
    public static Color chosenClr;
    public RectTransform area;
    public GameObject completePanel, mainMenuPanel, InGamePanel, levelSelectionPanel;
    public List<Color> colorsSet { get; set; }
    public Dictionary<Color, int> colorsCount;

    private void Awake()
    {
        instance = this;
        colorsCount = new Dictionary<Color, int>();
    }

    public void GotoMM()
    {
        InGamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void Play()
    {
        levelSelectionPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public IEnumerator OpenLevel()
    {
        yield return new WaitForSeconds(1f);
        InGamePanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
    }

    public void NextLevel()
    {
        InGamePanel.SetActive(false);
        completePanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
    }


    public void SetColor(Image img)
    {
        chosenClr = img.color;
        ObjectColor.onColorSelected?.Invoke(chosenClr);
    }

    public void FillColors()
    {
        foreach(Color x in colorsCount.Keys)
        {
            GameObject y = Instantiate(paintImg, content);
            y.GetComponent<Image>().color = x;
            y.GetComponentInChildren<Text>().text = colorsCount[x].ToString();

            y.GetComponent<Button>().onClick.RemoveAllListeners();
            y.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetColor(y.GetComponent<Image>());
            });
        }
    }
}
