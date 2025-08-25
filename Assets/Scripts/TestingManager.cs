using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestingManager : MonoBehaviour
{
    public static TestingManager Instance;
    public TMP_Text TapsText;

    public void Awake()
    {
        if(Instance == null)
            Instance = this;
        
    }

    public void SetText(string text)
    {
        TapsText.text = text;
    }

}
