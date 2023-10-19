using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGate : MonoBehaviour, IInteractable
{
    private enum eDoorState { Open, Close }

    [Header("=== Interact ===")]
    [SerializeField] private GameObject _bossRoomPortal;
    [SerializeField] private Transform _floatUIPos;
    [SerializeField] private eDoorState _doorState;
    private string _interactName;

    private Animator _animator;
    private bool _isOperate;
    private readonly int _hashDoorOperate = Animator.StringToHash("DoorOperate");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _doorState = eDoorState.Close;
    }

    public void CanInteract() => this.GetComponent<BoxCollider>().enabled = true;

    public void Interact()
    {
        _isOperate = true;
        _animator.SetTrigger(_hashDoorOperate);
    }

    // 문의 애니메이션 마지막에 달아놓아서 문 조작이 끝났음을 알림
    public void EndOperating()
    {
        if (_doorState == eDoorState.Open)
            _bossRoomPortal.SetActive(true);

        _isOperate = false;
    }

    // 문의 애니메이션 조작 중에 문의 상태를 바꿈
    public void StartOperating()
    {
        int index = (int)_doorState;
        index++;
        index %= 2;
        _doorState = (eDoorState)index;

        switch (_doorState)
        {
            case eDoorState.Open:
                _bossRoomPortal.SetActive(true);
                break;
            case eDoorState.Close:
                _bossRoomPortal.SetActive(false);
                break;
        }
    }

    public void SetActiveInteractUI(bool value)
    {
        if (value == true)
        {
            switch (_doorState)
            {
                case eDoorState.Open:
                    _interactName = "닫기";
                    break;
                case eDoorState.Close:
                    _interactName = "열기";
                    break;
            }
        }

        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractUI(value, pos, _interactName);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_isOperate)
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
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
    }
}
