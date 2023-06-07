using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Move & Rotate")]
    public float _movSpeed;
    public float _rotSpeed;
    [Range(0.0f, 90.0f)] public float _maxSlope;
    [HideInInspector] public float _v;
    [HideInInspector] public float _h;

    [Header("Dodge")]
    public float _dodgeSpeed; // 구를때 가속시킬 값 (dodgeSpeed = _dodgeSpeed * _movSpeed)
    public float _dodgeDur; // 구르기상태가 지속될 시간
    public float _dodgeCoolDown;
    
    [Header("Follow Cam")]
    public GameObject _followCamObj;

    Vector3 _dir; // 플레이어의 wasd조작으로 가게될 방향벡터값을 저장
    Animator _animator;
    Rigidbody _rbody;
    bool _dodgeAttackEnd;
    float _originDodgeCoolDown;
    WaitForFixedUpdate _wfs;

    [Header("Component")]
    PlayerCombat _combat;
    PlayerFSM _state;
    FollowCamera _followCam;
    FallBehaviour _fallBehaviour;
    CapsuleCollider _capsuleColl;
    SmoothDodgeBehaviour _smoothDodgeBehaviour;

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

        // 주기적으로 중력값을계산해 떨어질때 모션의 재생여부결정
        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);

        // idle, fall, move상태가 아니라면 움직이게 조작할 수 없음
        if (_state.State != PlayerFSM.eState.Idle && _state.State != PlayerFSM.eState.Fall && _state.State != PlayerFSM.eState.Move)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        // h나 v중 적어도 하나가 입력이된다면
        if ((_h != 0.0f || _v != 0.0f))
        {
            MovePlayer(); 
            RotatePlayer();
        }
        // h나 v중 하나도 입력되지않고, 떨어지는상태와 공격상태와 사망상태가 아니라면 = idle상태
        else if (!_fallBehaviour._isFall && _state.State != PlayerFSM.eState.Attack && _state.State != PlayerFSM.eState.Dead)
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

        if (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move || _state.State == PlayerFSM.eState.Fall)
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

    // 플레이어의 상태를 idle로 바꾸고 움직이는모션재생, 실제 움직임 구현
    void MovePlayer()
    {
        // 떨어질때 속력이 일정값 이하이면 fall상태로 전환
        if (_animator.GetFloat(_hashYVelocity) <= -0.4f)
            _state.State = PlayerFSM.eState.Fall;
        else
            _state.State = PlayerFSM.eState.Move;

        RaycastHit groundHit;
        Ray ray = new Ray(transform.position, -transform.up);

        #region Ray 추가로 예전코드 비활성화
        // 떨어지는중인지아닌지 여부로 move애니메이션 결정
        // bool isFall = _state.State == PlayerFSM.eState.Fall ? true : false;
        //_animator.SetBool(_hashMove, !isFall);  
        # endregion Ray 추가로 예전코드 비활성화

        if (Physics.Raycast(ray, out groundHit, 1.35f, 1 << LayerMask.NameToLayer("Ground")))
        {
            _animator.SetBool(_hashMove, true);
            _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        
            if (Vector3.Angle(transform.up, groundHit.normal) != 0)
            {
                Vector3 slopeMoveDir = Vector3.ProjectOnPlane(_dir, groundHit.normal);
                _dir = slopeMoveDir.normalized;
            }
        }
        else
        {
            _animator.SetBool(_hashMove, false);
            _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        }

        _rbody.MovePosition(_rbody.position + _dir * _movSpeed * Time.deltaTime);
    }

    public IEnumerator Dodge(float h, float v)
    {
        if (_combat._curLongRangeChargingTime >= 0.0f)
            _combat.InitChargingGauge();
        if (_followCam.CamState != FollowCamera.eCameraState.Follow)
        {
            _followCam.CamState = FollowCamera.eCameraState.Follow;
            Input.ResetInputAxes();
        }

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

            // 구르는 방향에 벽이 너무가까이있으면 벽을 뚫지않도록하기위해 짧은 ray발사 후, 충돌지역까지만 구르기로 이동
            currDur += Time.deltaTime;
            if (!Physics.Raycast(_rbody.position, transform.forward, out hit, _capsuleColl.radius))
            {
                // 플레이어 캡슐Coll의 반지름길이의 Ray가 벽과 충돌하지 않았을때는 구르기 이동
                _rbody.MovePosition(_rbody.position + dodgeDir * dodgeSpeed * Time.deltaTime);
            }
            else
            {
                // 플레이어 캡슐Coll 반지름길이의 Ray가 벽과 충돌한 경우에는
                // 벽의 각도를 구하고 해당 벽의 각도가 등반할 수 있는 값 이하이면 해당 경사면 방향으로 rbody MovePosition

                Physics.Raycast(_rbody.position, transform.forward, out hit, _capsuleColl.radius, 1 << LayerMask.NameToLayer("Ground"));
                wallAngle = Vector3.Angle(Vector3.up, hit.normal);
                // print(wallAngle);
                if (wallAngle <= _maxSlope)
                {
                    // 해당각도로 구르기이동
                    wallClimbDir = Vector3.ProjectOnPlane(Vector3.up, hit.normal);
                    wallClimbDir += transform.forward;
                    _rbody.MovePosition(_rbody.position + wallClimbDir * dodgeSpeed * Time.deltaTime);
                }
            }
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
}
