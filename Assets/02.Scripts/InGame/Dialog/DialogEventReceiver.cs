using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class DialogEventReceiver : MonoBehaviour, INotificationReceiver
{
    [Header("=== On/Off GameObject ===")]
    [SerializeField] private GameObject _dialogUI;
    [SerializeField] private GameObject _nextDialogBtn;

    [Header("=== Text ===")]
    [SerializeField] private Text _speakerText;
    [SerializeField] private Text _dialogText;

    private bool _isEndDialog;
    private float _timer = 2.0f;
    private bool _isEndLine;
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
        TestingMarker dialogMarker = notification as TestingMarker;

        _playable.Pause();
        _letteringWS = new WaitForSeconds(dialogMarker._letteringSpeed);
        _dialogUI.SetActive(true);

        StartCoroutine(StartDialog(dialogMarker._dialogCSV));
    }

    private IEnumerator StartDialog(TextAsset text)
    {
        string speaker = "";
        string dialog = "";
        string[] lines = text.text.Split('\n');
        int index = 1;
        StringBuilder letterSb = new StringBuilder();

        // CSV 파일의 총 대화 라인 수 만큼 반복
        while (index < lines.Length)
        {
            int letteringIndex = 0;
            string[] line = lines[index].Split(',');

            speaker = line[0];
            dialog = line[1];
            letterSb.Clear();
            _isEndLine = false;

            // 화자가 "" 가 아니면 바꿔줌
            if (!speaker.Equals(""))
            {
                _speakerText.text = speaker;
            }

            // 대화문을 한 글자씩 출력
            while (letteringIndex < dialog.Length)
            {
                // 대화문 도중 스페이스바 또는 마우스 좌클이 입력되면 바로 완성 후 마무리, 처음 몇 마디는 보여주게해야 자연스럽게 넘어가는 듯 보임
                if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && letteringIndex > 2)
                {
                    _dialogText.text = dialog;
                    break;
                }

                letterSb.Append(dialog[letteringIndex]);
                _dialogText.text = letterSb.ToString();

                letteringIndex++;
                yield return _letteringWS;
            }

            index++;
            _isEndLine = true;

            // 한 라인이 끝나고, (스페이스바 or 마우스좌클) 입력또는 시간초가 지나가면 다음 라인으로
            yield return _endLineWU;
            _timer = 2.0f;
        }

        _isEndDialog = true;
    }
}
