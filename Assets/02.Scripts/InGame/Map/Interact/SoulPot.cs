using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SoulPot : MonoBehaviour, IInteractable, IYOrNSelectOption
{
    [Header("=== Interact ===")]
    [SerializeField] private string _interactName;
    [SerializeField] private Transform _floatUIPos;

    private BuffProvider _buffProvider;
    private PlayableDirector _playable;
    private PlayerData _playerData;
    private bool _isInteract;
    private int _selectNum;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
        _buffProvider = GetComponent<BuffProvider>();
    }

    public void Interact()
    {
        if (_isInteract)
            return;

        _isInteract = true;
        ProductionMgr.StartProduction(_playable);
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatGameObjectUI(UIScene._instance._interactUI, value, pos, _interactName);
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
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
    }

    public int ReturnSelectResult()
    {
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
            if (_playerData._soulCount < ConstData.SHRINE_COST)
            {
                _selectNum = -1;
                return;
            }
            else
            {
                _isInteract = false;

                // 버프 주기
                PlayerBuff buff = _buffProvider.GenerateBuffInstance();
                UIScene._instance.UpdateSoulCount(-1 * ConstData.SHRINE_COST);
                UIScene._instance._buffMgr.BuffPlayer(buff);
                return;
            }
        }
    }

    public void CheckAnswer(bool isYes)
    {
        if (isYes)
        {
            _playable.Resume();
        }
        else
        {
            _isInteract = false;
            ProductionMgr.StopProduction(_playable);
        }
    }
}
