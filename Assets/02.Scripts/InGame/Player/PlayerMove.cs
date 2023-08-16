using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("=== Move, Rotate ===")]
    public float _movSpeed;
    public float _rotSpeed;
    [Range(0.0f, 90.0f)] public float _maxSlope;
    [HideInInspector] public float _v;
    [HideInInspector] public float _h;
    [SerializeField] private float _wallRayDintance;
    [SerializeField] private GameObject _groundCheckBox;
    private bool _isGrounded;
    private bool _isOnSlope;
    private Vector3 _dir; // 플레이어의 wasd조작으로 가게될 방향벡터값을 저장
    private Vector3 _gravity; // rigidbody.velocity 를 직접 조작하기때문에 중력또한 직접 조작
    private Ray _groundRay;
    private RaycastHit _groundHit;

    [Header("=== Dodge ===")]
    public float _dodgeSpeed; // 구를때 가속시킬 값 (dodgeSpeed = _dodgeSpeed * _movSpeed)
    public float _dodgeDur; // 구르기상태가 지속될 시간
    public float _dodgeCoolDown;
    private float _originDodgeCoolDown;
    private bool _dodgeAttackEnd;
    
    [Header("=== Follow Cam ===")]
    public GameObject _followCamObj;

    [Header("=== Component ===")]
    private Animator _animator;
    private Rigidbody _rbody;
    private PlayerCombat _combat;
    private PlayerFSM _state;
    private FollowCamera _followCam;
    private FallBehaviour _fallBehaviour;
    private CapsuleCollider _capsuleColl;
    private SmoothDodgeBehaviour _smoothDodgeBehaviour;
    private WaitForFixedUpdate _wfs;

    [Header("=== Animator Params ===")]
    readonly int _hashMove = Animator.StringToHash("isMove");
    readonly int _hashYVelocity = Animator.StringToHash("yVelocity");
    readonly int _hashRoll = Animator.StringToHash("isRoll");

    void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _state = GetComponent<PlayerFSM>();
        _combat = GetComponent<PlayerCombat>();
        _followCam = _followCamObj.GetComponent<FollowCamera>();
        _fallBehaviour = _animator.GetBehaviour<FallBehaviour>();
        _smoothDodgeBehaviour = _animator.GetBehaviour<SmoothDodgeBehaviour>();
        _capsuleColl = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        _originDodgeCoolDown = _dodgeCoolDown;
        _wfs = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (_state.State == PlayerFSM.eState.Hit)
            return;
        if (_state.State == PlayerFSM.eState.Dead)
            return;

        // idle, fall, move상태가 아니라면 움직이게 조작할 수 없음
        if (_state.State != PlayerFSM.eState.Idle && _state.State != PlayerFSM.eState.Fall && _state.State != PlayerFSM.eState.Move)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);
        
        ManipulPhysics();

        // h나 v가 입력됬을 때
        if (_h != 0.0f || _v != 0.0f)
        {
            MovePlayer();
            RotatePlayer();
        }
        else
        {
            // 땅이나 경사위에 있을때, 인풋이 입력되었다가 떼어지면!
            if (_isGrounded || _isOnSlope)
            {
                _dir = Vector3.zero;
                _rbody.velocity = Vector3.zero;
            }
        }

        // h나 v중 하나도 입력되지않고, 떨어지는상태와 공격상태와 사망상태가 아니라면 = idle상태
        if (_h == 0.0f && _v == 0.0f
            &&
            !_fallBehaviour._isFall && _state.State != PlayerFSM.eState.Attack && _state.State != PlayerFSM.eState.Dead)
        {
            _state.State = PlayerFSM.eState.Idle;
            _animator.SetBool(_hashMove, false);
        }
    }

    void Update()
    {
        if (_state.State == PlayerFSM.eState.Hit)
            return;
        if (_state.State == PlayerFSM.eState.Dead)
            return;

        // _groundRay = new Ray(transform.position, -transform.up * 0.3f);
        if (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move 
            || _state.State == PlayerFSM.eState.Fall || _state.State == PlayerFSM.eState.Charging)
        {
            _h = Input.GetAxisRaw("Horizontal");
            _v = Input.GetAxisRaw("Vertical");
        }

        // 회피키 입력관련
        if (Input.GetKeyDown(KeyCode.Space) && 
            (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move ||
            _state.State == PlayerFSM.eState.Charging) && _dodgeCoolDown == _originDodgeCoolDown)
            StartCoroutine(Dodge(_h, _v));
    }

    void MovePlayer()
    {
        // 떨어질때 속력이 일정값 이하이면 fall상태로 전환
        if (_animator.GetFloat(_hashYVelocity) <= -5.0f)
            _state.State = PlayerFSM.eState.Fall;
        else
            _state.State = PlayerFSM.eState.Move;

        _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        _dir = _isOnSlope ? GetSlopeDir(_dir) : _dir;
        _animator.SetBool(_hashMove, _dir != Vector3.zero);
        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);
        
        _rbody.velocity = _dir * _movSpeed + _gravity;
    }

    private bool OnGround()
    {
        Vector3 boxSize = _groundCheckBox.transform.lossyScale;
        return Physics.CheckBox(_groundCheckBox.transform.position, boxSize, Quaternion.identity, 1 << LayerMask.NameToLayer("Ground"));
    }

    private bool OnSlope()
    {
        // 땅 판단을 레이캐스트대신 콜리더넣고 isGrounded 스크립트 생성, 공중에 있지 않을때에는 중력끄기
        if (Physics.Raycast(_groundRay, out _groundHit, 0.3f, 1 << LayerMask.NameToLayer("Ground")))
        {
            float angle = Vector3.Angle(transform.up, _groundHit.normal);
            return angle != 0 && angle < _maxSlope;
        }

        return false;
    }

    private Vector3 GetSlopeDir(Vector3 dir)
    {
        return Vector3.ProjectOnPlane(dir, _groundHit.normal).normalized;
    }

    /// <summary>
    /// velocity를 이용한 플레이어 조작에 의해 발생하는 여러 현상 케어하는 메서드
    /// </summary>
    private void ManipulPhysics()
    {
        _groundRay = new Ray(transform.position, Vector3.down);
        _isGrounded = OnGround();
        _isOnSlope = OnSlope();

        _gravity = _isGrounded ? Vector3.zero : Vector3.down * Mathf.Abs(_rbody.velocity.y);
        _rbody.useGravity = !_isOnSlope;
    }

    public IEnumerator Dodge(float h, float v)
    {
        if (_combat._curLongRangeChargingTime >= 0.0f)
            _combat.InitChargingGauge();
        if (_followCam.CamState != FollowCamera.eCameraState.Follow)
            _followCam.CamState = FollowCamera.eCameraState.Follow;

        bool isDodgeAttackInput = false;
        Vector3 dodgeDir; 
        float currDur = 0.0f;
        float dodgeSpeed = _movSpeed * _dodgeSpeed;
        RaycastHit hit;
        float wallAngle;
        Vector3 wallClimbDir;

        // 구르기동작
        StartCoroutine(CoolDownDodge());
        _state.State = PlayerFSM.eState.Dodge;
        _animator.SetBool(_hashRoll, true);

        // 키입력이 된 상태이면 구르기방향을 입력한방향으로, 아니라면 캐릭터기준 정면으로
        if (_h != 0.0f || _v != 0.0f)
            dodgeDir = ((h * Vector3.right) + (v * Vector3.forward)).normalized;
        else
            dodgeDir = transform.forward;

        transform.forward = dodgeDir;

        while (currDur < _dodgeDur)
        {
            // 구르기추가타 입력값 저장
            if (Input.GetMouseButton(0) && !isDodgeAttackInput)
                isDodgeAttackInput = true;

            currDur += Time.deltaTime;

            // 3개의 레이(머리 배꼽 발끝)가 벽과 닿아있을 때 = 벽과 닿음
            //if (IsDeadEndRoad())
            //{
            //    _rbody.MovePosition(_rbody.position + Vector3.zero * Time.deltaTime);
            //    yield return _wfs;
            //}

            // 구르다가 Wall 을 만났을 때, 경사면을 체크 후 등반할 수 있는 각도 이하이면 해당 각도방향으로 구르기
            if (Physics.Raycast(_rbody.position, transform.forward, out hit, _capsuleColl.radius * _wallRayDintance,
                1 << LayerMask.NameToLayer("Ground")))
            {
                if (hit.collider.tag.Equals("Wall"))
                {
                    wallAngle = Vector3.Angle(Vector3.up, hit.normal);
                    if (wallAngle <= _maxSlope)
                    {
                        // 해당각도로 구르기이동
                        wallClimbDir = Vector3.ProjectOnPlane(Vector3.up, hit.normal);
                        wallClimbDir += transform.forward;
                        _rbody.MovePosition(_rbody.position + wallClimbDir * dodgeSpeed * Time.deltaTime);
                    }
                    else
                    {
                        _rbody.MovePosition(_rbody.position + Vector3.zero * Time.deltaTime);
                    }
                }
            }
            // 평지를 구를때
            else
            {
                _rbody.MovePosition(_rbody.position + dodgeDir * dodgeSpeed * Time.deltaTime);
            }

            #region 23.08.13 머리 배꼽 발끝 3개의 레이가 벽과 닿았을때 벽 닿음으로 판단하게 수정
            //// 구르는 방향에 벽이 너무가까이있으면 벽을 뚫지않도록하기위해 짧은 ray발사 후, 충돌지역까지만 구르기로 이동
            //// Ray가 벽에 닿지 않았을 때
            //if (!Physics.Raycast(_rbody.position, transform.forward, out hit, _capsuleColl.radius))
            //{
            //    // 구르기 이동
            //    _rbody.MovePosition(_rbody.position + dodgeDir * dodgeSpeed * Time.deltaTime);
            //}
            //// Ray가 벽에 닿았을 때
            //else
            //{
            //    // 벽의 각도를 구하고 해당 벽의 각도가 등반할 수 있는 값 이하이면 해당 경사면 방향으로 rbody MovePosition
            //    Physics.Raycast(_rbody.position, transform.forward, out hit, _capsuleColl.radius * 2.0f, 1 << LayerMask.NameToLayer("Ground"));

            //    wallAngle = Vector3.Angle(Vector3.up, hit.normal);
            //    if (wallAngle <= _maxSlope)
            //    {
            //        // 해당각도로 구르기이동
            //        wallClimbDir = Vector3.ProjectOnPlane(Vector3.up, hit.normal);
            //        wallClimbDir += transform.forward;
            //        _rbody.MovePosition(_rbody.position + wallClimbDir * dodgeSpeed * Time.deltaTime);
            //    }
            //}
            #endregion 23.08.13 머리 배꼽 발끝 3개의 레이가 벽과 닿았을때 벽 닿음으로 판단하게 수정
            yield return _wfs;
        }

        // 구르기추가타 입력여부에 따른처리
        if (isDodgeAttackInput)
        {
            _animator.SetBool(_hashRoll, false);
            _smoothDodgeBehaviour._isDodgeInput = false;
            _combat.ActDodgeAttack(); // 회피추가타 트리거 발동
            _state.State = PlayerFSM.eState.Attack;
            _combat.RotateToClickDir();

            yield return new WaitUntil(() => _dodgeAttackEnd);
            _state.State = PlayerFSM.eState.Idle;
            _dodgeAttackEnd = false;
        }
        else
        {
            _state.State = PlayerFSM.eState.Idle;
            _animator.SetBool(_hashRoll, false);
            _smoothDodgeBehaviour._isDodgeInput = false;
        }
    }

    // 플레이어가 이동시키려는 방향으로 캐릭터를 스무스하게 회전시켜줌
    void RotatePlayer()
    {
        if (_dir == Vector3.zero)
            return;

        Quaternion newRot = Quaternion.LookRotation(_dir);

        newRot = Quaternion.Slerp(_rbody.rotation, newRot, _rotSpeed * Time.deltaTime);
        newRot = Quaternion.Euler(transform.rotation.eulerAngles.x, newRot.eulerAngles.y, transform.rotation.eulerAngles.z);
        _rbody.rotation = newRot;
    }

    IEnumerator CoolDownDodge()
    {
        _dodgeCoolDown = 0.0f;
        while (_dodgeCoolDown <= _originDodgeCoolDown)
        {
            _dodgeCoolDown += Time.deltaTime;
            yield return null;
        }

        _dodgeCoolDown = _originDodgeCoolDown;
    }

    // 구르기 추가타 애니메이션 마지막에 달아놓는 delegate
    public void OutDodgeAttack()
    {
        _dodgeAttackEnd = true;
        _combat._attackStyle = PlayerCombat.eAttackStyle.NonCombat;
    }

    /// <summary>
    /// 머리, 배꼽, 발에서 발사하는 레이 모두가 벽과 충돌하고있는지
    /// </summary>
    /// <returns></returns>
    //private bool IsDeadEndRoad()
    //{
    //    Ray ray;
    //    RaycastHit rayHit;
        
    //    // i번째 레이가 벽과 충돌여부 판단, 하나라도 벽과 충돌하고있지않으면 return false
    //    for (int i = 0; i < _wallHitRays.Length; i++)
    //    {
    //        ray = new Ray(_wallHitRays[i].transform.position, _wallHitRays[i].transform.forward);

    //        if (Physics.Raycast(ray, out rayHit, _capsuleColl.radius * _wallRayDintance, 1 << LayerMask.NameToLayer("Ground")))
    //        {
    //            if (!rayHit.collider.tag.Equals("Wall"))
    //            {
    //                return false;
    //            }
    //            Debug.DrawRay(_wallHitRays[i].transform.position, _wallHitRays[i].transform.forward * _capsuleColl.radius * _wallRayDintance, Color.green);
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}
}
