using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class YOrNSelectDialogEventReceiver : MonoBehaviour, INotificationReceiver
{
    public DialogUI _selectionUI;

    private PlayableDirector _playable;
    private DialogMgr _dialogMgr;
    private IYOrNSelectOption _selectionResult;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
        _dialogMgr = new DialogMgr();
        _selectionResult = GetComponent<IYOrNSelectOption>();
    }

    // 씨네머신 재생 중, 커스텀 마커를 만나면 실행되는 함수
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (!notification.id.Equals("Selection"))
            return;

        // 마커로부터 대화 csv 파일을 받고, 파싱후 결과값을 메서드로 전달
        YOrNSelectDialogMarker marker = notification as YOrNSelectDialogMarker;
        IMultiSelection multiSelection = gameObject.GetComponent<IMultiSelection>();
        string[] lines;

        if (multiSelection == null)
        {
            lines = _dialogMgr.ParsingCSVLine(marker._playerSelection[0]);
        }
        else
        {
            int textFileIndex = multiSelection.DivideQuestion();
            lines = _dialogMgr.ParsingCSVLine(marker._playerSelection[textFileIndex]);
        }
        PlayDialogType(lines);
    }

    private void PlayDialogType(string[] lines)
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
            selection.InputSelectionData(_dialogMgr.ReplaceDialogSpecialChar(context));
            selection.AddListenerOnClick(() =>
            {
                _selectionResult.ApplyOption(selection._selectionIdx);
                _playable.Resume();
                _selectionUI._activateUI.SetActive(false);
            });
        }
    }
}
