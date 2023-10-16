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

// �� ��ũ��Ʈ�� ���� ������ ��Ƽ���� ������ ���ؼ��� ���ѵǾ�����
// ���� �ٸ� a, b, c ������ �Ǵ� �÷��̾��� �Է°� �ޱ� Ȥ�� �ٸ� ��Ȳ���� ��쿡�� �ٸ� ��ũ��Ʈ�� �ٽ� ������.
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
