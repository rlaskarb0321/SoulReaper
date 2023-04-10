using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum eState { Idle, Fall, Move, Dodge, Attack, Charging, Dead }
    [SerializeField] private eState _state;
    public eState State { get { return _state; } set { _state = value; } }

    // Component
    Animator _animator;
    Rigidbody _rbody;
    PlayerCombat _combat;
    PlayerMove _mov;
    FallBehaviour _fallBehaviour;

    // Field
    readonly int _hashRoll = Animator.StringToHash("isRoll");
    readonly int _hashCombo = Animator.StringToHash("AttackCombo");
    readonly int _hashDodgeAttack = Animator.StringToHash("DodgeAttack");
    bool _isRoll;
    int _atkCombo;

    // public
    [Header("Combat")]
    public Transform _weaponCombatPos; // 공격할때 무기의 위치값
    public Transform _weaponNonCombatPos; // 공격상태가 아닐때 무기의 위치값
    public GameObject _weapon; // 무기이미지

    void Awake()
    {
        _state = eState.Idle;

        _rbody = GetComponent<Rigidbody>();
        _combat = GetComponent<PlayerCombat>();
        _mov = GetComponent<PlayerMove>();
        _animator = GetComponent<Animator>();
        _fallBehaviour = _animator.GetBehaviour<FallBehaviour>();
    }

    void Start()
    {
        _isRoll = _animator.GetBool(_hashRoll);
        _atkCombo = _animator.GetInteger(_hashCombo);
    }

    void Update()
    {
        if (_fallBehaviour._isFall)
        {
            if (_isRoll)
                _animator.SetBool(_hashRoll, false);
            if (_atkCombo >= 1)
                _animator.SetInteger(_hashCombo, 0);

            _animator.ResetTrigger(_hashDodgeAttack);
            _state = eState.Fall;
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(_combat.ActFallAttack(_rbody, _animator));
            }
        }

        if (_state == eState.Attack)
        {
            _weapon.transform.SetParent(_weaponCombatPos);
            _weapon.transform.localPosition = Vector3.zero;
            _weapon.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            if (_weapon.transform.parent != _weaponNonCombatPos)
            {
                // 무기의 위치가 등이아니라면 등으로 조정
                _weapon.transform.SetParent(_weaponNonCombatPos);
                _weapon.transform.localPosition = Vector3.zero;
                _weapon.transform.localEulerAngles = Vector3.zero;
            }
        }
    }
}
