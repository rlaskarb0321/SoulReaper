using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

// �� ��ũ��Ʈ�� ���� ������ ��Ƽ���� ������ ���ؼ��� ���ѵǾ�����
// ���� �ٸ� a, b, c ������ �Ǵ� �÷��̾��� �Է°� �ޱ� Ȥ�� �ٸ� ��Ȳ���� ��쿡�� �� ��ũ��Ʈ�� Ȯ���ϸ� �ȴ�.
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

    // ��繮 �Ľ�, ��ȭ ���, ���� ��ó��
    private IEnumerator StartDialog(TextAsset text, Text speakerText, Text dialogText)
    {
        string speaker = "";
        string dialog = "";
        string[] lines = text.text.Split('\n'); // CSV ���� �������پ� ����
        int index = 1;
        StringBuilder letterSb = new StringBuilder();
        _isEndDialog = false;

        // CSV ������ �� ��ȭ ���� �� ��ŭ �ݺ�
        while (index < lines.Length)
        {
            if (_dialogText == null)
                break;

            int letteringIndex = 0;
            string[] line = lines[index].Split(','); // ȭ��, ��簡 �����

            speaker = line[0];
            dialog = line[1];
            dialog = _dialogMgr.ReplaceDialogSpecialChar(dialog);
            //dialog = dialog.Replace('*', '\n'); // ��ȹ�ڰ� ��繮�� Ư���� �ǹ̸� ���� ��ȣ�� ������ �װ� �Ľ̰����� �ݿ�
            letterSb.Clear();
            _isEndLine = false;

            // ȭ�ڰ� "" �� �ƴϸ� �ٲ���
            if (speakerText != null && !speaker.Equals(""))
            {
                speakerText.text = speaker;
            }

            // ��ȭ���� �� ���ھ� ���
            while (letteringIndex < dialog.Length)
            {
                // ��ȭ�� ���� �����̽��� �Ǵ� ���콺 ��Ŭ�� �ԷµǸ� �ٷ� �ϼ� �� ������, ó�� �� ����� �����ְ��ؾ� �ڿ������� �Ѿ�� �� ����
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
            if (_dialogMarker._dialogType == DialogMarker.eDialogType.Selection)
            {
                // ���⿡ ������ �߰��� �� �Ҵ��ϴ� �޼��� �ۼ�
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
        int totalLength = _selectionContents.transform.childCount; // ���� ��ũ�Ѻ� �������� �ڽ� ������Ʈ�� �ִ� ��Ȱ��ȭ ������Ʈ�� ����
        string[] lines = text.text.Split('\n');
        int selectionCount = lines.Length - 1; // ��ȭ �������� ����
        int startIndex = 1; // csv ���� ���۶���

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
