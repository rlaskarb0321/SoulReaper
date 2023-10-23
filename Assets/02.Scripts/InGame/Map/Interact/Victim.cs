using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class Victim : MonoBehaviour, IInteractable, IYOrNSelectOption
{
    [Header("=== Interact ===")]
    [SerializeField] private string _interactName;
    [SerializeField] private Transform _floatUIPos;

    [Header("PlayableAsset by number of not Liberated")]
    [SerializeField] private PlayableAsset[] _playableAssets;
    [SerializeField] private int _noSaveCount;
    [SerializeField] [Tooltip("����� ȥ�㸻 Ʈ���ſ� ���� �ִ� ��ũ��Ʈ")]
    private VictimTalk _victimTalk;

    private PlayableDirector _playableDirector;
    private int _selectNum;
    private bool _isInteract;

    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    public void Interact()
    {
        if (_isInteract)
        {
            return;
        }

        _isInteract = true;
        _playableDirector.playableAsset = _playableAssets[_noSaveCount];
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
        return _selectNum;
    }

    public void ApplyOption(int selectNum)
    {
        _selectNum = selectNum;

        // �������� Y�� ���
        if (selectNum.Equals((int)DialogSelection.eYesOrNo.Yes))
            return;
        
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
            ProductionMgr.StopProduction(_playableDirector);
        }
    }
}
