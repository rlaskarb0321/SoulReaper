using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGate : MonoBehaviour, IInteractable
{
    private enum eDoorState { Open, Close }

    [Header("=== Interact ===")]
    private string _interactName;
    [SerializeField] private Transform _floatUIPos;
    [SerializeField] private eDoorState _doorState;

    private Animator _animator;
    private bool _isOperate;
    private readonly int _hashDoorOperate = Animator.StringToHash("DoorOperate");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _doorState = eDoorState.Close;
    }

    public void Interact()
    {
        _isOperate = true;
        _animator.SetTrigger(_hashDoorOperate);
    }

    // ���� �ִϸ��̼� �������� �޾Ƴ��Ƽ� �� ������ �������� �˸�
    public void EndOperating() => _isOperate = false;

    // ���� �ִϸ��̼� ���� �߿� ���� ���¸� �ٲ�
    public void StartOperating()
    {
        int index = (int)_doorState;
        index++;
        index %= 2;
        _doorState = (eDoorState)index;
    }

    public void SetActiveInteractUI(bool value)
    {
        if (value == true)
        {
            switch (_doorState)
            {
                case eDoorState.Open:
                    _interactName = "�ݱ�";
                    break;
                case eDoorState.Close:
                    _interactName = "����";
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
