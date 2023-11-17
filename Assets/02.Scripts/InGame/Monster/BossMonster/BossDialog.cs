using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDialog
{
    public Dictionary<string, List<string>> Parsing(TextAsset textAsset)
    {
        string key = "";
        string situation = "";
        string dialog = "";
        string[] lines = textAsset.text.Split('\n');
        Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(',');
            situation = line[0];
            dialog = line[1];
            dialog = dialog.Trim('\r');

            if (!situation.Equals(""))
            {
                key = situation;
                dict.Add(key, new List<string>());
                dict[key].Add(dialog);
                continue;
            }

            dict[key].Add(dialog);
        }

        return dict;
    }
}
