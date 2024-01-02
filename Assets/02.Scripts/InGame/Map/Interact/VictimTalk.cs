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


    // 하이러키 창에서 참조해야하는 변수들
    [Header("=== Hierarchy ===")]
    [SerializeField]
    private GameObject _canvas;

    [SerializeField]
    private TMP_Text _text;


    // 스크립트의 동작을 위한 조건을 의미하는 변수들
    [Header("=== Condition ===")]
    [Tooltip("플레이어가 희생자의 인식 범위에 들어와 있는지 판단")]
    [SerializeField]
    private bool _isPlayerIn;

    [Tooltip("플레이어와 대화를 했는지 판단")]
    public bool _isInteract;

    [SerializeField]
    [Tooltip("대화 창 띄우고 있을 시간")]
    private float _dialogFloatTime;

    [SerializeField]
    [Tooltip("대화 창 다시 띄우는데 걸리는 시간, 즉 혼잣말 쿨타임")]
    private float _dialogCoolTime;


    // Field
    private string[] _selfTalkLine; // 혼잣말 대화 리스트
    private float _originFloatTime; // 대화 창 띄우고 있을 시간의 원본값
    private float _originDialogCoolTime; // 혼잣말 쿨타임의 원본값
    private bool _isCoolDown; // 혼잣말 쿨타임을 식히고있는지
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

        // 혼잣말 쿨다운 중
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
        // 상호작용 대화 중이거나 혼잣말 쿨다운 중일땐 혼잣말을 하지않는다.
        if (_isCoolDown)
            return;
        if (_isInteract)
        {
            _canvas.SetActive(false);
            return;
        }
        if (!_isPlayerIn)
            return;

        // 플레이어가 인식범위에 들어와있는지, 나가는지에 따라 다른 인덱스 번째의 혼잣말 텍스트
        int index;
        if (isPlayerOut)
            index = _selfTalkLine.Length - 1;
        else
            index = Random.Range(0, _selfTalkLine.Length - 1);

        // 혼잣말 쿨다운 시작
        _isCoolDown = true;
        _dialogCoolTime = _originDialogCoolTime;
        _dialogFloatTime = _originFloatTime;

        // 혼잣말 내용 보여주기
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
