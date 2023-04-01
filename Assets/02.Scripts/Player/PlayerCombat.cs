using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack")]
    public float _attackAdvancedDist; // ���ݽ� �����Ÿ�

    private Transform _player;
    private Camera _cam;
    private Animator _animator;
    private PlayerAttackBehaviour _atkBehaviour;
    private int _combo;
    private PlayerState _state;

    readonly int _hashCombo = Animator.StringToHash("AttackCombo");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _state = GetComponent<PlayerState>();
        _atkBehaviour = _animator.GetBehaviour<PlayerAttackBehaviour>();
        _cam = Camera.main;
        _player = transform;
        _combo = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && 
            (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move))
        {
            // �ٰŸ� ����
            _state.State = PlayerState.eState.Attack;
            RotateToClickDir();
            _animator.SetInteger(_hashCombo, ++_combo);
        }
        else if (Input.GetMouseButton(1))
        {
            // ���Ÿ�����
            RotateToClickDir();
        }
    }

    void RotateToClickDir()
    {
        RaycastHit hit;
        Vector3 clickVector;
        Vector3 dir;

        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            clickVector = new Vector3(hit.point.x, _player.position.y, hit.point.z);
            dir = clickVector - _player.position;
            transform.forward = dir;
        }
    }

    // Attack �ִϸ��̼� �������� �޾Ƴ��� Delegate
    // �޺��� �� �̾����, ������ ������ �����Ѵ�.
    public void SetComboInteger()
    {
        if (_atkBehaviour._isComboAtk)
        {
            _combo++;
            if (_combo > 2)
                _combo = 0;

            _animator.SetInteger(_hashCombo, _combo);
            _atkBehaviour._isComboAtk = false;
            return;
        }

        _combo = 0;
        _animator.SetInteger(_hashCombo, _combo);
        _state.State = PlayerState.eState.Idle;
    }

    // �޺����ݴܰ��� �������޺��� ������ �����ӿ� �޾Ƴ��� Delegate, �����޺��� �ʱ�ȭ��Ų��.
    public void EndComboAtk()
    {
        _combo = 0;
        _animator.SetInteger(_hashCombo, _combo);
        _state.State = PlayerState.eState.Idle;
    }
}
