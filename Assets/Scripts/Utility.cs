using UnityEngine;

public enum TutorialStages
{
    None = 0,
    Select_1 = 1,
    ZoomIn = 2,
    Paint_1 = 3,
    Pan = 4,
    Select_2 = 5,
    Paint_2 = 6,
    ZoomOut = 7,
    Rotate = 8,
    Done = 9,
}


public class Utility : MonoBehaviour
{
    public const string levelPref = "LevelNo";
    public const string Tutorial = "Tutorial";
    public const string Stars = "Stars";
    public const string SaveData = "SaveData";
}
