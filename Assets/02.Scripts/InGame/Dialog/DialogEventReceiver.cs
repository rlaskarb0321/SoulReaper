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

    // ��繮 �Ľ�, ��ȭ ���, ���� ��ó��
    private IEnumerator StartDialog(TextAsset text, Text speakerText, Text dialogText)
    {
        string speaker = "";
        string dialog = "";
        string[] lines = text.text.Split('\n'); // CSV ���� �������پ� ����
        int index = 1;
        StringBuilder letterSb = new StringBuilder();

        // CSV ������ �� ��ȭ ���� �� ��ŭ �ݺ�
        while (index < lines.Length)
        {
            if (_dialogText == null)
                break;

            int letteringIndex = 0;
            string[] line = lines[index].Split(','); // ȭ��, ��簡 �����

            speaker = line[0];
            dialog = line[1];
            dialog = dialog.Replace('*', '\n'); // ��ȹ�ڰ� ��繮�� Ư���� �ǹ̸� ���� ��ȣ�� ������ �װ� �Ľ̰����� �ݿ�
            letterSb.Clear();
            _isEndLine = false;

            // ȭ�ڰ� "" �� �ƴϸ� �ٲ���
            if (speakerText != null && !speaker.Equals(""))
            {
                speakerText.text = speaker;
            }

            // ���⿡ Selection �� Normal �б���, ��ȭ���ù� OnOff ����
            // ��ȭ���� �� ���ھ� ���
            while (letteringIndex < dialog.Length)
            {
                // ��ȭ�� ���� �����̽��� �Ǵ� ���콺 ��Ŭ�� �ԷµǸ� �ٷ� �ϼ� �� ������, ó�� �� ����� �����ְ��ؾ� �ڿ������� �Ѿ�� �� ����
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

            // �� ������ ������, (�����̽��� or ���콺��Ŭ) �Է¶Ǵ� �ð��ʰ� �������� ���� ��������
            yield return _endLineWU;
            _timer = 2.0f;
        }

        // ��ȭ�� ���� �� ó��
        _isEndDialog = true;
        SetTimelinePlay(true, _dialogMarker._dialogType);
    }

    // ��ȭ�� UI�� On/Off (�÷��̾� ��ȭ��, ���� ��ȭ��), ���׸ӽ� ���
    private void SetTimelinePlay(bool isSetGo, DialogMarker.eDialogType dialogType)
    {
        // ���� UI On/Off
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

        // ��ȭ�� ��������, ������� Cinemachine ���
        if (isSetGo)
        {
            _playable.Resume();
        }
        // ��ȭ ���������� Cinemachine �� ����
        else
        {
            _playable.Pause();
            StartCoroutine(StartDialog(_dialogMarker._dialogCSV, _speakerText[(int)_dialogMarker._dialogType], _dialogText[(int)_dialogMarker._dialogType]));
        }
    }
}
