using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

// 이 스크립트는 현재 보스의 파티참여 권유에 대해서만 국한되어있음
// 만일 다른 a, b, c 선택지 또는 플레이어의 입력값 받기 혹은 다른 상황같은 경우에는 이 스크립트를 확장하면 된다.
public class DialogEventReceiver : MonoBehaviour, INotificationReceiver
{
    [Header("=== On/Off GameObject ===")]
    [SerializeField] private GameObject[] _dialogUI;
    [SerializeField] private GameObject _nextDialogBtn;

    [Header("=== Selection Dialog ===")]
    [SerializeField] private GameObject _selectionContents;

    [Header("=== Text ===")]
    [SerializeField] private Text[] _speakerText;
    [SerializeField] private Text[] _dialogText;

    private bool _isEndDialog;
    private float _timer = 2.0f;
    private bool _isEndLine;
    private DialogMarker _dialogMarker;
    private WaitUntil _endLineWU;
    private WaitForSeconds _letteringWS;
    private PlayableDirector _playable;
    private DialogMgr _dialogMgr;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
        _endLineWU = new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.F) || _timer <= 0.0f);
        _dialogMgr = new DialogMgr();
    }

    private void Update()
    {
        if (_isEndLine && !_isEndDialog)
        {
            _timer -= Time.deltaTime;
        }
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        DialogMarker dialogMarker = notification as DialogMarker;

        _dialogMarker = dialogMarker;
        _letteringWS = new WaitForSeconds(_dialogMarker._letteringSpeed);
        SetTimelinePlay(false, _dialogMarker._dialogType);
        // StartCoroutine(StartDialog(_dialogMarker._dialogCSV));
    }

    // 대사문 파싱, 대화 출력, 끝난 후처리
    private IEnumerator StartDialog(TextAsset text, Text speakerText, Text dialogText)
    {
        string speaker = "";
        string dialog = "";
        string[] lines = text.text.Split('\n'); // CSV 파일 한줄한줄씩 저장
        int index = 1;
        StringBuilder letterSb = new StringBuilder();
        _isEndDialog = false;

        // CSV 파일의 총 대화 라인 수 만큼 반복
        while (index < lines.Length)
        {
            if (_dialogText == null)
                break;

            int letteringIndex = 0;
            string[] line = lines[index].Split(','); // 화자, 대사가 저장됨

            speaker = line[0];
            dialog = line[1];
            dialog = _dialogMgr.ReplaceDialogSpecialChar(dialog);
            //dialog = dialog.Replace('*', '\n'); // 기획자가 대사문에 특수한 의미를 갖는 기호를 넣으면 그걸 파싱과정에 반영
            letterSb.Clear();
            _isEndLine = false;

            // 화자가 "" 가 아니면 바꿔줌
            if (speakerText != null && !speaker.Equals(""))
            {
                speakerText.text = speaker;
            }

            // 대화문을 한 글자씩 출력
            while (letteringIndex < dialog.Length)
            {
                // 대화문 도중 스페이스바 또는 마우스 좌클이 입력되면 바로 완성 후 마무리, 처음 몇 마디는 보여주게해야 자연스럽게 넘어가는 듯 보임
                if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.F)) && letteringIndex > 2)
                {
                    dialogText.text = dialog;
                    break;
                }

                letterSb.Append(dialog[letteringIndex]);
                dialogText.text = letterSb.ToString();

                letteringIndex++;
                yield return _letteringWS;
            }

            index++;
            _isEndLine = true;

            // 한 라인이 끝나고, (스페이스바 or 마우스좌클) 입력또는 시간초가 지나가면 다음 라인으로
            yield return _endLineWU;
            _timer = 2.0f;
        }

        // 대화가 끝난 후 처리
        _isEndDialog = true;
        SetTimelinePlay(true, _dialogMarker._dialogType);
    }

    // 대화문 UI의 On/Off (플레이어 대화문, 적의 대화문), 씨네머신 재생
    private void SetTimelinePlay(bool isSetGo, DialogMarker.eDialogType dialogType)
    {
        // 관련 UI On/Off
        switch (dialogType)
        {
            case DialogMarker.eDialogType.Normal:
                _dialogUI[(int)DialogMarker.eDialogType.Normal].SetActive(!isSetGo);
                break;

            case DialogMarker.eDialogType.Selection:
                _dialogUI[(int)DialogMarker.eDialogType.Selection].SetActive(!isSetGo);
                break;

            case DialogMarker.eDialogType.Victim:
                _dialogUI[(int)DialogMarker.eDialogType.Victim].SetActive(!isSetGo);
                break;
        }

        // 대화가 끝났으니, 멈춰놨던 Cinemachine 재생
        if (isSetGo)
        {
            _playable.Resume();
        }
        // 대화 시작했으니 Cinemachine 을 멈춤
        else
        {
            _playable.Pause();
            if (_dialogMarker._dialogType == DialogMarker.eDialogType.Selection)
            {
                // 여기에 선택지 추가와 값 할당하는 메서드 작성
                ShowSelection(_dialogMarker._dialogCSV_1[0]);
            }
            else
            {
                int index = 0;
                if (DialogMgr._isPartySelect)
                {
                    index = DialogMgr._isSelectPartyYes ? 0 : 1;
                    DialogMgr._isPartySelect = false;
                }

                StartCoroutine(StartDialog(_dialogMarker._dialogCSV_1[index], _speakerText[(int)_dialogMarker._dialogType], _dialogText[(int)_dialogMarker._dialogType]));
            }
        }
    }

    private void ShowSelection(TextAsset text)
    {
        int totalLength = _selectionContents.transform.childCount; // 기존 스크롤뷰 콘텐츠의 자식 오브젝트로 있는 비활성화 오브젝트의 갯수
        string[] lines = text.text.Split('\n');
        int selectionCount = lines.Length - 1; // 대화 선택지의 개수
        int startIndex = 1; // csv 에서 시작라인

        for (int i = startIndex; i < lines.Length; i++)
        {
            //DialogSelection selection = _selectionContents.transform.GetChild(i - 1).GetComponent<DialogSelection>();

            //if (!_selectionContents.transform.GetChild(i - 1).gameObject.activeSelf)
            //    _selectionContents.transform.GetChild(i - 1).gameObject.SetActive(true);
            //if (i == startIndex)
            //    selection._btn.Select();

            //string[] line = lines[i].Split(',');
            //string content = line[1];

            //selection.RemoveAllListenerSelection();
            //selection.AddListenerSelection(() =>
            //{
            //    SetTimelinePlay(true, DialogMarker.eDialogType.Selection);
            //    DialogMgr._isSelectPartyYes = selection._isYes;
            //    DialogMgr._isPartySelect = true;
            //});
            //selection.InputSelectionData(content);
        }
    }
}
