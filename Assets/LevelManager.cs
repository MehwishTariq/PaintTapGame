using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> objsInlevel;

    public void Start()
    {
        FillList();
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