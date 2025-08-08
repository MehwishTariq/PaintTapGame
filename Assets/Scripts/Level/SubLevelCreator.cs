using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubLevelCreator : MonoBehaviour
{
    public GameObject LevelObject;
    public 

    void CreateLevels()
    {
        LevelData levelData = LevelObject.GetComponent<LevelData>();

        if (levelData == null)
        {
            Debug.LogError("LevelData component not found on LevelObject.");
            return;
        }

    }
}
