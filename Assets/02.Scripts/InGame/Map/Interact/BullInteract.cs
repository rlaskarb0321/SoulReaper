using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public interface IMultiSelection
{
    /// <summary>
    /// ������ ���� �ؽ�Ʈ������ �������϶� Ư�� �ε����� �ؽ�Ʈ ������ ���� �ϴ� �޼���
    /// </summary>
    /// <returns></returns>
    public int DivideQuestion();
}

public class BullInteract : MonoBehaviour, IInteractable, IYOrNSelectOption, IMultiSelection
{
    [Header("=== ��ȣ�ۿ� ===")]
    [SerializeField]
    private string _interactName;

    [SerializeField]
    private Transform _floatUIPos;

    [SerializeField]
    private GameObject _scroll;

    [SerializeField]
    private int _maxEndureCount;

    [Header("=== �� ������ ���� �� ���׸ӽ� ===")]
    [SerializeField]
    private PlayableAsset[] _cinemachines;

    // Field
    private PlayableDirector _playable;
    private int _selectNum;
    private bool _isInteract;
    private int _noInviteCount;
    [HideInInspector] public bool _isGateOpen;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
    }

    public void Interact()
    {
        if (_isInteract)
            return;

        _isInteract = true;

        // ���⼭ �� ���� ���ο� ���� �ٸ� Cinemachine ���
        if (_isGateOpen)
        {
            _playable.playableAsset = _cinemachines[1];
        }
        else
        {
            _playable.playableAsset = _cinemachines[0];
        }
        ProductionMgr.StartProduction(_playable);
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractTextUI(UIScene._instance._interactUI, value, pos, _interactName);
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
        if (Input.GetKey(KeyCode.F))
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
        // �ʴ��� ���̴� � �������� ��� �������� ¥�� �������� �ö󰣴�.
        if (_scroll.activeSelf)
        {
            _selectNum = 1;
            DestroyObnoxious();
            return;
        }

        // �ʴ����� ������ No �� �����ϸ� ���� �������� ¥�� �������� �ö󰣴�.
        if (selectNum.Equals((int)DialogSelection.eYesOrNo.No))
        {
            _selectNum = 1;
            DestroyObnoxious();
        }
        // �ʴ����� �������ְ� Yes �� ������ ���
        else
        {
            _selectNum = selectNum;
        }

        SetActiveInteractUI(false);
    }

    public void CheckAnswer(bool isYes)
    {
        if (!_scroll.activeSelf && isYes)
        {
            _playable.Resume();
            SetActiveInteractUI(false);
            _isInteract = false;
            _isGateOpen = true;
            _noInviteCount = 0;
            return;
        }

        ProductionMgr.StopProduction(_playable);
        _isInteract = false;
    }

    public int DivideQuestion()
    {
        // ��ũ���� �����ִ��� ���η� ���ϰ��� �ٸ��� ��, �����ִٴ°��� �ʴ����� ���� �ʾҴٴ� ��
        int value = _scroll.activeSelf ? 1 : 0;
        return value;
    }

    public void DestroyObnoxious()
    {
        _noInviteCount++;
        _isInteract = false;
        if (_noInviteCount == _maxEndureCount)
        {
            ProductionMgr.StopProduction(_playable);
            gameObject.GetComponentInParent<MonsterType>().ReactDamaged();
        }
    }
}
