using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AfterYOrNSelectDialogEventReceiver : MonoBehaviour, INotificationReceiver
{
    public DialogUI _dialogUI;

    private PlayableDirector _playable;
    private DialogMgr _dialogMgr;
    private IYOrNSelectOption _yesOrNo;
    private float _letteringSpeed;
    private bool _isEndLine;
    private bool _isEndDialog;
    private float _lineEndTimer = 2.0f;

    private WaitForSeconds _letteringWS;
    private WaitUntil _endLineWU;

    private void Awake()
    {
        _yesOrNo = GetComponent<IYOrNSelectOption>();
        _dialogMgr = new DialogMgr();
        _playable = GetComponent<PlayableDirector>();
        _endLineWU = new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || _lineEndTimer <= 0.0f);
    }

    private void Update()
    {
        if (_isEndLine && !_isEndDialog)
        {
            _lineEndTimer -= Time.deltaTime;
        }
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (!notification.id.Equals("After Y Or N"))
            return;

        AfterYOrNSelectDialogMarker marker = notification as AfterYOrNSelectDialogMarker;
        int selectNum = _yesOrNo.ReturnSelectResult();
        string[] lines = _dialogMgr.ParsingCSVLine(marker._dialogCSVFile[selectNum]);

        _letteringSpeed = marker._letteringSpeed;
        _letteringWS = new WaitForSeconds(_letteringSpeed);
        PlayDialogType(lines);
    }

    private void PlayDialogType(string[] lines)
    {
        _dialogUI._activateUI.gameObject.SetActive(true);
        _playable.Pause();
        StartCoroutine(PlayAfterSelectDialog(lines));
    }

    private IEnumerator PlayAfterSelectDialog(string[] lines)
    {
        int index = 1;
        string speaker = "";
        string context = "";
        StringBuilder letterSb = new StringBuilder();
        int selectNumber;
        _isEndDialog = false;

        // CSV ������ �� ���μ���ŭ �ݺ�
        while (index < lines.Length)
        {
            string[] line = lines[index].Split(','); // ���ε��� �ε�����° ������ ,�� ����
            int letteringIndex = 0;

            speaker = line[0];
            context = _dialogMgr.ReplaceDialogSpecialChar(line[1]);
            letterSb.Clear();
            _isEndLine = false;

            // line ���� ȭ�ڰ� ""�� �ƴϸ� �ٲ���
            if (speaker != null && !speaker.Equals("") && _dialogUI._speaker != null)
            {
                _dialogUI._speaker.text = speaker;
            }

            // ��ȭ���� �� ���ھ� ���
            while (letteringIndex < context.Length)
            {
                // ��ȭ�� ���� �����̽��� �Ǵ� ���콺 ��Ŭ�� �ԷµǸ� �ٷ� �ϼ�, ó�� �� ����� �����ְ��ؾ� �ڿ������� �Ѿ�� �� ����
                if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && letteringIndex > 3)
                {
                    _dialogUI._context.text = context;
                    break;
                }

                letterSb.Append(context[letteringIndex]);
                _dialogUI._context.text = letterSb.ToString();

                letteringIndex++;
                yield return _letteringWS;
            }

            index++;
            _isEndLine = true;

            // �� ������ ������, �����̽��� or ���콺��Ŭ�Է� �Ǵ� �ð��ʰ� �������� ���� ��������
            yield return _endLineWU;
            _lineEndTimer = 2.0f;
        }

        _isEndDialog = true;
        _dialogUI._activateUI.gameObject.SetActive(false);

        // ��ȭ�� ���� ��, y/n ���¿� ���� playable ������ ����
        selectNumber = _yesOrNo.ReturnSelectResult();
        _yesOrNo.CheckAnswer(selectNumber == (int)DialogSelection.eYesOrNo.Yes);
    }
}
