using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    public List<GameObject> objsInlevel;
    [SerializeField]
    int objsColored = 0;
    public static Action<bool> checkLevel;
    public List<Transform> nearPoints;

    public Vector3 GetNearestPoint(Vector3 touchpos)
    {
        List<float> distances = new List<float>();
        for (int i = 0; i < nearPoints.Count; i++) 
        {
            distances.Add(Vector3.Distance(nearPoints[i].position, touchpos));
        }
        return nearPoints[distances.FindIndex(x => x == distances.Min())].position;
    }

    [ContextMenu("ApplyAllColor")]
    public void ApplyallColor()
    {
        foreach(GameObject r in objsInlevel)
        {
            r.GetComponent<ObjectColor>().SetOriginalColor();
        }
    }
    void CheckLevel(bool colored)
    {
        if(colored)
            objsColored++;

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
    
    public void Start()
    {
        StartCoroutine(FillList());
    }

    private void OnEnable()
    {
        checkLevel += CheckLevel;
    }

    private void OnDisable()
    {
        checkLevel -= CheckLevel;
    }

    IEnumerator FillList()
    {
        foreach (GameObject x in objsInlevel)
        {
            ObjectColor obj = x.GetComponent<ObjectColor>();
            if (obj != null)
            {
                yield return new WaitUntil(()=> obj.objColorsState.Count > 0);
                foreach(var data in obj.objColorsState)
                {
                    if (!data.colored_state)
                    {
                        MaterialCreator.AddColorsToDictionary(data.clrName);
                    }
                    MaterialCreator.CreateMaterialFromColor(data.clrName);
                }

            }
        }
        UIManager.instance.FillColors();
    }
}