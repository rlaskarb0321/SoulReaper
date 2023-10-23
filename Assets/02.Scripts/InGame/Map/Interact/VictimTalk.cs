using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimTalk : MonoBehaviour
{
    [SerializeField] private TextAsset _selfTalkCSV;
    [SerializeField] private bool _isPlayerIn;
    [SerializeField] private string[] _selfTalkLine;
    [SerializeField] private Transform _selfTalkPos;

    private DialogMgr _dialogMgr;

    private void Awake()
    {
        _dialogMgr = new DialogMgr();
    }

    private void Start()
    {
        _selfTalkLine = _dialogMgr.ParsingCSVLine(_selfTalkCSV);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _isPlayerIn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _isPlayerIn = false;
    }
}
