using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Follow Cam")]
    public GameObject _followCamObj;

    [Header("Attack")]
    [Tooltip("���ݽ� �����Ÿ�")]public float _attackAdvancedDist;
    [Tooltip("���Ÿ����� ����ϴµ� �ʿ��� ��¡ �ð�")]public float _needChargingTime;
    [Tooltip("���� ���Ÿ� ���������ð�")]public float _curLongRangeChargingTime;
    [Tooltip("���ϰ��ݽ� �������� �ӵ�")] public float _fallAttackSpeed;

    private Transform _player;
    private Camera _cam;
    private Animator _animator;
    private Rigidbody _rbody;
    private int _combo;
    private bool _unFreeze;

    [Header("Component")]
    private AttackComboBehaviour _atkBehaviour;
    private PlayerState _state;
    private FollowCamera _followCam;

    readonly int _hashCombo = Animator.StringToHash("AttackCombo");
    readonly int _hashChargingValue = Animator.StringToHash("ChargingValue");
    readonly int _hashChargingBurst = Animator.StringToHash("ChargingBurst");
    readonly int _hashFallAttack = Animator.StringToHash("FallAttack");
    readonly int _hashDodgeAttack = Animator.StringToHash("DodgeAttack");

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _state = GetComponent<PlayerState>();
        _atkBehaviour = _animator.GetBehaviour<AttackComboBehaviour>();
        _followCam = _followCamObj.GetComponent<FollowCamera>();
        _rbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _cam = Camera.main;
        _player = this.transform;
        _combo = 0;

        InitChargingGauge();
    }

    void Update()
    {
        // ��or���Ÿ��������� �����ȯ����
        if (Input.GetMouseButtonDown(0) && 
            (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move))
        {
            // �ٰŸ� ����
            _state.State = PlayerState.eState.Attack;
            RotateToClickDir();
            _animator.SetInteger(_hashCombo, ++_combo);
        }

        // ��¡������� ��ȯ ����
        else if (Input.GetMouseButton(1) && 
            (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move
            || _state.State == PlayerState.eState.Charging))
        {
            // ���Ÿ�����
            if (_curLongRangeChargingTime < _needChargingTime)
            {
                _curLongRangeChargingTime += Time.deltaTime;
                _animator.SetFloat(_hashChargingValue, _curLongRangeChargingTime);
            }

            _state.State = PlayerState.eState.Charging;
            RotateToClickDir();
            _followCam.CamState = FollowCamera.eCameraState.Charging;
        }

        // ��¡Ÿ�ӿ����� ���Ÿ������� �ϰų� ��Ҹ� ����
        else if (Input.GetMouseButtonUp(1))
        {
            // ���Ÿ����� �߻�or���
            if (_curLongRangeChargingTime > _needChargingTime)
            {
                Debug.Log("�߻�");
                _animator.SetTrigger(_hashChargingBurst);
            }

            InitChargingGauge();
            _followCam.CamState = FollowCamera.eCameraState.Follow;
            _state.State = PlayerState.eState.Idle;
        }
    }

    /// <summary>
    /// Ŭ���� ���콺 �������� ĳ���͸� ȸ����Ŵ
    /// </summary>
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

    public void InitChargingGauge()
    {
        _curLongRangeChargingTime = 0.0f;
        _animator.SetFloat(_hashChargingValue, _curLongRangeChargingTime);
    }

    // Attack �ִϸ��̼� �������� �޾Ƴ��� Delegate, �޺��� �� �̾���� ������ ������ �����Ѵ�.
    public void SetComboInteger()
    {
        if (_atkBehaviour._isComboAtk)
        {
            _combo++;
            if (_combo > 2)
                _combo = 0;

            RotateToClickDir();
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

    public IEnumerator ActFallAttack(Rigidbody rbody, Animator animator)
    {
        RaycastHit hit;
        Vector3 landingPoint;
        float originAnimSpeed;

        animator.SetBool(_hashFallAttack, true);
        rbody.isKinematic = true;
        originAnimSpeed = animator.speed;
        yield return new WaitUntil(() => _unFreeze);

        animator.speed = 0.0f;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            landingPoint = hit.point;
            rbody.isKinematic = false;

            while (true)
            {
                if (Vector3.Distance(transform.position, landingPoint) <= 1.0f)
                {
                    animator.speed = originAnimSpeed;
                    break;
                }

                _state.State = PlayerState.eState.Attack; // ���ݻ��·� ��ȯ
                //transform.position = Vector3.MoveTowards(transform.position, landingPoint, _fallAttackSpeed);
                _rbody.MovePosition(_rbody.position + Vector3.down * _fallAttackSpeed * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public void ActDodgeAttack()
    {
        _animator.SetTrigger(_hashDodgeAttack);
    }

    public void UnFreeze()
    {
        if (_unFreeze)
        {
            _animator.SetBool(_hashFallAttack, false);
            _state.State = PlayerState.eState.Idle;
            _unFreeze = false;
            return;
        }

        _unFreeze = true;
    }

    public IEnumerator GetHit(Vector3 hitDir)
    {
        //float setDelta = 1.5f;
        //float accDelta = 0.0f;
        //while (accDelta < setDelta)
        //{
        //    accDelta += Time.deltaTime;
        //    _rbody.AddForce(hitDir * 100.0f);
        //    yield return null;
        //}
        yield return null;
    }
}
