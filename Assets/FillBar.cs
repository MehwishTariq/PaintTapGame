using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    Image img;

    public float fillDuration = 5f;
    public float targetFillAmount = 1f;

    private float currentFillAmount = 0f;

    private void Start()
    {
        img = GetComponent<Image>();
        StartCoroutine(FillImageOverTime());
    }
    
    IEnumerator FillImageOverTime()
    {
        float timer = 0f;
        float startAmount = img.fillAmount;

        while (timer < fillDuration)
        {
            timer += Time.deltaTime;
            currentFillAmount = Mathf.Lerp(startAmount, targetFillAmount, timer / fillDuration);
            img.fillAmount = currentFillAmount;
            yield return null; // Wait for the next frame
        }

        // Ensure that the fill amount reaches the target exactly
        img.fillAmount = targetFillAmount;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
