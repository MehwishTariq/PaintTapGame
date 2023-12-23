using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateColor : MonoBehaviour
{
    public Text txt;
    private void OnEnable()
    {
        ObjectColor.onColored += UpdateColorText;
    }
    private void OnDisable()
    {
        ObjectColor.onColored -= UpdateColorText;
    }

    void UpdateColorText(Color clr)
    {
        string clr1 = ColorUtility.ToHtmlStringRGBA(clr);
        string clr2 = ColorUtility.ToHtmlStringRGBA(GetComponent<Image>().color);
        if (clr1.Equals(clr2))
        {
            UIManager.instance.colorsCount[clr]--;
            txt.text = UIManager.instance.colorsCount[clr].ToString();
            if (UIManager.instance.colorsCount[clr] == 0)
            {
                
                //Add Tick
                gameObject.SetActive(false);
            }
        }
    }
}
