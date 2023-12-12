using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public interface IMultiSelection
{
    /// <summary>
    /// 선택을 묻는 텍스트파일이 여러개일때 특정 인덱스의 텍스트 파일을 고르게 하는 메서드
    /// </summary>
    /// <returns></returns>
    public int DivideQuestion();
}

public class BullInteract : MonoBehaviour, IInteractable, IYOrNSelectOption, IMultiSelection
{
    [Header("=== 상호작용 ===")]
    [SerializeField]
    private string _interactName;

    [SerializeField]
    private Transform _floatUIPos;

    [SerializeField]
    private GameObject _scroll;

    [SerializeField]
    private int _maxEndureCount;

    // Field
    private PlayableDirector _playable;
    private int _selectNum;
    private bool _isInteract;
    private int _noInviteCount;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
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
        UIScene._instance.FloatTextUI(UIScene._instance._interactUI, value, pos, _interactName);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        if (_isInteract)
        {
            SetActiveInteractUI(false);
            return;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
    }

    public int ReturnSelectResult()
    {
        return _selectNum;
    }

    public void ApplyOption(int selectNum)
    {
        // 초대장 없이는 어떤 선택지를 골라도 문지기의 짜증 게이지가 올라간다.
        if (_scroll.activeSelf)
        {
            _selectNum = 1;
            DestroyObnoxious();
            return;
        }

        // 초대장을 가지고도 No 를 선택하면 역시 문지기의 짜증 게이지가 올라간다.
        if (selectNum.Equals((int)DialogSelection.eYesOrNo.No))
        {
            _selectNum = 1;
            DestroyObnoxious();
        }
        // 초대장을 가지고있고 Yes 를 선택한 경우
        else
        {
            _selectNum = selectNum;
        }
    }

    public void CheckAnswer(bool isYes)
    {
        if (!_scroll.activeSelf && isYes)
        {
            _playable.Resume();
            SetActiveInteractUI(false);
            return;
        }

        ProductionMgr.StopProduction(_playable);
        _isInteract = false;
    }

    public int DivideQuestion()
    {
        // 스크롤이 켜져있는지 여부로 리턴값을 다르게 함, 켜져있다는것은 초대장을 받지 않았다는 뜻
        int value = _scroll.activeSelf ? 1 : 0;
        return value;
    }

    private void DestroyObnoxious()
    {
        _noInviteCount++;
        if (_noInviteCount == _maxEndureCount)
        {
            ProductionMgr.StopProduction(_playable);
            gameObject.GetComponentInParent<MonsterType>().ReactDamaged();
        }
    }
}
