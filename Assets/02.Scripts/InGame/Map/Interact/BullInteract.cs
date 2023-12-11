using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BullInteract : MonoBehaviour, IInteractable, IYOrNSelectOption
{
    [Header("=== 상호작용 ===")]
    [SerializeField]
    private string _interactName;

    [SerializeField]
    private Transform _floatUIPos;

    [SerializeField]
    private GameObject _scroll;

    // Field
    private PlayableDirector _playable;
    private int _selectNum;
    private bool _isInteract;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        print(ProductionMgr._isPlayingProduction);
    }

    public void Interact()
    {
        if (_isInteract)
            return;

        //print(_scroll.activeSelf);
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
        _selectNum = selectNum;

    }

    public void CheckAnswer(bool isYes)
    {
        _playable.Resume();
        _isInteract = false;

    }
}
