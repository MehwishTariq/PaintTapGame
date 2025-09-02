using UnityEngine;

public enum TutorialStages
{
    None,
    Rotate,
    ZoomIn,
    ZoomOut,
    Pan,
    Paint,
    Done
}


public class Utility : MonoBehaviour
{
    public const string levelPref = "LevelNo";
    public const string Tutorial = "Tutorial";
    public const string Coins = "Coins";
    public const string SaveData = "SaveData";
}
