using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Move & Rotate")]
    public float _movSpeed;
    public float _rotSpeed;

    [Header("Dodge")]
    [Tooltip("dodgeSpeed = _dodgeSpeed * _movSpeed")]public float _dodgeSpeed;
    public float _dodgeDur; // 구르기상태가 지속될 시간
    public float _dodgeCoolDown;

    private Vector3 _dir; // 플레이어의 wasd조작으로 가게될 방향벡터값을 저장
    private Animator _animator;
    private Rigidbody _rbody;
    private PlayerState _state;
    private float _h;
    private float _v;

    readonly int _hashMove = Animator.StringToHash("isMove");
    readonly int _hashYVelocity = Animator.StringToHash("yVelocity");
    readonly int _hashRoll = Animator.StringToHash("isRoll");

    private void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _state = GetComponent<PlayerState>();
    }

    void FixedUpdate()
    {
        // 주기적으로 중력값을계산해 떨어질때 모션의 재생여부결정
        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);

        // idle, fall, move상태가 아니라면 움직이게 조작할 수 없음
        if (_state.State != PlayerState.eState.Idle && _state.State != PlayerState.eState.Fall && _state.State != PlayerState.eState.Move)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        // idle, fall, move상태일때 플레이어의 위치이동입력키를 받고, state를 idle 혹은 move로 전이시키는 분기점
        _h = Input.GetAxisRaw("Horizontal");
        _v = Input.GetAxisRaw("Vertical");

        if ((_h != 0.0f || _v != 0.0f))
        {
            MovePlayer(); // 플레이어의 상태를 idle로 바꾸고 움직이는모션재생, 실제 움직임 구현
            RotatePlayer(); // 플레이어가 이동시키려는 방향으로 캐릭터를 스무스하게 회전시켜줌
        }
        else if (_state.State != PlayerState.eState.Fall)
        {
            _state.State = PlayerState.eState.Idle;
            _animator.SetBool(_hashMove, false);
        }
    }

    private void Update()
    {
        // 회피키 입력관련
        if (Input.GetKeyDown(KeyCode.Space) && 
            (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move))
            StartCoroutine(Dodge());
    }

    // 플레이어의 상태를 idle로 바꾸고 움직이는모션재생, 실제 움직임 구현
    void MovePlayer()
    {
        // 떨어질때 속력이 일정값 이하이면 fall상태로 전환
        if (_animator.GetFloat(_hashYVelocity) <= -2.0f)
            _state.State = PlayerState.eState.Fall;
        else
            _state.State = PlayerState.eState.Move;

        // 떨어지는중인지아닌지 여부로 move애니메이션 결정
        bool isFall = _state.State == PlayerState.eState.Fall ? true : false;
        _animator.SetBool(_hashMove, !isFall);
        _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        transform.position += _dir * _movSpeed * Time.deltaTime;
    }

    // 플레이어가 이동시키려는 방향으로 캐릭터를 스무스하게 회전시켜줌
    void RotatePlayer()
    {
        if (_dir == Vector3.zero)
            return;

        Quaternion newRot = Quaternion.LookRotation(_dir);
        _rbody.rotation = Quaternion.Slerp(_rbody.rotation, newRot, _rotSpeed * Time.deltaTime);
    }

    IEnumerator Dodge()
    {
        Vector3 dodgeDir = transform.forward; // 회피키 누를때 캐릭터가 보고있던 방향
        float currDur = 0.0f; // 지수함수의 x축
        // float dodgeSpeed = (Mathf.Pow(0.055f, currDur) + _dodgeSpeed) * Time.deltaTime; // 속도를 x축이 높아질수록 크게줄어드는 지수함수값으로 설정
        float dodgeSpeed = _movSpeed * _dodgeSpeed * Time.deltaTime;

        // 구르기동작
        _state.State = PlayerState.eState.Dodge;
        _animator.SetBool(_hashRoll, true);
        while (currDur < _dodgeDur)
        {
            currDur += Time.deltaTime;
            transform.position += dodgeDir * dodgeSpeed;
            yield return null;
        }

        // 구르기끝난후 처리
        _state.State = PlayerState.eState.Idle;
        _animator.SetBool(_hashRoll, false);
    }
}
