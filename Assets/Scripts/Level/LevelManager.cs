using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class LevelSaveData
{
    public List<ObjectColorSaveData> allObjects = new List<ObjectColorSaveData>();
}

public class LevelManager : MonoBehaviour
{
    public float TargetLevelTimeInSeconds;
    public List<GameObject> objsInlevel;
    [SerializeField]
    int objsColored = 0;

    float currentTime = 0;
    bool levelStarted;
    public int starGained{get; private set;}

    public void Start()
    {
        LoadLevel();
        GetTotalTaps();
        starGained = 3;
    }

    private void OnEnable()
    {
        EventManager.SubscribeToEvent<bool>(EventNames.OnCheckLevel,CheckLevel);
        EventManager.SubscribeToEvent(EventNames.OnSaveLevel, SaveLevel);
        EventManager.SubscribeToEvent(EventNames.OnResetGame,DeleteAllLevels);

    }

    private void OnDisable()
    {
        EventManager.UnsubscribeFromEvent<bool>(EventNames.OnCheckLevel, CheckLevel);
        EventManager.UnsubscribeFromEvent(EventNames.OnSaveLevel, SaveLevel);
        EventManager.UnsubscribeFromEvent(EventNames.OnResetGame,DeleteAllLevels);

    }

    void Update()
    {
        if(levelStarted)
        {
            currentTime += Time.deltaTime;
        }
    }

    public void GetTotalTaps()
    {
        int count = 0;
        foreach(var obj in objsInlevel)
        {
            count += obj.GetComponent<ObjectColor>().objColorsState.Count;
        }
        TestingManager.Instance.SetText("No of Taps: " + count.ToString());
    }


    void StarsGained()
    {
        for(int i = 1;i < 4 ;i++)
        {
            if(currentTime > TargetLevelTimeInSeconds * i)
            {
                starGained--;
                if(starGained == 0)
                    break;
            }
            else
                break;
        }
    }

    void CheckLevel(bool colored)
    {
        if (colored)
        {
            objsColored++;
        }
        if (objsColored >= objsInlevel.Count)
        {            
            levelStarted = false;
            StarsGained();
            EventManager.TriggerEvent(EventNames.OnComplete);
        }
    }

    public void DeleteAllLevels()
    {
#if !UNITY_WEBGL
        for (int i = 0; i < GameManager.instance.levels.Count; i++)
            SaveLoadManager<LevelSaveData>.Delete("Level" + i);            
#endif
        PlayerPrefs.DeleteAll();
    }
    
    public void SaveLevel()
    {
        if (PlayerPrefs.GetInt(Utility.Tutorial) == 0)
            return;

        LevelSaveData saveData = new LevelSaveData();
        List<ObjectColor> allObjectsColor = new();

        foreach (GameObject x in objsInlevel)
        {
            ObjectColor obj = x.GetComponent<ObjectColor>();
            if (obj != null)
                allObjectsColor.Add(obj);
        }

        int index = 0;
        foreach (var obj in allObjectsColor)
        {
            saveData.allObjects.Add(
                new ObjectColorSaveData(index, obj.objColorsState)
            );
            index++;
        }
        SaveLoadManager<LevelSaveData>.Save(saveData, "Level" + GameManager.Level_No);
    }

    void LoadLevel()
    {
        LevelSaveData loaded = SaveLoadManager<LevelSaveData>.Load("Level" + GameManager.Level_No);
        if (loaded == null)
        {
            foreach (var objData in objsInlevel)
            {
                var obj = objData.GetComponent<ObjectColor>();
                obj.GetColorFromMaterial();
            }
        }
        else
        {
            foreach (var objData in loaded.allObjects)
            {
                var obj = objsInlevel[objData.objectId];
                if (obj != null)
                {
                    var oc = obj.GetComponent<ObjectColor>();
                    oc.objColorsState = objData.colors;
                    oc.SetMaterialsFromColors();
                    oc.SetOriginalColor();                    
                }
            }
        }
        levelStarted = true;
    }
}