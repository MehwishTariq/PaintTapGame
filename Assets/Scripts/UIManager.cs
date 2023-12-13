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
    public GameObject completePanel;
    public List<Color> colorsSet { get; set; }
    public Dictionary<Color, int> colorsCount;

    private void Awake()
    {
        instance = this;
        colorsSet = new List<Color>();
        colorsCount = new Dictionary<Color, int>();
    }


    public void Restart()
    {
        SceneManager.LoadScene(0);
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
