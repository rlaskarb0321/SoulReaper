using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Move & Rotate")]
    public float _movSpeed;
    public float _rotSpeed;

    [Header("Dodge")]
    [Tooltip("dodgeSpeed = _dodgeSpeed * _movSpeed")]public float _dodgeSpeed;
    public float _dodgeDur; // ��������°� ���ӵ� �ð�
    public float _dodgeCoolDown;

    [Header("Follow Cam")]
    public GameObject _followCamObj;

    private Vector3 _dir; // �÷��̾��� wasd�������� ���Ե� ���⺤�Ͱ��� ����
    private Animator _animator;
    private Rigidbody _rbody;
    private float _h;
    private float _v;
    private bool _dodgeAttackEnd;
    private float _originDodgeCoolDown;

    [Header("Component")]
    PlayerCombat _combat;
    PlayerState _state;
    FollowCamera _followCam;
    FallBehaviour _fallBehaviour;

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
    }

    void Start()
    {
        _originDodgeCoolDown = _dodgeCoolDown;
    }

    void FixedUpdate()
    {
        // �ֱ������� �߷°�������� �������� ����� ������ΰ���
        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);

        // idle, fall, move���°� �ƴ϶�� �����̰� ������ �� ����
        if (_state.State != PlayerState.eState.Idle && _state.State != PlayerState.eState.Fall && _state.State != PlayerState.eState.Move)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        // h�� v�� ��� �ϳ��� �Է��̵ȴٸ�
        if ((_h != 0.0f || _v != 0.0f))
        {
            MovePlayer(); 
            RotatePlayer();
        }
        // h�� v�� �ϳ��� �Էµ����ʰ�, �������»��¿� ���ݻ��°� �ƴ϶�� = idle����
        else if (!_fallBehaviour._isFall && _state.State != PlayerState.eState.Attack)
        {
            _state.State = PlayerState.eState.Idle;
            _animator.SetBool(_hashMove, false);
        }
    }

    void Update()
    {
        if (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move || _state.State == PlayerState.eState.Fall)
        {
            _h = Input.GetAxisRaw("Horizontal");
            _v = Input.GetAxisRaw("Vertical");
        }

        // ȸ��Ű �Է°���
        if (Input.GetKeyDown(KeyCode.Space) && 
            (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move ||
            _state.State == PlayerState.eState.Charging) && _dodgeCoolDown == _originDodgeCoolDown)
            StartCoroutine(Dodge());
    }

    // �÷��̾��� ���¸� idle�� �ٲٰ� �����̴¸�����, ���� ������ ����
    void MovePlayer()
    {
        // �������� �ӷ��� ������ �����̸� fall���·� ��ȯ
        if (_animator.GetFloat(_hashYVelocity) <= -0.4f)
            _state.State = PlayerState.eState.Fall;
        else
            _state.State = PlayerState.eState.Move;

        // ���������������ƴ��� ���η� move�ִϸ��̼� ����
        bool isFall = _state.State == PlayerState.eState.Fall ? true : false;
        _animator.SetBool(_hashMove, !isFall);
        _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        transform.position += _dir * _movSpeed * Time.deltaTime;
    }

    // �÷��̾ �̵���Ű���� �������� ĳ���͸� �������ϰ� ȸ��������
    void RotatePlayer()
    {
        if (_dir == Vector3.zero)
            return;

        Quaternion newRot = Quaternion.LookRotation(_dir);
        _rbody.rotation = Quaternion.Slerp(_rbody.rotation, newRot, _rotSpeed * Time.deltaTime);
    }

    IEnumerator Dodge()
    {
        if (_combat._curLongRangeChargingTime >= 0.0f)
            _combat.InitChargingGauge();
        if (_followCam.CamState != FollowCamera.eCameraState.Follow)
        {
            _followCam.CamState = FollowCamera.eCameraState.Follow;
            Input.ResetInputAxes();
        }

        bool isDodgeAttackInput = false;
        Vector3 dodgeDir = transform.forward.normalized; // ȸ��Ű ������ ĳ���Ͱ� �����ִ� ����
        float currDur = 0.0f;
        float dodgeSpeed = _movSpeed * _dodgeSpeed;

        // �����⵿��
        StartCoroutine(CoolDownDodge());
        _state.State = PlayerState.eState.Dodge;
        _animator.SetBool(_hashRoll, true);
        while (currDur < _dodgeDur)
        {
            // �������߰�Ÿ �Է°� ����
            if (Input.GetMouseButtonDown(0))
                isDodgeAttackInput = true;

            currDur += Time.deltaTime;
            transform.position += dodgeDir * dodgeSpeed * Time.deltaTime;
            yield return null;
        }

        // �������߰�Ÿ �Է¿��ο� ����ó��
        if (isDodgeAttackInput)
        {
            _animator.SetBool(_hashRoll, false);
            _combat.ActDodgeAttack(); // ȸ���߰�Ÿ Ʈ���� �ߵ�
            _state.State = PlayerState.eState.Attack;

            yield return new WaitUntil(() => _dodgeAttackEnd);
            _state.State = PlayerState.eState.Idle;
            _dodgeAttackEnd = false;
        }
        else
        {
            _state.State = PlayerState.eState.Idle;
            _animator.SetBool(_hashRoll, false);
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

    // ������ �߰�Ÿ �ִϸ��̼� �������� �޾Ƴ��� delegate
    public void OutDodgeAttack()
    {
        _dodgeAttackEnd = true;
    }
}
