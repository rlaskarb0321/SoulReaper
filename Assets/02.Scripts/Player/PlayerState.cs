using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum eState { Idle, Fall, Move, Dodge, Attack, Charging, Hit, Dead }
    [SerializeField] private eState _state;
    public eState State { get { return _state; } set { _state = value; } }

    // Component
    Animator _animator;
    Rigidbody _rbody;
    PlayerCombat _combat;
    PlayerMove _mov;

    // Field
    readonly int _hashRoll = Animator.StringToHash("isRoll");
    readonly int _hashCombo = Animator.StringToHash("AttackCombo");
    bool _isRoll;
    int _atkCombo;

    // public
    [Header("Combat")]
    public Transform _weaponCombatPos; // �����Ҷ� ������ ��ġ��
    public Transform _weaponNonCombatPos; // ���ݻ��°� �ƴҶ� ������ ��ġ��
    public GameObject _weapon; // �����̹���

    void Awake()
    {
        _state = eState.Idle;

        _rbody = GetComponent<Rigidbody>();
        _combat = GetComponent<PlayerCombat>();
        _mov = GetComponent<PlayerMove>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _isRoll = _animator.GetBool(_hashRoll);
        _atkCombo = _animator.GetInteger(_hashCombo);
    }

    void Update()
    {
        if (_state == eState.Fall)
        {
            if (_isRoll)
                _animator.SetBool(_hashRoll, false);
            if (_atkCombo >= 1)
                _animator.SetInteger(_hashCombo, 0);
            
            if (Input.GetMouseButtonDown(0))
            {
                _combat.ActFallAttack();
            }
        }


        // �� �������� �𸣰ڴµ� ��� ��ġ�� �̻��ϰԵȴ�.
        if (_state == eState.Attack)
        {
            //_weapon.transform.SetParent(null);
            _weapon.transform.SetParent(_weaponCombatPos);
            _weapon.transform.localPosition = Vector3.zero;
        }
        else
        {
            // ������ ��ġ�� ���̾ƴ϶�� ������ ����
            _weapon.transform.SetParent(_weaponNonCombatPos);
            _weapon.transform.localPosition = Vector3.zero;

        }
    }
}
