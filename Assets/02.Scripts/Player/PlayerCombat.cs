using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Follow Cam")]
    public GameObject _followCamObj;

    [Header("Attack")]
    [Tooltip("공격시 전진거리")]public float _attackAdvancedDist;
    [Tooltip("원거리공격 사용하는데 필요한 차징 시간")]public float _needChargingTime;
    [Tooltip("현재 원거리 공격충전시간")]public float _curLongRangeChargingTime;
    [Tooltip("낙하공격시 떨어지는 속도")] public float _fallAttackSpeed;

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
        // 근or원거리공격으로 모션전환관련
        if (Input.GetMouseButtonDown(0) && 
            (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move))
        {
            // 근거리 공격
            _state.State = PlayerState.eState.Attack;
            RotateToClickDir();
            _animator.SetInteger(_hashCombo, ++_combo);
        }

        // 차징모션으로 전환 관련
        else if (Input.GetMouseButton(1) && 
            (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move
            || _state.State == PlayerState.eState.Charging))
        {
            // 원거리공격
            if (_curLongRangeChargingTime < _needChargingTime)
            {
                _curLongRangeChargingTime += Time.deltaTime;
                _animator.SetFloat(_hashChargingValue, _curLongRangeChargingTime);
            }

            _state.State = PlayerState.eState.Charging;
            RotateToClickDir();
            _followCam.CamState = FollowCamera.eCameraState.Charging;
        }

        // 차징타임에따라 원거리공격을 하거나 취소를 결정
        else if (Input.GetMouseButtonUp(1))
        {
            // 원거리공격 발사or취소
            if (_curLongRangeChargingTime > _needChargingTime)
            {
                Debug.Log("발사");
                _animator.SetTrigger(_hashChargingBurst);
            }

            InitChargingGauge();
            _followCam.CamState = FollowCamera.eCameraState.Follow;
            _state.State = PlayerState.eState.Idle;
        }
    }

    /// <summary>
    /// 클릭한 마우스 방향으로 캐릭터를 회전시킴
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

    // Attack 애니메이션 마지막에 달아놓는 Delegate, 콤보를 더 이어나갈지 공격을 끝낼지 결정한다.
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

    // 콤보공격단계중 마지막콤보의 마지막 프레임에 달아놓는 Delegate, 어택콤보를 초기화시킨다.
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

                _state.State = PlayerState.eState.Attack; // 공격상태로 전환
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
