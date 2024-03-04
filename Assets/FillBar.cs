using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    float clocktime = 7;
    Image img;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    private void Update()
    {
        if (clocktime > 0)
        {
            clocktime -= Time.deltaTime;
            img.fillAmount = 7 - clocktime;
        }

        if (AppOpenManager.isadDone)
        {
            AppOpenManager.isadDone = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
