using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove_1 : MonoBehaviour
{
    [Header("=== Move ===")]
    [SerializeField] private float _movSpeed;
    [SerializeField] private float _rotSpeed;
    [SerializeField] private float _h;
    [SerializeField] private float _v;
    [SerializeField] private Vector3 _dir;

    [Header("=== Grounded ===")]
    [SerializeField] private Transform _grounded;
    public bool _isGrounded; // 디버깅 용

    [Header("=== Slope ===")]
    private const float RAY_DIST = 2.1f;
    private RaycastHit _slopeHit;
    private float _maxSlope = 50.0f;
    public bool _isOnSlope; // 디버깅 용
    [SerializeField] private Transform _nextPos;

    [Header("=== Dodge ===")]
    [SerializeField] private float _dodgeSpeed;
    [SerializeField] private float _dodgeDur;
    [SerializeField] private float _currDodgeDur;
    [SerializeField] private float _dodgeCoolTime;
    [SerializeField] private float _currDodgeCool; // 디버깅용으로 [SerializeField]
    [SerializeField] private Vector3 _dodgeDir;
    

    [Header("=== Anim Params ===")]
    private Animator _animator;
    readonly int _hashMove = Animator.StringToHash("isMove");
    readonly int _hashRoll = Animator.StringToHash("isRoll");
    readonly int _hashFall = Animator.StringToHash("isFall");

    [Header("=== Component ===")]
    private Rigidbody _rbody;
    private PlayerFSM _state;

    private void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _state = GetComponent<PlayerFSM>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _currDodgeCool = _dodgeCoolTime;
        _dodgeSpeed *= _movSpeed;
    }

    private void Update()
    {
        if (_state.State == PlayerFSM.eState.Hit || _state.State == PlayerFSM.eState.Dead)
        {
            return;
        }

        if (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move
            || _state.State == PlayerFSM.eState.Fall || _state.State == PlayerFSM.eState.Charging)
        {
            _h = Input.GetAxisRaw("Horizontal");
            _v = Input.GetAxisRaw("Vertical");
            _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _currDodgeCool == _dodgeCoolTime &&
            (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move || _state.State == PlayerFSM.eState.Charging))
        {
            if (_dir.Equals(Vector3.zero))
                _dodgeDir = transform.forward;
            else
                _dodgeDir = _dir;

            _state.State = PlayerFSM.eState.Dodge;
            transform.forward = _dodgeDir;
            StartCoroutine(CoolDownDodge());
            _animator.SetBool(_hashRoll, true);
        }
    }

    private void FixedUpdate()
    {
        //float angle = CalcNextFrameGroundAngle(_movSpeed);
        //if (_isGrounded && !_isOnSlope && angle != 0.0f)
        //{
        //    _isOnSlope = true;
        //    print(angle + ", bug");
        //}

        if (_state.State == PlayerFSM.eState.Hit || _state.State == PlayerFSM.eState.Dead)
        {
            return;
        }

        if (_state.State == PlayerFSM.eState.Dodge)
        {
            Dodge();
        }

        if (_state.State != PlayerFSM.eState.Idle && _state.State != PlayerFSM.eState.Fall && _state.State != PlayerFSM.eState.Move)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        if ((_h == 0.0f && _v == 0.0f) &&
            _state.State != PlayerFSM.eState.Fall && _state.State != PlayerFSM.eState.Attack 
            && _state.State != PlayerFSM.eState.Dead && _state.State != PlayerFSM.eState.Dodge)
        {
            _state.State = PlayerFSM.eState.Idle;
            _animator.SetBool(_hashMove, false);
        }

        _animator.SetBool(_hashFall, !_isGrounded);
        MovePlayer(_dir, _movSpeed);
        RotatePlayer();
    }

    #region 플레이어 움직임 관련 메서드
    private void MovePlayer(Vector3 dir, float movSpeed)
    {
        if (_animator.GetBool(_hashFall).Equals(true))
            _state.State = PlayerFSM.eState.Fall;
        if ((_h != 0.0f || _v != 0.0f) && _state.State != PlayerFSM.eState.Dodge)
            _state.State = PlayerFSM.eState.Move;

        bool isOnSlope = IsOnSlope();
        bool isGrounded = IsGrounded();
        Vector3 velocity = CalcNextFrameGroundAngle(movSpeed) < _maxSlope ? dir : Vector3.zero;
        Vector3 gravity = Vector3.down * Mathf.Abs(_rbody.velocity.y);

        _isGrounded = isGrounded;
        _isOnSlope = isOnSlope;
        if (isGrounded && isOnSlope)
        {
            velocity = GetSlopeDir(velocity);
            gravity = Vector3.zero;
            _rbody.useGravity = false;
        }
        else
        {
            _rbody.useGravity = true;
        }

        _animator.SetBool(_hashMove, (_h != 0.0f || _v != 0.0f) && _state.State != PlayerFSM.eState.Dodge);
        _rbody.velocity = velocity * movSpeed + gravity;
        //print(_rbody.velocity);
    }

    private bool IsGrounded()
    {
        Vector3 boxSize = _grounded.transform.lossyScale;
        return Physics.CheckBox(_grounded.position, boxSize, Quaternion.identity, 1 << LayerMask.NameToLayer("Ground"));
    }

    private bool IsOnSlope()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DIST, 1 << LayerMask.NameToLayer("Ground")))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle != 0 && angle < _maxSlope;
        }
        return false;

        //Ray ray = new Ray(transform.position, Vector3.down);
        //if (Physics.Raycast(ray, out _slopeHit, RAY_DIST, 1 << LayerMask.NameToLayer("Ground")))
        //{
        //    if (CalcNextFrameGroundAngle(_movSpeed) != 0.0f)
        //        return true;

        //    float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
        //    return angle != 0 && angle < _maxSlope;
        //}
        //return false;
    }

    private Vector3 GetSlopeDir(Vector3 dir)
    {
        return Vector3.ProjectOnPlane(dir, _slopeHit.normal).normalized;
    }

    private float CalcNextFrameGroundAngle(float movSpeed)
    {
        var nextFramePlayerPos = _nextPos.position + _dir * _movSpeed * Time.fixedDeltaTime;
        if (Physics.Raycast(nextFramePlayerPos, Vector3.down, out RaycastHit hit, RAY_DIST, 1 << LayerMask.NameToLayer("Ground")))
        {
            return Vector3.Angle(Vector3.up, hit.normal);
        }
        return 0.0f;
    }
    #endregion 플레이어 움직임 관련 메서드

    private void RotatePlayer()
    {
        if (_dir == Vector3.zero)
            return;

        Quaternion newRot = Quaternion.LookRotation(_dir);

        newRot = Quaternion.Slerp(_rbody.rotation, newRot, _rotSpeed * Time.deltaTime);
        newRot = Quaternion.Euler(transform.rotation.eulerAngles.x, newRot.eulerAngles.y, transform.rotation.eulerAngles.z);
        _rbody.rotation = newRot;
    }

    private void Dodge()
    {
        if (_currDodgeDur >= _dodgeDur)
        {
            _animator.SetBool(_hashRoll, false);
            _state.State = PlayerFSM.eState.Idle;
            _currDodgeDur = 0.0f;
            return;
        }

        MovePlayer(_dodgeDir, _dodgeSpeed);
        _currDodgeDur += Time.fixedDeltaTime;
    }

    private IEnumerator CoolDownDodge()
    {
        _currDodgeCool = 0.0f;
        while (_currDodgeCool <= _dodgeCoolTime)
        {
            _currDodgeCool += Time.deltaTime;
            yield return null;
        }

        _currDodgeCool = _dodgeCoolTime;
    }
}
