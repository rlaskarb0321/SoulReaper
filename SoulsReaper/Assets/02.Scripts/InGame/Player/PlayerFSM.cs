using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFSM : MonoBehaviour
{
    public enum eState { Idle, Fall, Move, Dodge, Attack, Charging, Hit, Dead, Ladder, LadderOut }
    [Header("=== State ===")]
    [SerializeField] private eState _state;
    public eState State { get { return _state; } set { _state = value; } }

    [Header("=== Combat ===")]
    public float _hitDelay;
    public Transform _weaponCombatPos; // 공격할때 무기의 위치값
    public Transform _weaponNonCombatPos; // 공격상태가 아닐때 무기의 위치값
    public GameObject _weapon; // 무기이미지
    public float _knockBackValue;
    public float _knockBackDelta;
    
    Animator _animator;
    Rigidbody _rbody;
    PlayerMove_1 _move;
    PlayerCombat _combat;
    readonly int _hashRoll = Animator.StringToHash("isRoll");
    readonly int _hashCombo = Animator.StringToHash("AttackCombo");
    readonly int _hashDodgeAttack = Animator.StringToHash("DodgeAttack");
    readonly int _hashGetUP = Animator.StringToHash("GetUP");
    Vector3 _atkDir;
    public Vector3 AtkDir { set { _atkDir = value; } }
    float _originHitDelayValue;
    
    void Awake()
    {
        _state = eState.Idle;

        _move = GetComponent<PlayerMove_1>();
        _rbody = GetComponent<Rigidbody>();
        _combat = GetComponent<PlayerCombat>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _originHitDelayValue = _hitDelay;
    }

    void Update()
    {
        if (_state == eState.Dead || _state == eState.LadderOut)
            return;

        if (!_move._isGrounded)
            Fall();
        else
            _rbody.isKinematic = false;

        if (_state == eState.Attack)
            SetWeaponPos(_weaponCombatPos);
        else if (_state != eState.Attack && _weapon.transform.parent != _weaponNonCombatPos)
            SetWeaponPos(_weaponNonCombatPos);

        if (_state == eState.Hit)
        {
            KnockBack();
            return;
        }
    }

    // 맞은방향으로 일정시간동안 넉백시키는 함수
    void KnockBack()
    {
        if (_hitDelay > 0.0f)
        {
            // _rbody.addforce
            _rbody.isKinematic = false;
            _rbody.MovePosition(_rbody.position + _atkDir * Time.deltaTime * Mathf.Pow(_knockValue, _hitDelay * _knockBackDelta));
            _hitDelay -= Time.deltaTime;
            return;
        }

        _move.ClimbDown();
        _animator.SetTrigger(_hashGetUP);
        _hitDelay = _originHitDelayValue;
        _state = eState.Idle;
    }

    void Fall()
    {
        ResetPlayerAnimParams();
        _state = eState.Fall;
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(_combat.ActFallAttack(_rbody, _animator));
        }
    }

    // 무기의 위치를 옮겨주는 함수
    void SetWeaponPos(Transform parent)
    {
        _weapon.transform.SetParent(parent);
        _weapon.transform.localPosition = Vector3.zero;
        _weapon.transform.localEulerAngles = Vector3.zero;
    }

    // 애니메이션 파라미터 초기화
    public void ResetPlayerAnimParams()
    {
        _animator.SetBool(_hashRoll, false);
        _animator.SetInteger(_hashCombo, 0);
        _animator.ResetTrigger(_hashDodgeAttack);
    }
}
