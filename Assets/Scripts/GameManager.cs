using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ZoomCamera cameraRef;
    public List<GameObject> levels = new List<GameObject>();
    public static int Level_No;
    GameObject levelObj;
    [SerializeField]
    ParticleSystem winParticles;
    public LevelManager levelManager { set; get; }
    public Level LevelObject { set; get; }
    [SerializeField]
    float time;

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        EventManager.SubscribeToEvent(EventNames.OnComplete, LevelComplete);
        EventManager.SubscribeToEvent(EventNames.OnPlay, CreateLevel);
        EventManager.SubscribeToEvent(EventNames.OnNextLevel, TurnParticlesOff);

    }

    public void CreateLevel()
    {
        int levelNo = PlayerPrefs.GetInt(Utility.levelPref.ToString(), 1);
        if (levelObj != null)
        {
            MaterialCreator.ClearData();
            Destroy(levelObj);
        }
        Level_No = levelNo;
        levelObj = Instantiate(levels[levelNo - 1], levels[levelNo - 1].transform.position, Quaternion.identity);
        levelObj.SetActive(true);
        cameraRef.gameObject.SetActive(true);
        Invoke(nameof(ResetCam), 0.5f);
        EventManager.TriggerEvent(EventNames.OnOpenLevel);
        LevelObject = levelObj.GetComponent<Level>();
        levelManager = LevelObject.Manager;
        EventManager.TriggerEvent(EventNames.OnObjectSet,LevelObject);
    }

    void ResetCam()
    {
        EventManager.TriggerEvent(EventNames.OnCameraReset);
    }
[ContextMenu("Level Complete")]
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
        EventManager.TriggerEvent<int>(EventNames.OnCompleteUI, levelManager.starGained);
    }

    public void TurnParticlesOff()
    {
        winParticles.gameObject.SetActive(false);
    }
}
