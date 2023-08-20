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
    [SerializeField] private GameObject[] _raySet;
    public bool _isGrounded;
    public bool _isOnSlope;
    public GameObject _nextPosRay;
    public Vector3 _dir; // 플레이어의 wasd조작으로 가게될 방향벡터값을 저장
    private Vector3 _gravity; // rigidbody.velocity 를 직접 조작하기때문에 중력또한 직접 조작
    private Ray _groundRay;
    private RaycastHit _groundHit;

    [Header("=== Dodge ===")]
    public float _dodgeSpeed; // 구를때 가속시킬 값 (dodgeSpeed = _dodgeSpeed * _movSpeed)
    public float _dodgeDur; // 구르기상태가 지속될 시간
    public float _currDodgeDur;
    public float _dodgeCoolDown;
    private float _originDodgeCoolDown;
    private bool _dodgeAttackEnd;
    public Vector3 _dodgeDir;

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
    readonly int _hashRoll = Animator.StringToHash("isRoll");
    readonly int _hashFall = Animator.StringToHash("isFall");

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
        _currDodgeDur = 0.0f;
        _wfs = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (_state.State == PlayerFSM.eState.Hit)
            return;
        if (_state.State == PlayerFSM.eState.Dead)
            return;

        if (_state.State == PlayerFSM.eState.Dodge)
        {
            if (_currDodgeDur > _dodgeDur)
            {
                _currDodgeDur = 0.0f;
                _state.State = PlayerFSM.eState.Idle;
                _animator.SetBool(_hashRoll, false);
                return;
            }

            _currDodgeDur += Time.deltaTime;
            _rbody.velocity = _dodgeDir * _movSpeed * _dodgeSpeed + _gravity;
        }

        // idle, fall, move상태가 아니라면 움직이게 조작할 수 없음
        if (_state.State != PlayerFSM.eState.Idle && _state.State != PlayerFSM.eState.Fall && _state.State != PlayerFSM.eState.Move)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        ManipulPhysics();
        _animator.SetBool(_hashFall, !_isGrounded);

        // h나 v가 입력됬을 때
        //if (_h != 0.0f || _v != 0.0f)
        //{
        //    _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        //    MovePlayer(_dir, _movSpeed);
        //    RotatePlayer();
        //}
        //// 아닐 때
        //else
        //{
        //    // 땅이나 경사위에 있을때, 인풋이 입력되었다가 떼어지면!
        //    if (_isGrounded || _isOnSlope)
        //    {
        //        _dir = Vector3.zero;
        //        _rbody.velocity = Vector3.zero;
        //    }
        //}

        _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        MovePlayer(_dir, _movSpeed);
        RotatePlayer();

        // 아닐 때
        if (_h == 0.0f && _v == 0.0f)
        {
            // 땅이나 경사위에 있을때, 인풋이 입력되었다가 떼어지면!
            if (_isGrounded || _isOnSlope)
            {
                _dir = Vector3.zero;
                _rbody.velocity = Vector3.zero;
            }
        }

        // h나 v중 하나도 입력되지않고, 떨어지는상태와 공격상태와 사망상태가 아니라면 = idle상태
        if ((_h == 0.0f && _v == 0.0f) &&
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
        {
            if (_dir.Equals(Vector3.zero))
            {
                _dodgeDir = transform.forward;
            }
            else
            {
                _dodgeDir = _dir;
            }

            transform.forward = _dodgeDir;
            _state.State = PlayerFSM.eState.Dodge;
            StartCoroutine(CoolDownDodge());
            _animator.SetBool(_hashRoll, true);
            //StartCoroutine(Dodge(_h, _v));
        }
    }

    void MovePlayer(Vector3 dir, float movSpeed)
    {
        // //떨어질때 속력이 일정값 이하이면 fall상태로 전환
        //if (_animator.GetBool(_hashFall))
        //    _state.State = PlayerFSM.eState.Fall;
        //else
        //    _state.State = PlayerFSM.eState.Move;

        //_animator.SetBool(_hashMove, _dir != Vector3.zero);
        //_dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        //_dir = _isOnSlope ? GetSlopeDir(_dir) : _dir;

        //_rbody.velocity = _dir * _movSpeed + _gravity;

        // 떨어질때 속력이 일정값 이하이면 fall상태로 전환
        if (_animator.GetBool(_hashFall))
            _state.State = PlayerFSM.eState.Fall;
        else
            _state.State = PlayerFSM.eState.Move;

        _animator.SetBool(_hashMove, dir != Vector3.zero);
        dir = _isOnSlope ? GetSlopeDir(dir) : dir;
        _rbody.velocity = dir * movSpeed + _gravity;
    }

    private bool OnGround()
    {
        Vector3 boxSize = _groundCheckBox.transform.lossyScale;
        return Physics.CheckBox(_groundCheckBox.transform.position, boxSize, Quaternion.identity, 1 << LayerMask.NameToLayer("Ground"));
    }

    private bool OnSlope(Ray ray)
    {
        if (Physics.Raycast(ray, out _groundHit, 1 << LayerMask.NameToLayer("Ground")))
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
        // 1. 현재 속도로 이동하면 닿게될 다음프레임에 벽이있을때 벽의 각도를 구하고 올라갈 수 없으면 못 올라가게
        // 그리고 올라갈때 멈칫하는 현상 해결할 수 있을듯
        // 2. 스파이더맨 현상

        _groundRay = new Ray(transform.position, Vector3.down);
        _isGrounded = OnGround();
        _isOnSlope = OnSlope(_groundRay);

        _gravity = _isGrounded ? Vector3.zero : Vector3.down * Mathf.Abs(_rbody.velocity.y);
        _rbody.useGravity = !_isOnSlope || !_isGrounded;
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
        bool defalutGravity = _rbody.useGravity;

        // 구르기동작
        StartCoroutine(CoolDownDodge());
        _rbody.useGravity = true;
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

            // 구르다가 경사진 면을 만났을 때, 경사면을 체크 후 등반할 수 있는 각도 이하이면 구르기각도 수정
            if (Physics.Raycast(_rbody.position, -transform.up, out hit, _capsuleColl.radius * _wallRayDintance,
                1 << LayerMask.NameToLayer("Ground")))
            {
                if (hit.transform.tag.Equals("Wall"))
                {
                    dodgeDir = Vector3.zero;
                }
                else
                {
                    wallAngle = Vector3.Angle(Vector3.up, hit.normal);
                    if (wallAngle <= _maxSlope)
                    {
                        // 해당각도로 구르기이동
                        wallClimbDir = Vector3.ProjectOnPlane(Vector3.up, hit.normal);
                        wallClimbDir += transform.forward;

                        dodgeDir = wallClimbDir;
                    }
                    else
                    {
                        dodgeDir = Vector3.zero;
                    }
                }
            }

            _rbody.MovePosition(_rbody.position + dodgeDir * dodgeSpeed * Time.deltaTime);
            //print(dodgeDir);
            //_rbody.velocity = dodgeDir * dodgeSpeed + _gravity;
            yield return _wfs;
        }
        _rbody.useGravity = defalutGravity;

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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Wall") && (_h != 0.0f || _v != 0.0f))
        {
            Vector3 wallNorm = collision.contacts[0].normal;
            Vector3 projection = Vector3.ProjectOnPlane(transform.forward - wallNorm, wallNorm); // 벽과 마찰시 움직이게 될 벡터
            float power = _movSpeed * projection.magnitude * 2.0f;

            //Debug.DrawRay(collision.contacts[0].point, wallNorm, Color.green);
            //Debug.DrawRay(transform.position, transform.forward - wallNorm, Color.red);
            //Debug.DrawRay(collision.contacts[0].point, projection, Color.blue);
            _rbody.AddForce(projection.normalized * power, ForceMode.Impulse);
        }
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
