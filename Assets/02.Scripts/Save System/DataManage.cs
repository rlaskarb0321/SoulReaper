using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataManage
{
    public static string SavePath => Application.persistentDataPath + "/saves/";

    #region Save Data Method
    public static void SaveMData(MapData data, string saveFileName)
    {
        if (!Directory.Exists(SavePath))
        {
            Debug.Log("弃歹 货肺 积己, M");
            Directory.CreateDirectory(SavePath);
        }

        string saveJson = JsonUtility.ToJson(data);
        string saveFilePath = SavePath + saveFileName + ".json";

        File.WriteAllText(saveFilePath, saveJson);
    }

    public static void SaveCData(CharacterData data, string saveFileName)
    {
        if (!Directory.Exists(SavePath))
        {
            Debug.Log("弃歹 货肺 积己, C");
            Directory.CreateDirectory(SavePath);
        }

        string saveJson = JsonUtility.ToJson(data);
        string saveFilePath = SavePath + saveFileName + ".json";

        File.WriteAllText(saveFilePath, saveJson);
    }

    public static void SaveBData(BuffData data, string saveFileName)
    {
        if (!Directory.Exists(SavePath))
        {
            Debug.Log("弃歹 货肺 积己, B");
            Directory.CreateDirectory(SavePath);
        }

        string saveJson = JsonUtility.ToJson(data);
        string saveFilePath = SavePath + saveFileName + ".json";

        File.WriteAllText(saveFilePath, saveJson);
    }
    #endregion Save Data Method

    #region Load Data Method
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

    public static BuffData LoadBData(string saveFileName)
    {
        string saveFilePath = SavePath + saveFileName + ".json";

        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("No such saveFile exists");
            return null;
        }

        string saveFile = File.ReadAllText(saveFilePath);
        BuffData saveData = JsonUtility.FromJson<BuffData>(saveFile);

        return saveData;
    }
    #endregion Load Data Method
}
