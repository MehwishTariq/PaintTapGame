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
    public static List<MaterialData> shaderMaterials = new();
    static Material sourceMat, shaderMat;
    public Material SourceMaterial;
    public Material ShaderMaterial;
    static Material whiteMaterial;

    private void Start()
    {
        sourceMat = SourceMaterial;
        shaderMat = ShaderMaterial;
        whiteMaterial = new Material(sourceMat);
        whiteMaterial.SetColor("_Color", Color.white);
    }

    public static void ClearData()
    {
        coloredMaterials.Clear();
    }

    public static string GetColorName(Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(color);
    }

    public static Color GetColorFromName(string name)
    {
        if (ColorUtility.TryParseHtmlString(name, out var color))
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

    public static Material GetMaterialFromColor(string clrName, bool isShader)
    {
        Color clr = GetColorFromName(clrName);
        if (clr != null)
        {
            MaterialData mat;
            if (isShader)
                mat = shaderMaterials.Find(mat => mat.ColorName == clrName);
            else
                mat = coloredMaterials.Find(mat => mat.ColorName == clrName);

            if (mat != null)
            {
                return mat.Material;
            }
        }

        return null;
    }

    public static void CreateMaterialFromColor(Color clr, bool isShader)
    {
        string colorName = GetColorName(clr);
        CreateMaterialFromColor(colorName, isShader); // call the string version
    }

    public static void CreateMaterialFromColor(string clrName, bool isShader)
    {
        int index = 0;
        if (isShader)
        {
            index = shaderMaterials.FindIndex(mat => mat.ColorName == clrName);
            if (index != -1)
            {
                shaderMaterials[index].ColorCount++;
                return;
            }

            Material shader_Mat = new Material(shaderMat);
            shader_Mat.SetColor("_Color", GetColorFromName(clrName));
            shader_Mat.name = clrName;
            shaderMaterials.Add(new(shader_Mat.name, 1, shader_Mat));
        }
        else
        {
            index = coloredMaterials.FindIndex(mat => mat.ColorName == clrName);
            if (index != -1)
            {
                coloredMaterials[index].ColorCount++;
                return;
            }

            Material coloredMat = new Material(sourceMat);
            coloredMat.SetColor("_Color", GetColorFromName(clrName));
            coloredMat.name = clrName;
            coloredMaterials.Add(new(coloredMat.name, 1, coloredMat));

        }


    }

}
