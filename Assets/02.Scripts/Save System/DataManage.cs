using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataManage
{
    private static string SavePath => Application.persistentDataPath + "/saves/";

    // Save Method
    public static void SaveMData(MapData data, string saveFileName)
    {
        if (!Directory.Exists(SavePath))
        {
            Debug.Log("颇老 货肺 积己");
            Directory.CreateDirectory(SavePath);
        }

        string saveJson = JsonUtility.ToJson(data);
        string saveFilePath = SavePath + saveFileName + ".json";

        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
    }

    public static void SaveCData(CharacterData data, string saveFileName)
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        string saveJson = JsonUtility.ToJson(data);
        string saveFilePath = SavePath + saveFileName + ".json";

        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
    }

    // Load Method
    public static MapData LoadMData(string saveFileName)
    {
        string saveFilePath = SavePath + saveFileName + ".json";

        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("No such saveFile exists");
            return null;
        }

        string saveFile = File.ReadAllText(saveFilePath);
        MapData saveData = JsonUtility.FromJson<MapData>(saveFile);

        return saveData;
    }

    public static CharacterData LoadCData(string saveFileName)
    {
        string saveFilePath = SavePath + saveFileName + ".json";

        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("No such saveFile exists");
            return null;
        }

        string saveFile = File.ReadAllText(saveFilePath);
        CharacterData saveData = JsonUtility.FromJson<CharacterData>(saveFile);

        return saveData;
    }
}
