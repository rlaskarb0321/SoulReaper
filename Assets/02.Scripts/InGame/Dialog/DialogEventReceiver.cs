using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class DialogEventReceiver : MonoBehaviour, INotificationReceiver
{
    [Header("=== On/Off GameObject ===")]
    [SerializeField] private GameObject[] _dialogUI;
    [SerializeField] private GameObject _nextDialogBtn;

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

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
        _endLineWU = new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || _timer <= 0.0f);
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

        // CSV 파일의 총 대화 라인 수 만큼 반복
        while (index < lines.Length)
        {
            if (_dialogText == null)
                break;

            int letteringIndex = 0;
            string[] line = lines[index].Split(','); // 화자, 대사가 저장됨

            speaker = line[0];
            dialog = line[1];
            dialog = dialog.Replace('*', '\n'); // 기획자가 대사문에 특수한 의미를 갖는 기호를 넣으면 그걸 파싱과정에 반영
            letterSb.Clear();
            _isEndLine = false;

            // 화자가 "" 가 아니면 바꿔줌
            if (speakerText != null && !speaker.Equals(""))
            {
                speakerText.text = speaker;
            }

            // 여기에 Selection 과 Normal 분기점, 대화선택문 OnOff 관련
            // 대화문을 한 글자씩 출력
            while (letteringIndex < dialog.Length)
            {
                // 대화문 도중 스페이스바 또는 마우스 좌클이 입력되면 바로 완성 후 마무리, 처음 몇 마디는 보여주게해야 자연스럽게 넘어가는 듯 보임
                if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && letteringIndex > 2)
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
            StartCoroutine(StartDialog(_dialogMarker._dialogCSV, _speakerText[(int)_dialogMarker._dialogType], _dialogText[(int)_dialogMarker._dialogType]));
        }
    }
}
