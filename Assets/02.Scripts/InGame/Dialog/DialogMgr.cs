using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DialogUI
{
    [Header("Parent Object")]
    public GameObject _activateUI;

    [Header("Dialog")]
    public TMP_Text _speaker;
    public TMP_Text _context;
    public GameObject _selectionContent;
}

// 이 스크립트는 현재 보스의 파티참여 권유에 대해서만 국한되어있음
// 만일 다른 a, b, c 선택지 또는 플레이어의 입력값 받기 혹은 다른 상황같은 경우에는 다른 스크립트를 다시 만들자.
public class DialogMgr
{
    // Party
    public static bool _isPartySelect;
    public static bool _isSelectPartyYes;

    public string[] ParsingCSVLine(TextAsset textAsset)
    {
        return textAsset.text.Split('\n');
    }

    public string ReplaceDialogSpecialChar(string input)
    {
        char[] specialChar = { '#', '*', '&' };
        char[] changeChar = { '\0', '\n', ',' };

        for (int i = 0; i < specialChar.Length; i++)
        {
            if (input.Contains(specialChar[i]))
            {
                input = input.Replace(specialChar[i], changeChar[i]);
            }
        }

        return input;
    }
}
