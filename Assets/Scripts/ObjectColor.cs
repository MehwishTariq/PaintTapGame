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

[System.Serializable]
public class ObjectColorSaveData
{
    public int objectId;
    public List<ObjectsData> colors;

    public ObjectColorSaveData(int objectId, List<ObjectsData> colors)
    {
        this.objectId = objectId;
        this.colors = colors;
    }
}

public class ObjectColor : MonoBehaviour
{
    [SerializeField] public List<ObjectsData> objColorsState;
    public bool colored;
    public static Action<string> onColorSelected, onColored;
    public Texture checkbg;
    Renderer rend;
    Collider col;
    public Material grayMat;
    public Material outlineMat;
    int coloredObjects = 0;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
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
                    outlineMat.SetColor("_OutlineColor", MaterialCreator.GetColorFromName(clrName));
                    mats[index] = outlineMat;
                }
                else
                    mats[index] = grayMat;
            }
            index++;
        }
        rend.materials = mats;
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

    public void SetMaterialsFromColors()
    {
        foreach(var obj in objColorsState)
        {
            MaterialCreator.CreateMaterialFromColor(obj.clrName);
        }
    }

    [ContextMenu("GetColor")]
    public void GetColorFromMaterial()
    {
        objColorsState = new List<ObjectsData>();
        Renderer rend = GetComponent<Renderer>();
        Material[] mats = rend.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            Color clr = mats[i].GetColor("_Color");
            objColorsState.Add(new(clr, false));
            MaterialCreator.CreateMaterialFromColor(clr);
        }
        SetLevelGray();
    }

    [ContextMenu("SETORIGINALCOLORS")]
    public void SetOriginalColor()
    {
        Renderer rend = GetComponent<Renderer>();
        Material[] mats = rend.materials;
        int i = 0;
        foreach (var obj in objColorsState)
        {
            if (obj.colored_state)
            {
                MaterialCreator.UpdateCountOfColor(obj.clrName);
                mats[i] = MaterialCreator.GetMaterialFromColor(obj.clrName);
                coloredObjects++;
            }
            else
                mats[i] = grayMat;

            i++;
        }
        if (coloredObjects == objColorsState.Count)
        {
            colored = true;
            LevelManager.checkLevel?.Invoke(colored);
        }
        rend.materials = mats;
    }

    public void ShowColoredLevel()
    {
        Renderer rend = GetComponent<Renderer>();
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
        Renderer rend = GetComponent<Renderer>();
        Material[] mats = rend.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = MaterialCreator.GetWhiteColor();
        }
        rend.materials = mats;
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
					if(coloredObjects == objColorsState.Count)
					{
						colored = true;
						LevelManager.checkLevel?.Invoke(colored);
                    }
                    LevelManager.SaveLevelData?.Invoke();
                    break;
                }
            }
            index++;
        }
        rend.materials = mats;
        return correctColor;
    }
}
