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


    // 그 외 private 한 변수들
    private string[] _selfTalkLine; // 혼잣말 대화 리스트
    private float _originFloatTime; // 대화 창 띄우고 있을 시간의 원본값
    private float _originDialogCoolTime; // 혼잣말 쿨타임의 원본값
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
