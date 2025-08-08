using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

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
    static List<MaterialData> coloredMaterials = new();
    static Dictionary<string, int> colorsCount = new();
    static Material sourceMat;
    
    public Material SourceMaterial;
    static Material whiteMaterial;

    private void OnEnable()
    {
        sourceMat = SourceMaterial;
        colorsCount.Clear();
        coloredMaterials.Clear();
        whiteMaterial = new Material(sourceMat);
        whiteMaterial.SetColor("_Color", Color.white);
    }

    public static string GetColorName(Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGBA(color);
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
        bool colorFound = ColorUtility.TryParseHtmlString(clrName, out Color clr);
        if (colorFound)
        {
            MaterialData mat = coloredMaterials.Find(mat => mat.ColorName == clrName);
            if (mat != null)
            {
                return mat.Material;
            }
        }

        return null;
    }

    public static void CreateMaterialFromColor(string clrName)
    {
        ColorUtility.TryParseHtmlString(clrName, out Color color);
        int index = coloredMaterials.FindIndex(mat => mat.ColorName == clrName);
        if (index != -1)
        {
            return;
        }

        Material coloredMat = new Material(sourceMat);
        coloredMat.SetColor("_Color", color);
        coloredMat.name = clrName;
        coloredMaterials.Add(new(coloredMat.name,1, coloredMat));
    }
}
