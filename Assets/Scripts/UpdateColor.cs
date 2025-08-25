using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateColor : MonoBehaviour
{
    public Text txt;
    public Image tick;
    private void OnEnable()
    {
        EventManager.SubscribeToEvent<string>(EventNames.OnColored, UpdateColorText);
    }
    private void OnDisable()
    {
        EventManager.UnsubscribeFromEvent<string>(EventNames.OnColored, UpdateColorText);
    }

    void UpdateColorText(string clr)
    {
        string clr2 = MaterialCreator.GetColorName(GetComponent<Image>().color);
        if (clr.Equals(clr2))
        {
            int clrCount = MaterialCreator.UpdateCountOfColor(clr);
            txt.text = clrCount.ToString();
            if (clrCount == 0)
            {
                SetCompletedColors();
            }
        }
    }

    public void SetCompletedColors()
    {
        tick.gameObject.SetActive(true);
        txt.gameObject.SetActive(false);
        GetComponent<Button>().interactable = false;
        transform.SetSiblingIndex(MaterialCreator.GetColorsDictionary().Count - 1);
    }
}
