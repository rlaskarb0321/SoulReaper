using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    private bool _canClimbLadder;
    private PlayerFSM _playerFSM;
    [SerializeField] Transform _entryTrigger;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (!_canClimbLadder)
            return;
        if (_playerFSM.State == PlayerFSM.eState.Ladder) 
            return;
        if (_playerFSM.State != PlayerFSM.eState.Idle && _playerFSM.State != PlayerFSM.eState.Move)
            return;

        _playerFSM.State = PlayerFSM.eState.Ladder;
        SetActiveInteractUI(false);
        _playerFSM.transform.position = _entryTrigger.position;
        _playerFSM.transform.forward = transform.forward;
    }

    public void SetActiveInteractUI(bool value)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (_playerFSM == null)
            _playerFSM = other.GetComponent<PlayerFSM>();

        SetActiveInteractUI(true);
        _canClimbLadder = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
        _canClimbLadder = false;
    }
}
