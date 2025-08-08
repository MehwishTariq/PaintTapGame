using UnityEditor;
using UnityEngine;

public class CustomTools
{

    [MenuItem("Tools/TutorialDone")]
    public static void TutorialDone()
    {
        PlayerPrefs.SetInt("Tutorial", 1);
    }

}
