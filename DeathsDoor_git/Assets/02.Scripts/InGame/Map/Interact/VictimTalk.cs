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


    // Field
    private string[] _selfTalkLine; // ȥ�㸻 ��ȭ ����Ʈ
    private float _originFloatTime; // ��ȭ â ���� ���� �ð��� ������
    private float _originDialogCoolTime; // ȥ�㸻 ��Ÿ���� ������
    private bool _isCoolDown; // ȥ�㸻 ��Ÿ���� �������ִ���
    private RectTransform _rect;

    private void Awake()
    {
        _rect = _text.gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        DialogMgr dialogMgr = new DialogMgr();

        _selfTalkLine = dialogMgr.ParsingCSVLine(_selfTalkCSV);
        _originFloatTime = _dialogFloatTime;
        _originDialogCoolTime = _dialogCoolTime;
    }

    private void Update()
    {
        if (_isInteract)
        {
            _canvas.SetActive(false);
            return;
        }

        // ȥ�㸻 ��ٿ� ��
        if (_isCoolDown)
        {
            CoolDownTalkMySelf();
            MaintainTalkMySelfUI();
        }
    }

    private void CoolDownTalkMySelf()
    {
        if (_dialogCoolTime <= 0.0f)
        {
            _isCoolDown = false;
            _dialogFloatTime = _originFloatTime;
            return;
        }
        _dialogCoolTime -= Time.deltaTime;
    }

    private void MaintainTalkMySelfUI()
    {
        if (_dialogFloatTime <= 0.0f)
        {
            if (_canvas.activeSelf)
                _canvas.SetActive(false);

            _dialogFloatTime = 0.0f;
            return;
        }

        _dialogFloatTime -= Time.deltaTime;
    }

    private void DoTalkMySelf(bool isPlayerOut = false)
    {
        // ��ȣ�ۿ� ��ȭ ���̰ų� ȥ�㸻 ��ٿ� ���϶� ȥ�㸻�� �����ʴ´�.
        if (_isCoolDown)
            return;
        if (_isInteract)
        {
            _canvas.SetActive(false);
            return;
        }
        if (!_isPlayerIn)
            return;

        // �÷��̾ �νĹ����� �����ִ���, ���������� ���� �ٸ� �ε��� ��°�� ȥ�㸻 �ؽ�Ʈ
        int index;
        if (isPlayerOut)
            index = _selfTalkLine.Length - 1;
        else
            index = Random.Range(0, _selfTalkLine.Length - 1);

        // ȥ�㸻 ��ٿ� ����
        _isCoolDown = true;
        _dialogCoolTime = _originDialogCoolTime;
        _dialogFloatTime = _originFloatTime;

        // ȥ�㸻 ���� �����ֱ�
        ShowTalkMySelfUI(_selfTalkLine[index]);
    }

    private void ShowTalkMySelfUI(string context)
    {
        if (_text.text != context)
            _text.text = context;

        _canvas.transform.forward = -Camera.main.transform.forward;
        _canvas.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_isInteract)
            return;

        _isPlayerIn = true;
        DoTalkMySelf();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_isInteract)
            return;

        DoTalkMySelf();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_isInteract)
            return;
        
        if (Random.Range(0, 101) > ConstData.VICTIM_TRIGGER_OUT_TALK_MY_SELF_PERCENTAGE)
        {
            _isPlayerIn = false;
        }
        else
        {
            _isCoolDown = false;
            DoTalkMySelf(true);
            _isPlayerIn = false;
        }
    }
}
