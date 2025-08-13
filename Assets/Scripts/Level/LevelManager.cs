using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LevelSaveData
{
    public List<ObjectColorSaveData> allObjects = new List<ObjectColorSaveData>();
}

public class LevelManager : MonoBehaviour
{
    public List<GameObject> objsInlevel;
    [SerializeField]
    int objsColored = 0;
    public static Action<bool> checkLevel;
    public static Action SaveLevelData;
    public List<Transform> nearPoints;

    public void Start()
    {
        LoadLevel();
    }

    private void OnEnable()
    {
        checkLevel += CheckLevel;
        SaveLevelData += SaveLevel;
        EventManager.SubscribeToEvent(EventNames.OnComplete, Complete);
    }

    private void OnDisable()
    {
        checkLevel -= CheckLevel;
        SaveLevelData -= SaveLevel;
    }

    public void Complete()
    {
        Debug.Log("LEVEL MANAGER COMPLETE");
    }

    public Vector3 GetNearestPoint(Vector3 touchpos)
    {
        List<float> distances = new List<float>();
        for (int i = 0; i < nearPoints.Count; i++) 
        {
            distances.Add(Vector3.Distance(nearPoints[i].position, touchpos));
        }
        return nearPoints[distances.FindIndex(x => x == distances.Min())].position;
    }

    void CheckLevel(bool colored)
    {
        if (colored)
        {
            objsColored++;
        }
        if (objsColored >= objsInlevel.Count)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayWinSound();
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + 100);
            int coins = PlayerPrefs.GetInt("Coins", 0);
            UIManager.instance.coins.text = coins.ToString();
            GameManager.instance.LevelComplete();            
        }
    }
    
    public void SaveLevel()
    {
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

        UIManager.instance.FillColors();
    }
}