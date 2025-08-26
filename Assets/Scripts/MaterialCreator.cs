using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialData
{
    public string ColorName;
    public int ColorCount;
    public Material Material;

    public MaterialData(string colorName, int colorCount, Material material)
    {
        ColorName = colorName;
        ColorCount = colorCount;
        Material = material;
    }
}

public class MaterialCreator : MonoBehaviour
{
    public static List<MaterialData> coloredMaterials = new();
    static Material sourceMat;
    public Material SourceMaterial;
    static Material whiteMaterial;

    private void Start()
    {
        sourceMat = SourceMaterial;
        whiteMaterial = new Material(sourceMat);
        whiteMaterial.SetColor("_BaseColor", Color.white);
    }

    public static void ClearData()
    {
        coloredMaterials.Clear();
    }

    public static string GetColorName(Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGBA(color);
    }

    public static Color GetColorFromName(string name)
    {
        if(ColorUtility.TryParseHtmlString(name, out var color)) 
            return color;

        return Color.black;
    }

    public static Material GetWhiteColor()
    {
        return whiteMaterial;
    }

    public static List<MaterialData> GetColorsDictionary()
    {
        return coloredMaterials;
    }

    public static int GetCountOfColor(string clrName)
    {
        MaterialData mat = coloredMaterials.Find(mat => mat.ColorName == clrName);
        if (mat != null)
        {
            return mat.ColorCount;
        }
        return 0;
    }

    public static int UpdateCountOfColor(string clrName)
    {
        int index = coloredMaterials.FindIndex(mat => mat.ColorName == clrName);
        if (index != -1)
        {
            coloredMaterials[index].ColorCount--;
            return coloredMaterials[index].ColorCount;
        }

        return -1;
    }

    public static void AddColorsToDictionary(string clr)
    {
        int index = coloredMaterials.FindIndex(mat => mat.ColorName == clr);
        if (index != -1)
        {
            coloredMaterials[index].ColorCount++;
        }
    }

    public static Material GetMaterialFromColor(string clrName)
    {
        Color clr = GetColorFromName(clrName);
        if (clr != null)
        {
            MaterialData mat = coloredMaterials.Find(mat => mat.ColorName == clrName);
            if (mat != null)
            {
                return mat.Material;
            }
        }

        return null;
    }

    public static void CreateMaterialFromColor(Color clr)
    {
        string colorName = GetColorName(clr);
        CreateMaterialFromColor(colorName); // call the string version
    }

    public static void CreateMaterialFromColor(string clrName)
    {
        int index = coloredMaterials.FindIndex(mat => mat.ColorName == clrName);
        if (index != -1)
        {
            AddColorsToDictionary(clrName);
            return;
        }

        Material coloredMat = new Material(sourceMat);
        coloredMat.SetColor("_BaseColor", GetColorFromName(clrName));
        coloredMat.name = clrName;
        coloredMaterials.Add(new(coloredMat.name, 1, coloredMat));
        
    }

}
