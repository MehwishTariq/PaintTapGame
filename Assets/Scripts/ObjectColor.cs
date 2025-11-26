using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
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
    Renderer rend;
    Collider col;
    public Material grayMat,grayHighlightMat;
    int coloredObjects = 0;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }
    
    private void OnEnable()
    {
        EventManager.SubscribeToEvent<string>(EventNames.OnColorSelect, HighlightObject);
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeFromEvent<string>(EventNames.OnColorSelect, HighlightObject);
    }

    Tween TweenFresnel(Material mat, string property, float to, float duration)
    {
        return DOTween.To(
            () => mat.GetFloat(property),
            x => mat.SetFloat(property, x),
            to,
            duration
        );
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
                    mats[index] = grayHighlightMat;
                    mats[index].SetFloat("_Intensity", 2f);
                    mats[index].SetColor("_HighlightColor",MaterialCreator.GetColorFromName(MaterialCreator.GetColorName(grayMat.color)));
                    // Kill old tween on this material
                    DOTween.Kill(mats[index]);

                    // Start looping tween
                    TweenFresnel(mats[index], "_Intensity", 5f, 1f)
                        .SetLoops(-1)
                        .SetId(mats[index]);
                }
                else
                {
                    DOTween.Kill(mats[index]);
                    mats[index] = grayMat;
                }
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
            MaterialCreator.CreateMaterialFromColor(obj.clrName,false);
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
            MaterialCreator.CreateMaterialFromColor(clr,false);
            MaterialCreator.CreateMaterialFromColor(clr,true);
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
                mats[i] = MaterialCreator.GetMaterialFromColor(obj.clrName,false);
                coloredObjects++;
                EventManager.TriggerEvent(EventNames.OnColorFill);
            }
            else
                mats[i] = grayMat;

            i++;
        }
        if (coloredObjects == objColorsState.Count)
        {
            colored = true;
            EventManager.TriggerEvent<bool>(EventNames.OnCheckLevel, colored);
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
            mats[i] = MaterialCreator.GetMaterialFromColor(obj.clrName,false);
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

    public bool CheckIfCorrectColor(Vector3 hitPos)
    {
        if (colored)
            return false;

        List<Material> mats = rend.materials.ToList();
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
                    EventManager.TriggerEvent<Vector3>(EventNames.OnChangeParticlePos, hitPos);
                    EventManager.TriggerEvent<string>(EventNames.OnColored, chosenColor);
                    mats[index] = MaterialCreator.GetMaterialFromColor(chosenColor,false);


                    correctColor = true;
					coloredObjects++;
                    EventManager.TriggerEvent(EventNames.OnColorFill);
					if(coloredObjects == objColorsState.Count)
					{
						colored = true;
                        EventManager.TriggerEvent<bool>(EventNames.OnCheckLevel, colored);
                    }
                    EventManager.TriggerEvent(EventNames.OnSaveLevel);
                    break;
                }
            }
            index++;
        }
        rend.materials = mats.ToArray();
        return correctColor;
    }
}
