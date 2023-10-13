using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class Victim : MonoBehaviour, IInteractable, IYOrNSelectOption
{
    // PlayableDirector 의 Timeline 을 여러개 만들어서, _noSaveCount 번쨰 인덱스의 Timeline을 재생시키자
    [SerializeField] private PlayableAsset[] _playableAssets;
    [SerializeField] private PlayableDirector _playableDirector;

    private int _noSaveCount;
    private int _selectNum;
    private bool _isInteract;

    public void Interact()
    {
        if (_isInteract)
            return;

        _isInteract = true;
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
        if (selectNum.Equals(0))
            return;
        
        if (++_noSaveCount > 3)
        {
            _noSaveCount = 3;
        }
    }

    public void EndDialog()
    {
        _isInteract = false;
    }
}
