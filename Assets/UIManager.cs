using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Transform content;
    public GameObject paintImg;
    public static Color chosenClr;
    public List<Color> colorsSet { get; set; }

    private void Awake()
    {
        instance = this;
        colorsSet = new List<Color>();
    }

    public void SetColor(Image img)
    {
        chosenClr = img.color;
        ObjectColor.onColorSelected?.Invoke(chosenClr);
    }

    public void FillColors()
    {
        foreach(Color x in colorsSet)
        {
            GameObject y = Instantiate(paintImg, content);
            y.GetComponent<Image>().color = x;

            y.GetComponent<Button>().onClick.RemoveAllListeners();
            y.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetColor(y.GetComponent<Image>());
            });
        }
    }
}
