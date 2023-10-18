using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [Header("=== Interact ===")]
    [SerializeField] private string _interactName;

    public enum eTriggerPos { Up, Down, None, }
    [Header("=== Ladder ===")]
    public eTriggerPos _triggerPos;
    [SerializeField] private PlayerMove_1 _player;
    [SerializeField] private Transform[] _triggers;
    [SerializeField] private Transform[] _entryPos;

    private void Update()
    {
        if (_player == null)
            return;
        if (_player._state.State != PlayerFSM.eState.Ladder)
            return;

        // 위에 배치된 입장 위치의 y값 보다 플레이어의 y값이 높으면 다 올라왔음 판정
        if (_player.transform.position.y > _triggers[(int)eTriggerPos.Up].position.y)
        {
            //print("player : " + _player.transform.position.y + "up : " + _entryPos[(int)eTriggerPos.Up].position.y);
            _player.ClimbDown(eTriggerPos.Up);
        }

        // 아래에 배치된 입장 위치의 y값 보다 플레이어의 y값이 낮으면 다 내려옴 판정
        if (_player.transform.position.y < _triggers[(int)eTriggerPos.Down].position.y)
        {
            _player.ClimbDown(eTriggerPos.Down);
        }
    }

    // 플레이어의 상태값을 Ladder로 바꿔주고, 플레이어의 위치, 방향 조정
    public void Interact()
    {
        if (_player._state.State == PlayerFSM.eState.Ladder)
            return;
        if (_player._state.State != PlayerFSM.eState.Idle && _player._state.State != PlayerFSM.eState.Move)
            return;

        SetActiveInteractUI(false);
        _player._state.State = PlayerFSM.eState.Ladder;
        _player.transform.forward = transform.forward;
        _player.transform.position = _entryPos[(int)_triggerPos].position;
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_entryPos[(int)_triggerPos].position);
        UIScene._instance.FloatInteractUI(value, pos, _interactName);
    }

    // 플레이어를 참조
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (_player == null)
            _player = other.GetComponent<PlayerMove_1>();
    }

    // 플레이어 참조 해제
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _player = null;
        SetActiveInteractUI(false);
    }

    // 상호작용 UI표시, F 키 누르면 상호작용
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_player._state.State == PlayerFSM.eState.Ladder || _player._state.State == PlayerFSM.eState.LadderOut)
            return;
        if (Input.GetKey(KeyCode.F))
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }
}
