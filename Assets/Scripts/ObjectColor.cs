using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

[System.Serializable]
public class ObjectsData
{
    public string clrName;
    public bool colored_state;

    public ObjectsData(Color clr, bool colored_state)
    {
        this.clrName =  MaterialCreator.GetColorName(clr);
        this.colored_state = colored_state;
    }
}

public class ObjectColor : MonoBehaviour
{
    [SerializeField]public List<ObjectsData> objColorsState;
    public bool colored;
    public static Action<string> onColorSelected, onColored;
    public Texture checkbg;
    Renderer rend;
    Collider col;
    public Material grayMat;
	int coloredObjects = 0;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
        objColorsState = new List<ObjectsData>();
        LoadObjectsState();
    }

    public void LoadObjectsState()
    {
        List<ObjectsData> loadedData = SaveLoadManager<List<ObjectsData>>.Load("Level" + GameManager.Level_No);
        if (loadedData == null)
        {
            GetColorFromMaterial();
            SetLevelGray();
        }
        else
        {
            objColorsState.AddRange(loadedData);
            SetOriginalColor();
        }
    }

    [ContextMenu("SetLevelGray")]
    public void SetLevelGray()
    {
        Renderer rend = GetComponent<Renderer>();
        Material[] mats = rend.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = grayMat;
        }
        rend.materials = mats;
    }


    private void OnEnable()
    {
        onColorSelected += HighlightObject;
    }

    private void OnDisable()
    {
        onColorSelected -= HighlightObject;
    }

    void HighlightObject(string clrName)
    {
        col.enabled = false;
        Material[] mats = rend.materials;

        int index = 0;
        foreach (var data in objColorsState)
        {            
            if (!data.colored_state)
            {
                if (data.clrName.Equals(clrName))
                {
                    col.enabled = true;
                }
            }
            index++;
        }
        rend.materials = mats;
    }

    [ContextMenu("GetCOlor")]
    public void GetColorFromMaterial()
    {
        Renderer rend = GetComponent<Renderer>();
        Material[] mats = rend.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            objColorsState.Add(new(mats[i].GetColor("_Color"), false));
        }
    }

    [ContextMenu("SETORIGINALCOLORS")]
    public void SetOriginalColor()
    {
        Material[] mats = rend.materials;
        int i = 0;
        foreach (var obj in objColorsState)
        {
           mats[i] = MaterialCreator.GetMaterialFromColor(obj.clrName);
           i++;
        }
        rend.materials = mats;
    }

    [ContextMenu("SETALLWHITE")]
    public void SetWhiteColor()
    {
        rend.sharedMaterial = MaterialCreator.GetWhiteColor();
    }

    public bool CheckIfCorrectColor()
    {
        if (colored)
            return false;

        Material[] mats = rend.materials;
        bool correctColor = false;
        int index = 0;
        foreach (var data in objColorsState)
        {
            if (!data.colored_state)
            {
                string chosenColor = MaterialCreator.GetColorName(UIManager.chosenClr);
                if (data.clrName.Equals(chosenColor))
                {
                    data.colored_state = true;
                    col.enabled = false;
                    onColored?.Invoke(chosenColor);
                    mats[index] = MaterialCreator.GetMaterialFromColor(chosenColor);
                    correctColor = true;
					coloredObjects++;
                    SaveLoadManager<List<ObjectsData>>.Save(objColorsState, "Level" + GameManager.Level_No);
					if(coloredObjects == objColorsState.Count)
					{
						colored = true;
						LevelManager.checkLevel?.Invoke(colored);
                    }
					break;
                }
            }
            index++;
        }
        rend.materials = mats;
        return correctColor;
    }
}
