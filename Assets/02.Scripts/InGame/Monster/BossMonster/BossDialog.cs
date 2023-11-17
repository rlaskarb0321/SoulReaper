using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IndexingDict
{
    public string _key;
    public List<string> _dialogs;

    public IndexingDict(string key, List<string> dialogs)
    {
        _key = key;
        _dialogs = dialogs;
    }
}

public class BossDialog
{
    public List<IndexingDict> DialogParsing(TextAsset textAsset)
    {
        string situation = "";
        string dialog = "";
        int index = -1;
        string[] lines = textAsset.text.Split('\n');
        List<IndexingDict> dataList = new List<IndexingDict>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(',');
            situation = line[0];
            dialog = line[1];
            dialog = dialog.Trim('\r');

            if (!situation.Equals(""))
            {
                index++;
                IndexingDict data = new IndexingDict(situation, new List<string>());
                dataList.Add(data);
                dataList[index]._dialogs.Add(dialog);
                continue;
            }

            dataList[index]._dialogs.Add(dialog);
        }

        return dataList;
    }

    //public Dictionary<string, List<string>> Parsing(TextAsset textAsset)
    //{
    //    string key = "";
    //    string situation = "";
    //    string dialog = "";
    //    string[] lines = textAsset.text.Split('\n');
    //    Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

    //    for (int i = 1; i < lines.Length; i++)
    //    {
    //        string[] line = lines[i].Split(',');
    //        situation = line[0];
    //        dialog = line[1];
    //        dialog = dialog.Trim('\r');

    //        if (!situation.Equals(""))
    //        {
    //            key = situation;
    //            dict.Add(key, new List<string>());
    //            dict[key].Add(dialog);
    //            continue;
    //        }

    //        dict[key].Add(dialog);
    //    }

    //    return dict;
    //}
}
