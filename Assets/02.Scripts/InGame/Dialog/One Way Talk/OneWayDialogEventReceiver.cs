using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// ���͸� ȿ���� �ʿ��� ��ȭ���� ����
/// </summary>
public interface IDialogLetteringEffect
{
    public IEnumerator PlayDialogLettering(string[] lines);
}

public class OneWayDialogEventReceiver : MonoBehaviour, INotificationReceiver, IDialogLetteringEffect
{
    public DialogUI _oneWayDialogUI;
    public GameObject _interactObj;

    private IInteractNPC _interact;
    private PlayableDirector _playable;
    private DialogMgr _dialogMgr;
    private float _letteringSpeed;
    private bool _isEndLine;
    private bool _isEndDialog;
    private float _lineEndTimer = 2.0f;

    private WaitForSeconds _letteringWS;
    private WaitUntil _endLineWU;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
        _dialogMgr = new DialogMgr();
        _endLineWU = new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || _lineEndTimer <= 0.0f);
        if (_interactObj != null)
            _interact = _interactObj.GetComponent<IInteractNPC>();
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
        if (!notification.id.Equals("One Way"))
            return;

        OneWayDialogMarker marker = notification as OneWayDialogMarker;
        string[] lines = _dialogMgr.ParsingCSVLine(marker._oneWayDialog);

        _letteringSpeed = marker._letteringSpeed;
        _letteringWS = new WaitForSeconds(_letteringSpeed);
        PlayDialogType(lines);
    }

    private void PlayDialogType(string[] lines)
    {
        _oneWayDialogUI._activateUI.gameObject.SetActive(true);
        _playable.Pause();
        StartCoroutine(PlayDialogLettering(lines));
    }

    public IEnumerator PlayDialogLettering(string[] lines)
    {
        int index = 1;
        string speaker = "";
        string context = "";
        StringBuilder letterSb = new StringBuilder();
        bool isRichTextMode = false;
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
            if (speaker != null && !speaker.Equals("") && _oneWayDialogUI._speaker != null)
            {
                _oneWayDialogUI._speaker.text = speaker;
            }

            // ��ȭ���� �� ���ھ� ���
            while (letteringIndex < context.Length)
            {
                // ��ȭ�� ���� �����̽��� �Ǵ� ���콺 ��Ŭ�� �ԷµǸ� �ٷ� �ϼ�, ó�� �� ����� �����ְ��ؾ� �ڿ������� �Ѿ�� �� ����
                if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && letteringIndex > 3)
                {
                    context = context.Replace("\\", "");
                    _oneWayDialogUI._context.text = context;
                    break;
                }

                if (isRichTextMode)
                {
                    if (context[letteringIndex].Equals('\\'))
                    {
                        isRichTextMode = false;
                        // print("��ġ �ؽ�Ʈ ��� ��! " + letterSb.ToString());
                        _oneWayDialogUI._context.text = letterSb.ToString();
                        letteringIndex++;
                        yield return _letteringWS;
                        continue;
                    }

                    // print("RichTextMode " + context[letteringIndex] + " �߰�");
                    letterSb.Append(context[letteringIndex]);
                    letteringIndex++;
                    yield return null;
                    continue;
                }

                if (context[letteringIndex].Equals('\\'))
                {
                    if (!isRichTextMode)
                    {
                        // print(" \ �߰�");
                        isRichTextMode = true;
                        letteringIndex++;
                        yield return null;
                        continue;
                    }
                }

                letterSb.Append(context[letteringIndex]);
                _oneWayDialogUI._context.text = letterSb.ToString();
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
        _oneWayDialogUI._activateUI.gameObject.SetActive(false);
        if (_interactObj != null)
            _interact.ResetInteract(); 
        _playable.Resume();
    }
}
