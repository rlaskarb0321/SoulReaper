using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove_1 : MonoBehaviour
{
    [Header("=== Move ===")]
    [SerializeField] private float _movSpeed;
    [SerializeField] private float _rotSpeed;
    [Range(0.01f, 0.9f)] [SerializeField] private float _ladderSpeed;
    public Vector3 _dir;
    private float _h;
    private float _v;
    private eOnSlopeState _onSlopeState;

    [Header("=== Grounded ===")]
    [SerializeField] private Transform _grounded;
    public bool _isGrounded; // 디버깅 용

    [Header("=== Slope ===")]
    private const float RAY_DIST = 3.1f;
    private RaycastHit _slopeHit;
    private float _maxSlope = 50.0f;
    [SerializeField] private Transform _nextPos;
    private enum eOnSlopeState { None, CurrOnSlope, NextOnSlope, OnStairs }

    [Header("=== Stairs ===")]
    [SerializeField] private Transform _stepLower;
    [SerializeField] private Transform _stepUpper;
    [SerializeField] private float _lowerDist;
    [SerializeField] private float _upperDist;
    [SerializeField] private float _stepHeight;
    [Range(0.0f, 0.9f)] [SerializeField] private float _stairMovSpeed;
    private float _originSpeed;

    [Header("=== Dodge ===")]
    [SerializeField] private float _dodgeSpeed;
    [SerializeField] private float _dodgeDur;
    [SerializeField] private float _currDodgeDur;
    [SerializeField] private float _dodgeCoolTime;
    private Vector3 _dodgeDir;
    private float _currDodgeCool; // 디버깅용으로 [SerializeField]
    private bool _isDodgeAttackInput;
    
    [Header("=== Anim Params ===")]
    private Animator _animator;
    readonly int _hashMove = Animator.StringToHash("isMove");
    readonly int _hashRoll = Animator.StringToHash("isRoll");
    readonly int _hashFall = Animator.StringToHash("isFall");
    readonly int _hashIsLadder = Animator.StringToHash("isLadder");
    readonly int _hashLadderInput = Animator.StringToHash("isLadderInput");
    readonly int _hashClimbSpeed = Animator.StringToHash("ClimbSpeed");

    [Header("=== Component ===")]
    private Rigidbody _rbody;
    [HideInInspector] public PlayerFSM _state;
    private PlayerCombat _combat;
    private CapsuleCollider _capsuleColl;
    private int _groundLayer;

    private void Awake()
    {
        _capsuleColl = GetComponent<CapsuleCollider>();
        _rbody = GetComponent<Rigidbody>();
        _state = GetComponent<PlayerFSM>();
        _animator = GetComponent<Animator>();
        _combat = GetComponent<PlayerCombat>();
    }

    private void Start()
    {
        _groundLayer = 1 << LayerMask.NameToLayer("Ground");
        _currDodgeCool = _dodgeCoolTime;
        _dodgeSpeed *= _movSpeed;
        _originSpeed = _movSpeed;
        _stairMovSpeed *= _movSpeed;
    }

    private void Update()
    {
        if (_state.State == PlayerFSM.eState.Ladder)
        {
            ClimbLadder();
            return;
        }

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

            _dodgeDir = _dodgeDir.normalized;
            _state.State = PlayerFSM.eState.Dodge;
            transform.forward = _dodgeDir;
            StartCoroutine(CoolDownDodge());
            _animator.SetBool(_hashRoll, true);
        }
    }

    private void FixedUpdate()
    {
        if (_state.State == PlayerFSM.eState.Hit || _state.State == PlayerFSM.eState.Dead || _state.State == PlayerFSM.eState.Ladder)
        {
            return;
        }

        if (_state.State == PlayerFSM.eState.Dodge)
        {
            Dodge();
        }

        if (_state.State != PlayerFSM.eState.Idle && _state.State != PlayerFSM.eState.Fall && 
            _state.State != PlayerFSM.eState.Move && _state.State != PlayerFSM.eState.Ladder)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        _animator.SetBool(_hashFall, !_isGrounded);
        MovePlayer(_dir, _movSpeed);
        StepOnStair();
        RotatePlayer();

        if ((_h == 0.0f && _v == 0.0f) && !_animator.GetBool(_hashFall) &&
            _state.State != PlayerFSM.eState.Attack && _state.State != PlayerFSM.eState.Ladder &&
            _state.State != PlayerFSM.eState.Dead && _state.State != PlayerFSM.eState.Dodge)
        {
            _state.State = PlayerFSM.eState.Idle;
            _animator.SetBool(_hashMove, false);
        }
    }

    #region 플레이어 움직임 관련 메서드
    private void MovePlayer(Vector3 dir, float movSpeed)
    {
        if (_animator.GetBool(_hashFall))
            _state.State = PlayerFSM.eState.Fall;
        if ((_h != 0.0f || _v != 0.0f) && _state.State != PlayerFSM.eState.Dodge)
            _state.State = PlayerFSM.eState.Move;

        _isGrounded = IsGrounded();
        _rbody.useGravity = true;
        _onSlopeState = GetSlopeState();
        Vector3 velocity = dir;
        Vector3 gravity = Vector3.down * Mathf.Abs(_rbody.velocity.y);
        //print(_onSlopeState);

        if (_isGrounded)
        {
            switch (_onSlopeState)
            {
                case eOnSlopeState.None:
                    //print("평지");
                    _movSpeed = _movSpeed.Equals(_originSpeed) ? _originSpeed : Mathf.Lerp(_movSpeed, _originSpeed, 1.0f);
                    _rbody.useGravity = true;
                    velocity = CalcNextFrameGroundAngle(movSpeed) < _maxSlope ? dir : Vector3.zero;
                    break;

                case eOnSlopeState.CurrOnSlope:
                case eOnSlopeState.NextOnSlope:
                    //print("현재 경사면 위 or 다음이 경사면 위");
                    _movSpeed = _movSpeed.Equals(_originSpeed) ? _originSpeed : Mathf.Lerp(_movSpeed, _originSpeed, 0.5f);
                    _rbody.useGravity = false;
                    velocity = GetSlopeDir(velocity);
                    gravity = Vector3.zero;
                    break;

                case eOnSlopeState.OnStairs:
                    _movSpeed = Mathf.Lerp(_movSpeed, _stairMovSpeed, 0.5f);
                    _rbody.useGravity = dir == Vector3.zero ? false : true;
                    velocity = dir;
                    gravity = Vector3.zero;
                    break;
            }
        }

        _animator.SetBool(_hashMove, (_h != 0.0f || _v != 0.0f) && _state.State != PlayerFSM.eState.Dodge);
        _rbody.velocity = velocity * movSpeed + gravity;
    }

    private void StepOnStair()
    {
        RaycastHit hit;
        bool isLowerHit 
            = Physics.Raycast(_stepLower.position, transform.TransformDirection(Vector3.forward), out hit, _lowerDist, _groundLayer);
        bool isUpperHit = Physics.Raycast(_stepUpper.position, transform.TransformDirection(Vector3.forward), _upperDist, _groundLayer);

        Debug.DrawRay(_stepLower.position, transform.TransformDirection(Vector3.forward) * _lowerDist, Color.red);
        Debug.DrawRay(_stepUpper.position, transform.TransformDirection(Vector3.forward) * _upperDist, Color.red);

        if (hit.collider != null && (hit.collider.tag.Equals("Wall") || hit.collider.tag.Equals("Slope")))
            return;

        // 올라갈때
        if (isLowerHit && !isUpperHit)
        {
            if (!_dir.Equals(Vector3.zero))
                _rbody.position += new Vector3(0f, _stepHeight * Time.deltaTime, 0f);
        }

        // 내려갈때
        if (!isLowerHit && !isUpperHit && _onSlopeState == eOnSlopeState.OnStairs)
        {
            RaycastHit downHit;
            if (Physics.Raycast(transform.position, Vector3.down, out downHit, _groundLayer))
            {
                if (downHit.distance > 0.8f)
                {
                    //print("down?");
                    _isGrounded = true;
                    _rbody.position -= new Vector3(0f, _stepHeight * Time.deltaTime, 0f);
                }
            }
        }
    }

    private bool IsGrounded()
    {
        Vector3 boxSize = _grounded.transform.lossyScale;
        return Physics.CheckBox(_grounded.position, boxSize, Quaternion.identity, _groundLayer);
    }

    private eOnSlopeState GetSlopeState()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DIST, _groundLayer))
        {
            if (_slopeHit.collider.tag.Equals("Stairs"))
                return eOnSlopeState.OnStairs;

            float currAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            float nextAngle = CalcNextFrameGroundAngle(_movSpeed);

            if (currAngle != 0.0f && currAngle < _maxSlope)
                return eOnSlopeState.CurrOnSlope;

            if (nextAngle != 0.0f && nextAngle < _maxSlope)
                return eOnSlopeState.NextOnSlope;

        }
        return eOnSlopeState.None;
    }

    private Vector3 GetSlopeDir(Vector3 dir)
    {
        return Vector3.ProjectOnPlane(dir, _slopeHit.normal).normalized;
    }

    private float CalcNextFrameGroundAngle(float movSpeed)
    {
        var nextFramePlayerPos = _nextPos.position + _dir * _movSpeed * Time.fixedDeltaTime * 0.5f;
        if (Physics.Raycast(nextFramePlayerPos, Vector3.down, out RaycastHit hit, RAY_DIST, _groundLayer))
        {
            if (hit.collider.tag.Equals("Stairs"))
            {
                return 0.0f;
            }
            else
            {
                return Vector3.Angle(Vector3.up, hit.normal);
            }
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
            if (_isDodgeAttackInput)
            {
                print("dodge attack");
                _isDodgeAttackInput = false;
            }

            _animator.SetBool(_hashRoll, false);
            _state.State = PlayerFSM.eState.Idle;
            _currDodgeDur = 0.0f;
            return;
        }

        if (Input.GetMouseButton(0) && !_isDodgeAttackInput)
        {
            _isDodgeAttackInput = true;
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

    private void OnCollisionStay(Collision collision)
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

        if (Physics.Raycast(pos, transform.forward, _capsuleColl.radius * 1.5f, _groundLayer)
            && collision.gameObject.tag.Equals("Wall") && (_h != 0.0f || _v != 0.0f))
        {
            Vector3 wallNorm = collision.contacts[0].normal;
            Vector3 projection = Vector3.ProjectOnPlane(transform.forward - wallNorm, wallNorm); // 벽과 마찰시 움직이게 될 벡터
            float power = _movSpeed * projection.magnitude * 2.5f;

            _rbody.AddForce(projection.normalized * power, ForceMode.Impulse);
        }
    }

    private void ClimbLadder()
    {
        if (!_animator.GetBool(_hashIsLadder))
            _animator.SetBool(_hashIsLadder, true);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClimbDown();
            return;
        }

        _v = Input.GetAxisRaw("Vertical");
        _rbody.isKinematic = true;
        _rbody.position += new Vector3(0.0f, _v * (Time.fixedDeltaTime * _ladderSpeed), 0.0f);
        _animator.SetBool(_hashLadderInput, _v != 0);
        _animator.SetFloat(_hashClimbSpeed, _v);
    }

    public void ClimbDown()
    {
        _state.State = PlayerFSM.eState.Fall;
        _rbody.isKinematic = false;
        _animator.SetBool(_hashIsLadder, false);
    }
}
