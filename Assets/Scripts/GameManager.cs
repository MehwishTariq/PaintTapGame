using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] public RotateCamera cameraRef;
    [SerializeField] List<GameObject> levels = new List<GameObject>();
    public static int Level_No;
    public static Action<int> onGameStart;
    GameObject levelObj;
    [SerializeField]
    ParticleSystem winParticles;
    public LevelManager levelManager { set; get; }
    [SerializeField]
    float time;

    private void Awake()
    {
        instance = this;

    }
    public void CreateLevel(int levelNo)
    {
        AudioManager.Instance.PlayClick();
        if (levelObj != null)
        {
            MaterialCreator.ClearData();
            Destroy(levelObj);
        }
        Level_No = levelNo;
        levelObj = Instantiate(levels[levelNo - 1], levels[levelNo - 1].transform.position, Quaternion.identity);
        levelObj.gameObject.SetActive(true);
        cameraRef.gameObject.SetActive(true);
        Invoke("ResetCam", 0.5f);
        StartCoroutine(UIManager.instance.OpenLevel());
        levelManager = levelObj.GetComponentInChildren<LevelManager>();
    }

    void ResetCam()
    {
        UIManager.ResetTransforms?.Invoke();
    }

    public void LevelComplete()
    {
        foreach (var item in levelManager.objsInlevel)
        {
            item.GetComponent<ObjectColor>().SetWhiteColor();
        }
        StartCoroutine(Complete());

    }
    IEnumerator Complete()
    {
        UIManager.instance.InGamePanel.SetActive(false);
        ResetCam();
        yield return new WaitForSeconds(0.2f);
        float delay = time/ levelManager.objsInlevel.Count;
        for (int i = 0; i < levelManager.objsInlevel.Count; i++)
        {
            levelManager.objsInlevel[i].GetComponent<ObjectColor>().ShowColoredLevel();
            yield return new WaitForSeconds(delay);
        }
        winParticles.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        UIManager.instance.completePanel.SetActive(true);
        EventManager.TriggerEvent(EventNames.OnComplete);
    }

    public void TurnParticlesOff()
    {
        winParticles.gameObject.SetActive(false);
    }
}
