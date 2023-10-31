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
    [Tooltip("희생자 혼잣말 트리거와 같이 있는 스크립트")]
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
        // 플레이어가 골랐던 선택지 번호를 리턴
        return _selectNum;
    }

    public void ApplyOption(int selectNum)
    {
        // 플레이어가 고른 선택지에 대한 번호를 저장
        _selectNum = selectNum;

        // 선택지가 Y일 경우
        if (selectNum.Equals((int)DialogSelection.eYesOrNo.Yes))
        {
            // 플레이어가 가진 영혼의 수를 비교
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
        // Y를 고르면 Playable을 재생시키고 N를 고르면 상호작용이 가능하도록 만들고 Playable을 중단함
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
