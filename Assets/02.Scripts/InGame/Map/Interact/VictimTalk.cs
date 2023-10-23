using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VictimTalk : MonoBehaviour
{
    [Header("=== Dialog File ===")]
    [SerializeField]
    private TextAsset _selfTalkCSV;


    // ���̷�Ű â���� �����ؾ��ϴ� ������
    [Header("=== Hierarchy ===")]
    [SerializeField]
    private GameObject _canvas;

    [SerializeField]
    private TMP_Text _text;


    // ��ũ��Ʈ�� ������ ���� ������ �ǹ��ϴ� ������
    [Header("=== Condition ===")]
    [Tooltip("�÷��̾ ������� �ν� ������ ���� �ִ��� �Ǵ�")]
    [SerializeField]
    private bool _isPlayerIn;

    [Tooltip("�÷��̾�� ��ȭ�� �ߴ��� �Ǵ�")]
    public bool _isInteract;

    [SerializeField]
    [Tooltip("��ȭ â ���� ���� �ð�")]
    private float _dialogFloatTime;

    [SerializeField]
    [Tooltip("��ȭ â �ٽ� ���µ� �ɸ��� �ð�, �� ȥ�㸻 ��Ÿ��")]
    private float _dialogCoolTime;


    // �� �� private �� ������
    private string[] _selfTalkLine; // ȥ�㸻 ��ȭ ����Ʈ
    private float _originFloatTime; // ��ȭ â ���� ���� �ð��� ������
    private float _originDialogCoolTime; // ȥ�㸻 ��Ÿ���� ������
    private RectTransform _rect;

    private void Start()
    {
        DialogMgr dialogMgr = new DialogMgr();

        _rect = _text.gameObject.GetComponent<RectTransform>();
        _selfTalkLine = dialogMgr.ParsingCSVLine(_selfTalkCSV);
        _originFloatTime = _dialogFloatTime;
        _originDialogCoolTime = _dialogCoolTime;
    }

    private void DoTalkMySelf(string context)
    {
        _text.text = context;
        _canvas.SetActive(true);
        _dialogFloatTime = _originFloatTime;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
        StartCoroutine(FloatDialogUI());
    }

    private IEnumerator FloatDialogUI()
    {
        if (!_isPlayerIn)
            yield break;

        while (_dialogFloatTime > 0.0f)
        {
            _dialogFloatTime -= Time.deltaTime;
            yield return null;
        }

        _canvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_isInteract)
            return;

        int randomIdx = Random.Range(0, _selfTalkLine.Length - 1);

        _isPlayerIn = true;
        _dialogCoolTime = _originDialogCoolTime;
        DoTalkMySelf(_selfTalkLine[randomIdx]);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_isInteract)
            return;

        if (_dialogCoolTime <= 0.0f)
        {
            int randomIdx = Random.Range(0, _selfTalkLine.Length - 1);

            _dialogCoolTime = _originDialogCoolTime;
            DoTalkMySelf(_selfTalkLine[randomIdx]);
            return;
        }
        else
        {
            _dialogCoolTime -= Time.deltaTime;
        }

        _canvas.transform.forward = -Camera.main.transform.forward;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_isInteract)
            return;
        if (_dialogFloatTime < _originFloatTime * 0.3f)
            DoTalkMySelf(_selfTalkLine[_selfTalkLine.Length - 1]);

        _isPlayerIn = false;
        _dialogCoolTime = _originDialogCoolTime;
    }
}
