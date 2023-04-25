using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Move & Rotate")]
    public float _h;
    public float _v;
    public float _movSpeed;
    public float _rotSpeed;

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

    [Header("Component")]
    PlayerCombat _combat;
    PlayerState _state;
    FollowCamera _followCam;
    FallBehaviour _fallBehaviour;
    SmoothDodgeBehaviour _smoothDodgeBehaviour;

    readonly int _hashMove = Animator.StringToHash("isMove");
    readonly int _hashYVelocity = Animator.StringToHash("yVelocity");
    readonly int _hashRoll = Animator.StringToHash("isRoll");

    void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _state = GetComponent<PlayerState>();
        _combat = GetComponent<PlayerCombat>();
        _followCam = _followCamObj.GetComponent<FollowCamera>();
        _fallBehaviour = _animator.GetBehaviour<FallBehaviour>();
        _smoothDodgeBehaviour = _animator.GetBehaviour<SmoothDodgeBehaviour>();
    }

    void Start()
    {
        _originDodgeCoolDown = _dodgeCoolDown;
    }

    void FixedUpdate()
    {
        if (_state.State == PlayerState.eState.Hit)
            return;

        // 주기적으로 중력값을계산해 떨어질때 모션의 재생여부결정
        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);

        // idle, fall, move상태가 아니라면 움직이게 조작할 수 없음
        if (_state.State != PlayerState.eState.Idle && _state.State != PlayerState.eState.Fall && _state.State != PlayerState.eState.Move)
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
        // h나 v중 하나도 입력되지않고, 떨어지는상태와 공격상태가 아니라면 = idle상태
        else if (!_fallBehaviour._isFall && _state.State != PlayerState.eState.Attack)
        {
            _state.State = PlayerState.eState.Idle;
            _animator.SetBool(_hashMove, false);
        }
    }

    void Update()
    {
        if (_state.State == PlayerState.eState.Hit)
            return;

        if (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move || _state.State == PlayerState.eState.Fall)
        {
            _h = Input.GetAxisRaw("Horizontal");
            _v = Input.GetAxisRaw("Vertical");
        }

        // 회피키 입력관련
        if (Input.GetKeyDown(KeyCode.Space) && 
            (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move ||
            _state.State == PlayerState.eState.Charging) && _dodgeCoolDown == _originDodgeCoolDown)
            StartCoroutine(Dodge(_h, _v));
    }

    // 플레이어의 상태를 idle로 바꾸고 움직이는모션재생, 실제 움직임 구현
    void MovePlayer()
    {
        // 떨어질때 속력이 일정값 이하이면 fall상태로 전환
        if (_animator.GetFloat(_hashYVelocity) <= -0.4f)
            _state.State = PlayerState.eState.Fall;
        else
            _state.State = PlayerState.eState.Move;

        // 떨어지는중인지아닌지 여부로 move애니메이션 결정
        bool isFall = _state.State == PlayerState.eState.Fall ? true : false;
        _animator.SetBool(_hashMove, !isFall);
        _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        _rbody.MovePosition(_rbody.position + _dir * _movSpeed * Time.deltaTime);
    }

    // 플레이어가 이동시키려는 방향으로 캐릭터를 스무스하게 회전시켜줌
    void RotatePlayer()
    {
        if (_dir == Vector3.zero)
            return;

        Quaternion newRot = Quaternion.LookRotation(_dir);
        _rbody.rotation = Quaternion.Slerp(_rbody.rotation, newRot, _rotSpeed * Time.deltaTime);
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

        // 구르기동작
        StartCoroutine(CoolDownDodge());
        _state.State = PlayerState.eState.Dodge;
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
            RaycastHit hit;
            currDur += Time.deltaTime;
            if (!Physics.Raycast(_rbody.position, transform.forward, out hit, 0.25f))
            {
                _rbody.MovePosition(_rbody.position + dodgeDir * dodgeSpeed * Time.deltaTime);
            }

            yield return new WaitForFixedUpdate();
        }

        // 구르기추가타 입력여부에 따른처리
        if (isDodgeAttackInput)
        {
            _animator.SetBool(_hashRoll, false);
            _smoothDodgeBehaviour._isDodgeInput = false;
            _combat.ActDodgeAttack(); // 회피추가타 트리거 발동
            _state.State = PlayerState.eState.Attack;
            _combat.RotateToClickDir();

            yield return new WaitUntil(() => _dodgeAttackEnd);
            _state.State = PlayerState.eState.Idle;
            _dodgeAttackEnd = false;
        }
        else
        {
            _state.State = PlayerState.eState.Idle;
            _animator.SetBool(_hashRoll, false);
            _smoothDodgeBehaviour._isDodgeInput = false;
        }
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
    }
}
