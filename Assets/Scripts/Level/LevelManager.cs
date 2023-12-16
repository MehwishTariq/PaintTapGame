using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> objsInlevel;
    [SerializeField]
    int objsColored = 0;
    public static Action<bool> checkLevel;

    void CheckLevel(bool colored)
    {
        if(colored)
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
            if (!UIManager.instance.colorsCount.ContainsKey(x.GetComponent<ObjectColor>().objClr))
            {
                UIManager.instance.colorsCount.Add(x.GetComponent<ObjectColor>().objClr, 1);
            }
            else
            {
                UIManager.instance.colorsCount[x.GetComponent<ObjectColor>().objClr]++;
            }
        }

        UIManager.instance.FillColors();
    }
}