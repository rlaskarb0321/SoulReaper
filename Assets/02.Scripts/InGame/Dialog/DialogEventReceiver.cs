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

        // CSV ������ �� ��ȭ ���� �� ��ŭ �ݺ�
        while (index < lines.Length)
        {
            int letteringIndex = 0;
            string[] line = lines[index].Split(',');

            speaker = line[0];
            dialog = line[1];
            letterSb.Clear();
            _isEndLine = false;

            // ȭ�ڰ� "" �� �ƴϸ� �ٲ���
            if (!speaker.Equals(""))
            {
                _speakerText.text = speaker;
            }

            // ��ȭ���� �� ���ھ� ���
            while (letteringIndex < dialog.Length)
            {
                // ��ȭ�� ���� �����̽��� �Ǵ� ���콺 ��Ŭ�� �ԷµǸ� �ٷ� �ϼ� �� ������, ó�� �� ����� �����ְ��ؾ� �ڿ������� �Ѿ�� �� ����
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

            // �� ������ ������, (�����̽��� or ���콺��Ŭ) �Է¶Ǵ� �ð��ʰ� �������� ���� ��������
            yield return _endLineWU;
            _timer = 2.0f;
        }

        _isEndDialog = true;
    }
}
