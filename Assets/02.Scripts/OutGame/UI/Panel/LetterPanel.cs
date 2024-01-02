using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class LetterPanel : UIPanel
{
    [SerializeField]
    private TMP_Text _titleText;

    [SerializeField]
    private TMP_Text _contentText;

    public void SetText(string[] text)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 1; i < text.Length; i++)
        {
            sb.AppendLine(text[i]);
        }
        _contentText.text = sb.ToString();
        _titleText.text = text[0];
    }

    public override void PlaySetActiveSound(bool isTurnOn, AudioSource audio)
    {
        base.PlaySetActiveSound(isTurnOn, audio);
    }
}
