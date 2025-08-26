using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class SaveLoadManager<T>
{
    private static string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName + ".json");
    }

    public static void Save(T data, string fileName)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
#if UNITY_WEBGL
            PlayerPrefs.SetString(Utility.SaveData, json);
#else
            File.WriteAllText(GetFilePath(fileName), json);
#endif
#if UNITY_EDITOR
            Debug.Log($"[Save] Saved {typeof(T)} to {GetFilePath(fileName)}");
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Save] Failed to save {fileName}: {ex.Message}");
        }
    }

    public static T Load(string fileName)
    {
        string path = GetFilePath(fileName);
        if (!File.Exists(path))
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[Load] No file found at {path}");
#endif
            return default;
        }

        try
        {
#if UNITY_WEBGL
           string json = PlayerPrefs.GetString(Utility.SaveData);
#else
            string json = File.ReadAllText(path);
#endif
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Load] Failed to load {fileName}: {ex.Message}");
            return default;
        }
    }

    public static bool Exists(string fileName)
    {
        return File.Exists(GetFilePath(fileName));
    }

    public static void Delete(string fileName)
    {
        string path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
#if UNITY_EDITOR
            Debug.Log($"[Delete] Deleted {path}");
#endif
        }
    }

}
