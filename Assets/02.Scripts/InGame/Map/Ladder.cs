using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    public enum eTriggerPos { Up, Down, None, }
    public eTriggerPos _triggerPos;
    private PlayerMove_1 _player;
    [SerializeField] private Transform[] _triggers;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    Interact();
        //}

        if (_player == null)
            return;
        if (_player._state.State != PlayerFSM.eState.Ladder)
            return;

        if (_player.transform.position.y > _triggers[(int)eTriggerPos.Up].position.y)
        {
            _player.ClimbDown(eTriggerPos.Up);
        }

        if (_player.transform.position.y < _triggers[(int)eTriggerPos.Down].position.y)
        {
            _player.ClimbDown(eTriggerPos.Down);
        }
    }


    public void Interact()
    {
        if (_player._state.State == PlayerFSM.eState.Ladder)
            return;
        if (_player._state.State != PlayerFSM.eState.Idle && _player._state.State != PlayerFSM.eState.Move)
            return;

        Vector3 entryPos = _triggers[(int)_triggerPos].position;

        SetActiveInteractUI(false);
        _player._state.State = PlayerFSM.eState.Ladder;
        _player.transform.forward = transform.forward;
        _player.transform.position = entryPos;
    }

    public void SetActiveInteractUI(bool value)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (_player == null)
            _player = other.GetComponent<PlayerMove_1>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _player = null;
        SetActiveInteractUI(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_player._state.State == PlayerFSM.eState.Ladder)
            return;
        if (Input.GetKey(KeyCode.F))
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }
}
