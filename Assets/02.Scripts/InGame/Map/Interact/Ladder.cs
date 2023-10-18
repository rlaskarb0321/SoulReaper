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

        // ���� ��ġ�� ���� ��ġ�� y�� ���� �÷��̾��� y���� ������ �� �ö���� ����
        if (_player.transform.position.y > _triggers[(int)eTriggerPos.Up].position.y)
        {
            //print("player : " + _player.transform.position.y + "up : " + _entryPos[(int)eTriggerPos.Up].position.y);
            _player.ClimbDown(eTriggerPos.Up);
        }

        // �Ʒ��� ��ġ�� ���� ��ġ�� y�� ���� �÷��̾��� y���� ������ �� ������ ����
        if (_player.transform.position.y < _triggers[(int)eTriggerPos.Down].position.y)
        {
            _player.ClimbDown(eTriggerPos.Down);
        }
    }

    // �÷��̾��� ���°��� Ladder�� �ٲ��ְ�, �÷��̾��� ��ġ, ���� ����
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

    // �÷��̾ ����
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (_player == null)
            _player = other.GetComponent<PlayerMove_1>();
    }

    // �÷��̾� ���� ����
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _player = null;
        SetActiveInteractUI(false);
    }

    // ��ȣ�ۿ� UIǥ��, F Ű ������ ��ȣ�ۿ�
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
