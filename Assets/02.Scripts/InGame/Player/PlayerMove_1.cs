using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove_1 : MonoBehaviour
{
    [Header("=== Move ===")]
    [SerializeField] private float _movSpeed;
    [SerializeField] private float _rotSpeed;
    [SerializeField] private Vector3 _dir;
    private float _h;
    private float _v;

    [Header("=== Grounded ===")]
    [SerializeField] private Transform _grounded;
    public bool _isGrounded; // 디버깅 용

    [Header("=== Slope ===")]
    private const float RAY_DIST = 2.1f;
    private RaycastHit _slopeHit;
    private float _maxSlope = 50.0f;
    [SerializeField] private Transform _nextPos;
    private enum eOnSlopeState { None, currOnSlope, nextOnSlope }

    [Header("=== Dodge ===")]
    [SerializeField] private float _dodgeSpeed;
    [SerializeField] private float _dodgeDur;
    [SerializeField] private float _currDodgeDur;
    [SerializeField] private float _dodgeCoolTime;
    [SerializeField] private Vector3 _dodgeDir;
    private float _currDodgeCool; // 디버깅용으로 [SerializeField]
    private bool _isDodgeAttackInput;
    

    [Header("=== Anim Params ===")]
    private Animator _animator;
    readonly int _hashMove = Animator.StringToHash("isMove");
    readonly int _hashRoll = Animator.StringToHash("isRoll");
    readonly int _hashFall = Animator.StringToHash("isFall");

    [Header("=== Component ===")]
    private Rigidbody _rbody;
    private PlayerFSM _state;
    private PlayerCombat _combat;
    private CapsuleCollider _capsuleColl;

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

        _isGrounded = IsGrounded();
        eOnSlopeState onSlopeState = GetSlopeState();
        Vector3 velocity = CalcNextFrameGroundAngle(movSpeed) < _maxSlope ? dir : Vector3.zero;
        Vector3 gravity = Vector3.down * Mathf.Abs(_rbody.velocity.y);

        if (_isGrounded)
        {
            switch (onSlopeState)
            {
                case eOnSlopeState.None:
                    //print("평지");
                    _rbody.useGravity = true;
                    break;

                case eOnSlopeState.currOnSlope:
                case eOnSlopeState.nextOnSlope:
                    //print("현재 경사면 위 or 다음이 경사면 위");
                    _rbody.useGravity = false;
                    velocity = GetSlopeDir(velocity);
                    gravity = Vector3.zero;
                    break;

                #region 23.08.21 다음 프레임이 경사면일 경우 움직일 방향을 꺾을예정이었으나, 결론적으론 한 프레임앞서서 느려질 뿐이었음
                    //case eOnSlopeState.currOnSlope:
                    //    //print("현재 경사");
                    //    _rbody.useGravity = false;
                    //    velocity = GetSlopeDir(velocity);
                    //    gravity = Vector3.zero;
                    //    break;

                    //case eOnSlopeState.nextOnSlope:
                    //    print("다음이 경사");
                    //    var nextFramePlayerPos = _nextPos.position + velocity * _movSpeed * Time.fixedDeltaTime * 0.5f;
                    //    if (Physics.Raycast(nextFramePlayerPos, Vector3.down, out RaycastHit hit, RAY_DIST, 1 << LayerMask.NameToLayer("Ground")))
                    //    {
                    //        //Debug.DrawRay(nextFramePlayerPos, Vector3.down * RAY_DIST, Color.red); // 한 프레임 뒤 땅 판단용
                    //        //print("next : " + Vector3.Angle(Vector3.up, hit.normal)); // 한 프레임 뒤 땅의 각도
                    //        //print("curr : " + Vector3.Angle(Vector3.up, _slopeHit.normal)); // 현재 땅의 각도
                    //        //print("curr Dir :" + dir); // 현재 진행 방향
                    //        //print("Goal dir : " + Vector3.ProjectOnPlane(dir, hit.normal)); // 한 프레임 뒤 진행 방향

                    //        velocity = Vector3.ProjectOnPlane(dir, hit.normal);
                    //        velocity = velocity.y < 0.0f ? new Vector3(dir.x, 0.0f, dir.z) : velocity;
                    //        print(velocity);
                    //    }

                    //    _rbody.useGravity = false;
                    //    velocity = GetSlopeDir(velocity);
                    //    gravity = Vector3.zero;
                    //    break;
                    #endregion
            }
        }

        _animator.SetBool(_hashMove, (_h != 0.0f || _v != 0.0f) && _state.State != PlayerFSM.eState.Dodge);
        //print(velocity);
        _rbody.velocity = velocity * movSpeed + gravity;
    }

    private bool IsGrounded()
    {
        Vector3 boxSize = _grounded.transform.lossyScale;
        return Physics.CheckBox(_grounded.position, boxSize, Quaternion.identity, 1 << LayerMask.NameToLayer("Ground"));
    }

    private eOnSlopeState GetSlopeState()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DIST, 1 << LayerMask.NameToLayer("Ground")))
        {
            float currAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            float nextAngle = CalcNextFrameGroundAngle(_movSpeed);

            if (currAngle != 0.0f && currAngle < _maxSlope)
                return eOnSlopeState.currOnSlope;

            if (nextAngle != 0.0f && nextAngle < _maxSlope)
                return eOnSlopeState.nextOnSlope;

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

        if (Physics.Raycast(pos, transform.forward, _capsuleColl.radius * 1.5f, 1 << LayerMask.NameToLayer("Ground"))
            && collision.gameObject.tag.Equals("Wall") && (_h != 0.0f || _v != 0.0f))
        {
            Vector3 wallNorm = collision.contacts[0].normal;
            Vector3 projection = Vector3.ProjectOnPlane(transform.forward - wallNorm, wallNorm); // 벽과 마찰시 움직이게 될 벡터
            float power = _movSpeed * projection.magnitude * 2.5f;

            _rbody.AddForce(projection.normalized * power, ForceMode.Impulse);
        }
    }
}
