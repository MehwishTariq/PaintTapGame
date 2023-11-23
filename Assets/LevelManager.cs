using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> objsInlevel;
    static int objsColored = 0;
    public static Action checkLevel;

    void CheckLevel()
    {
        objsColored++;
        if (objsColored >= objsInlevel.Count)
        {
            UIManager.instance.completePanel.SetActive(true);
        }
    }

    public void Start()
    {
        checkLevel += CheckLevel;
        FillList();
    }

    private void OnDisable()
    {
        checkLevel -= CheckLevel;
    }
    void FillList()
    {
        foreach(GameObject x in objsInlevel)
        {
            if(!UIManager.instance.colorsSet.Contains(x.GetComponent<ObjectColor>().objClr))
                UIManager.instance.colorsSet.Add(x.GetComponent<ObjectColor>().objClr);
        }

        UIManager.instance.FillColors();
    }
}