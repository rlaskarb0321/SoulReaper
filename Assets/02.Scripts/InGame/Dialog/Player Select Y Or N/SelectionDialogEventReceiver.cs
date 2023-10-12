using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 선택지 대화에서 키고 끌 ui 오브젝트, 글을 입력할 화자text와 대화내용text
/// </summary>
public class SelectionDialogEventReceiver : MonoBehaviour, INotificationReceiver
{
    public DialogUI _selectionUI;
    private PlayableDirector _playable;
    private DialogMgr _dialogMgr;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
        _dialogMgr = new DialogMgr();
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (!notification.id.Equals("Selection"))
            return;

        SelectionDialogMarker marker = notification as SelectionDialogMarker;
        string[] lines = _dialogMgr.ParsingCSVLine(marker._playerSelection);

        PlayDialog(lines);
    }

    private void PlayDialog(string[] lines)
    {
        int selectionIndex = Array.FindIndex(lines, line => line.Contains('#'));
        int selectionCount = lines.Length - (selectionIndex + 1);
        string[] playerLine = lines[selectionIndex].Split(',');

        _playable.Pause();
        _selectionUI._activateUI.SetActive(true);
        _selectionUI._speaker.text = playerLine[0];
        _selectionUI._context.text = _dialogMgr.ReplaceDialogSpecialChar(playerLine[1]);

        for (int i = 0; i < selectionCount; i++)
        {
            DialogSelection selection = _selectionUI._selectionContent.transform.GetChild(i).GetComponent<DialogSelection>();
            string[] line = lines[selectionIndex + (i + 1)].Split(',');
            string speaker = line[0];
            string context = line[1];

            if (!selection.gameObject.activeSelf)
                selection.gameObject.SetActive(true);
            if (i.Equals(0))
                selection._btn.Select();

            selection.RemoveAllListenerSelection();
            selection.InputSelectionData(context);
            selection.AddListenerSelection(() =>
            {
                _playable.Resume();
                _selectionUI._activateUI.SetActive(false);
            });
        }
    }
}
