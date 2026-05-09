using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public SDKFunctions Sdk { get; private set;}

    private void Awake()
    {
        instance = this;
        Sdk = new();
    }

    private void OnEnable()
    {
        EventManager.SubscribeToEvent(EventNames.OnComplete, LevelComplete);
        EventManager.SubscribeToEvent(EventNames.OnPlay, CreateLevel);
        EventManager.SubscribeToEvent(EventNames.OnResetGame, DeleteAllLevels);

        int levelsOpened = PlayerPrefs.GetInt(Utility.levelPref, 1);
        if (levelsOpened >= levels.Count)
        {
            EventManager.TriggerEvent(EventNames.OnGameComplete);
        }
    }


    public void DeleteAllLevels()
    {
#if !UNITY_WEBGL
        for (int i = 0; i < GameManager.instance.levels.Count; i++)
            SaveLoadManager<LevelSaveData>.Delete("Level" + i);            
#endif
        PlayerPrefs.DeleteAll();

        if (TutorialController.TutorialStages == TutorialStages.Done)
            PlayerPrefs.SetInt(Utility.Tutorial, 1);

        EventManager.TriggerEvent(EventNames.OnPlay);
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
        EventManager.TriggerEvent(EventNames.OnObjectSet, LevelObject);
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
        float delay = Mathf.Clamp(
            time / levelManager.objsInlevel.Count,
            0.05f,
            0.2f
        );
        for (int i = 0; i < levelManager.objsInlevel.Count; i++)
        {
            levelManager.objsInlevel[i].GetComponent<ObjectColor>().ShowColoredLevel();
            yield return new WaitForSeconds(delay);
        }
        AnimateModelOnComplete();
        winParticles.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        winParticles.gameObject.SetActive(false);

        EventManager.TriggerEvent<int>(EventNames.OnCompleteUI, levelManager.starGained);
    }

    void AnimateModelOnComplete()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(
            levelObj.transform
                .DOLocalRotate(new Vector3(0, 360, 0), 1.2f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
        );

        seq.Append(
            levelObj.transform
                .DOPunchScale(Vector3.one * 0.15f, 0.4f, 6, 0.8f)
        );

        seq.PrependInterval(0.2f);
    }
}
