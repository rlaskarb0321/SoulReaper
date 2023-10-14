using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class Victim : MonoBehaviour, IInteractable, IYOrNSelectOption
{
    [Header("PlayableAsset by number of not saved")]
    [SerializeField] private PlayableAsset[] _playableAssets;
    [SerializeField] private int _noSaveCount;

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
            return;

        _isInteract = true;
        _playableDirector.playableAsset = _playableAssets[_noSaveCount];
        ProductionMgr.StartProduction(_playableDirector);
    }

    public void SetActiveInteractUI(bool value)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (Input.GetKey(KeyCode.F))
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }

    public int ReturnSelectResult()
    {
        return _selectNum;
    }

    public void ApplyOption(int selectNum)
    {
        _selectNum = selectNum;

        // 선택지가 Y일 경우
        if (selectNum.Equals((int)DialogSelection.eYesOrNo.Yes))
            return;
        
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
            ProductionMgr.StopProduction(_playableDirector);
        }
    }
}
