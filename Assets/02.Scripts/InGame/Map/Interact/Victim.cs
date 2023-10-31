using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class Victim : MonoBehaviour, IInteractable, IYOrNSelectOption
{
    [Header("=== Interact ===")]
    [SerializeField]
    private string _interactName;

    [SerializeField] 
    private Transform _floatUIPos;

    [Header("=== PlayableAsset by number of not Liberated ===")]
    [SerializeField] 
    private PlayableAsset[] _playableAssets;

    [SerializeField] 
    private int _noSaveCount;

    [SerializeField] 
    [Tooltip("����� ȥ�㸻 Ʈ���ſ� ���� �ִ� ��ũ��Ʈ")]
    private VictimTalk _victimTalk;

    [Header("=== Dialog ===")]
    [SerializeField]
    private Image _hierarchyImg;

    [SerializeField]
    private Image _sourceIgm;

    [SerializeField]
    private TMP_Text _soulPriceText;

    [SerializeField]
    private int _soulPriceValue;

    [Header("=== Data ===")]
    [SerializeField]
    private DataApply _apply;

    private PlayableDirector _playableDirector;
    private int _selectNum;
    [HideInInspector] public bool _isInteract;
    private PlayerData _playerData;

    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
        _victimTalk = GetComponentInChildren<VictimTalk>();
    }

    public void Interact()
    {
        if (_isInteract)
        {
            return;
        }

        _isInteract = true;
        _playableDirector.playableAsset = _playableAssets[_noSaveCount];
        _victimTalk._isInteract = true;
        ProductionMgr.StartProduction(_playableDirector);
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractUI(value, pos, _interactName);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_playerData == null)
            _playerData = other.GetComponent<PlayerData>();
        if (_isInteract)
        {
            SetActiveInteractUI(false);
            return;
        }
        if (Input.GetKey(KeyCode.F))
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }

    private void OnTriggerExit(Collider other)
    {
        SetActiveInteractUI(false);
    }

    public int ReturnSelectResult()
    {
        // �÷��̾ ����� ������ ��ȣ�� ����
        return _selectNum;
    }

    public void ApplyOption(int selectNum)
    {
        // �÷��̾ �� �������� ���� ��ȣ�� ����
        _selectNum = selectNum;

        // �������� Y�� ���
        if (selectNum.Equals((int)DialogSelection.eYesOrNo.Yes))
        {
            // �÷��̾ ���� ��ȥ�� ���� ��
            if (_playerData._soulCount < 100)
            {
                _selectNum = -1;
                return;
            }
            else
            {
                UIScene._instance.UpdateSoulCount(-100.0f);
                return;
            }
        }
        
        if (++_noSaveCount > _playableAssets.Length - 1)
        {
            _noSaveCount = _playableAssets.Length - 1;
        }
    }

    public void CheckAnswer(bool isAnswerYes)
    {
        // Y�� ���� Playable�� �����Ű�� N�� ���� ��ȣ�ۿ��� �����ϵ��� ����� Playable�� �ߴ���
        if (isAnswerYes)
        {
            _playableDirector.Resume();
        }
        else
        {
            _isInteract = false;
            _victimTalk._isInteract = false;
            ProductionMgr.StopProduction(_playableDirector);
        }
    }

    public void EditDataSignal()
    {
        _apply.EditMapData();
    }
}
