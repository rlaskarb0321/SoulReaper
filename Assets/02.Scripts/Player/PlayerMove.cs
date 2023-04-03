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

    private Vector3 _dir; // �÷��̾��� wasd�������� ���Ե� ���⺤�Ͱ��� ����
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
        // �ֱ������� �߷°�������� �������� ����� ������ΰ���
        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);

        // idle, fall, move���°� �ƴ϶�� �����̰� ������ �� ����
        if (_state.State != PlayerState.eState.Idle && _state.State != PlayerState.eState.Fall && _state.State != PlayerState.eState.Move)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        // idle, fall, move�����϶� �÷��̾��� ��ġ�̵��Է�Ű�� �ް�, state�� idle Ȥ�� move�� ���̽�Ű�� �б���
        _h = Input.GetAxisRaw("Horizontal");
        _v = Input.GetAxisRaw("Vertical");

        if ((_h != 0.0f || _v != 0.0f))
        {
            MovePlayer(); // �÷��̾��� ���¸� idle�� �ٲٰ� �����̴¸�����, ���� ������ ����
            RotatePlayer(); // �÷��̾ �̵���Ű���� �������� ĳ���͸� �������ϰ� ȸ��������
        }
        else if (_state.State != PlayerState.eState.Fall)
        {
            _state.State = PlayerState.eState.Idle;
            _animator.SetBool(_hashMove, false);
        }
    }

    private void Update()
    {
        // ȸ��Ű �Է°���
        if (Input.GetKeyDown(KeyCode.Space) && 
            (_state.State == PlayerState.eState.Idle || _state.State == PlayerState.eState.Move))
            StartCoroutine(Dodge());
    }

    // �÷��̾��� ���¸� idle�� �ٲٰ� �����̴¸�����, ���� ������ ����
    void MovePlayer()
    {
        // �������� �ӷ��� ������ �����̸� fall���·� ��ȯ
        if (_animator.GetFloat(_hashYVelocity) <= -2.0f)
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
        Vector3 dodgeDir = transform.forward; // ȸ��Ű ������ ĳ���Ͱ� �����ִ� ����
        float currDur = 0.0f; // �����Լ��� x��
        // float dodgeSpeed = (Mathf.Pow(0.055f, currDur) + _dodgeSpeed) * Time.deltaTime; // �ӵ��� x���� ���������� ũ���پ��� �����Լ������� ����
        float dodgeSpeed = _movSpeed * _dodgeSpeed * Time.deltaTime;

        // �����⵿��
        _state.State = PlayerState.eState.Dodge;
        _animator.SetBool(_hashRoll, true);
        while (currDur < _dodgeDur)
        {
            currDur += Time.deltaTime;
            transform.position += dodgeDir * dodgeSpeed;
            yield return null;
        }

        // �����ⳡ���� ó��
        _state.State = PlayerState.eState.Idle;
        _animator.SetBool(_hashRoll, false);
    }
}
