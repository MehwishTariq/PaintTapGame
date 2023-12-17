using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    public List<GameObject> objsInlevel;
    [SerializeField]
    int objsColored = 0;
    public static Action<bool> checkLevel;

    public static Action<int> save;

    [SerializeField]
    List<bool> isColored = new List<bool>();
    
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
        save += SaveGame;
        FillList();
    }

    private void OnDisable()
    {
        checkLevel -= CheckLevel;
        save -= SaveGame;
    }
    void FillList()
    {
        LoadGame(GameManager.Level_No);
        UIManager.instance.colorsCount.Clear();
        foreach (GameObject x in objsInlevel)
        {
            if (!x.GetComponent<ObjectColor>().colored)
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
        }

        UIManager.instance.FillColors();
    }

    void SaveGame(int levelNo)
    {
        isColored.Clear();
        for(int i = 0; i < objsInlevel.Count; i++)
        {
            isColored.Add(objsInlevel[i].GetComponent<ObjectColor>().colored);
        }
        Debug.Log("SAVE DATA");
        //Create new xml file
        XmlSerializer serializer = new XmlSerializer(typeof(List<bool>));            //Create serializer
        FileStream stream = new FileStream(Application.persistentDataPath + "/Level" + levelNo, FileMode.Create); //Create file at this path
        serializer.Serialize(stream, isColored);//Write the data in the xml file
        stream.Close();//Close the stream
    }

    void LoadGame(int levelNo)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<bool>));            //Create serializer
            FileStream stream = new FileStream(Application.persistentDataPath + "/Level" + levelNo, FileMode.Open); //Load file at this path
            if (stream != null)
            {
                isColored.AddRange(serializer.Deserialize(stream) as List<bool>);
                stream.Close();//Close the stream

                for (int i = 0; i < isColored.Count; i++)
                {
                    ObjectColor clr = objsInlevel[i].GetComponent<ObjectColor>();
                    clr.colored = isColored[i];
                    if (clr.colored)
                    {
                        objsInlevel[i].GetComponent<Renderer>().material.SetColor("_Color", clr.objClr);
                        checkLevel?.Invoke(clr.colored);
                    }
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveGame(GameManager.Level_No);
        }
    }
    //private void OnApplicationQuit()
    //{
    //    SaveGame(GameManager.Level_No);
    //}
}