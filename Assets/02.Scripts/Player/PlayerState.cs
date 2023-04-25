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
    FallBehaviour _fallBehaviour;

    // Field
    readonly int _hashRoll = Animator.StringToHash("isRoll");
    readonly int _hashCombo = Animator.StringToHash("AttackCombo");
    readonly int _hashDodgeAttack = Animator.StringToHash("DodgeAttack");
    readonly int _hashHit = Animator.StringToHash("Hit");
    readonly int _hashGetUP = Animator.StringToHash("GetUP");
    bool _isRoll;
    int _atkCombo;
    Vector3 _atkDir;
    float _originHitDelayValue;
    GameObject _followCamObj;
    FollowCamera _followCam;

    // public
    [Header("Combat")]
    public float _hitDelay;
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
        _fallBehaviour = _animator.GetBehaviour<FallBehaviour>();
        _followCamObj = _combat._followCamObj;
        _followCam = _followCamObj.GetComponent<FollowCamera>();
    }

    void Start()
    {
        _isRoll = _animator.GetBool(_hashRoll);
        _originHitDelayValue = _hitDelay;
        _atkCombo = _animator.GetInteger(_hashCombo);
    }

    void Update()
    {
        if (_fallBehaviour._isFall)
            Fall();

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

    // �÷��̾ �������������� ����ʹ����̾����� �˷��ִ� �Լ�
    public void GetHit(Vector3 attackDir)
    {
        if (_state == eState.Hit)
            return;

        _state = eState.Hit;
        StartCoroutine(_followCam.ShakingCamera());
        attackDir = attackDir.normalized;
        transform.forward = -attackDir;
        _animator.SetTrigger(_hashHit);
        _combat.EndComboAtk();
        _atkDir = attackDir;
    }

    // ������������ �����ð����� �˹��Ű�� �Լ�
    void KnockBack()
    {
        if (_hitDelay > 0.0f)
        {
            _rbody.MovePosition(_rbody.position + _atkDir * Time.deltaTime * Mathf.Pow(50448.5f, _hitDelay * 0.15f));
            _hitDelay -= Time.deltaTime;
            return;
        }

        _animator.SetTrigger(_hashGetUP);
        _hitDelay = _originHitDelayValue;
        _state = eState.Idle;
    }

    void Fall()
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

    // ������ ��ġ�� �Ű��ִ� �Լ�
    void SetWeaponPos(Transform parent)
    {
        _weapon.transform.SetParent(parent);
        _weapon.transform.localPosition = Vector3.zero;
        _weapon.transform.localEulerAngles = Vector3.zero;
    }
}
