using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpdateColor : MonoBehaviour
{
    public TMP_Text txt;
    public Image tick, paintBox,stroke;
    Tween highlightBox;

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
        string clr2 = MaterialCreator.GetColorName(paintBox.color);
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
        DisableHighlights();
    }

    public void ResetState()
    {
        tick.gameObject.SetActive(false);
        txt.gameObject.SetActive(true);
        GetComponent<Button>().interactable = true;        
    }

    public void HighlightBox()
    {
        highlightBox = stroke.DOFade(0.5f,0.4f).SetLoops(-1);
    }

    public void DisableHighlights()
    {
        if(highlightBox != null)
            highlightBox.Kill();

        stroke.color = new Color(stroke.color.r,stroke.color.g, stroke.color.b, 0);
    }
}
