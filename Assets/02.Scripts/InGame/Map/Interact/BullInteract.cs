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
        // ��ũ���� �����ִ��� ���η� ���ϰ��� �ٸ��� ��, �����ִٴ°��� �ʴ����� ���� �ʾҴٴ� ��
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
