using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Move & Rotate")]
    public float _movSpeed;
    public float _rotSpeed;

    [Header("Dodge")]
    public float _dodgeDist;
    public float _dodgeMotionSpeed;
    public float _dodgeCoolDown;

    private Vector3 _dir;
    private Animator _animator;
    private Rigidbody _rbody;
    private bool _isDodge;
    private float _h;
    private float _v;

    readonly int _hashMove = Animator.StringToHash("isMove");
    readonly int _hashYVelocity = Animator.StringToHash("yVelocity");
    readonly int _hashRoll = Animator.StringToHash("isRoll");

    private void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        _h = Input.GetAxisRaw("Horizontal");
        _v = Input.GetAxisRaw("Vertical");
        _animator.SetBool(_hashMove, Input.anyKey && (_h != 0.0f || _v != 0.0f));

        if (_h != 0.0f || _v != 0.0f)
        {
            MovePlayer();
            RotatePlayer();
        }

        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isDodge)
        {
            _isDodge = true;
            StartCoroutine(Dodge());
        }
    }

    void MovePlayer()
    {
        _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        transform.position += _dir * _movSpeed * 0.065f;
    }

    void RotatePlayer()
    {
        if (_dir == Vector3.zero)
            return;

        Quaternion newRot = Quaternion.LookRotation(_dir);
        _rbody.rotation = Quaternion.Slerp(_rbody.rotation, newRot, _rotSpeed * 0.065f);
    }

    IEnumerator Dodge()
    {
        RaycastHit hit; // ����ĳ��Ʈ�߻� ������ ����
        Vector3 destination; // ������� ������ ������

        // �������� ��ü�� �������ִٸ� ���̸����� �浹�Ѱ��� ������������
        if (Physics.Raycast(
            transform.position, transform.forward, out hit,
            (transform.position + (transform.forward * _dodgeDist)).magnitude))
            destination = hit.point;
        else
            destination = transform.position + (transform.forward * _dodgeDist);
        _rbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // ���������� ���۽����ְ� �����°�ó�� ��ġ�� �Ű���
        _animator.SetBool(_hashRoll, _isDodge);
        while (_isDodge)
        {
            transform.position = Vector3.Lerp(transform.position, destination, _dodgeMotionSpeed);
            yield return null;
        }
    }

    // RollForward �ִϸ��̼� �����������ӿ� Delegate�� �޾Ƴ��� �޼ҵ�
    public void OutDodge()
    {
        // �����Ⱑ ���� �� ó��
        _rbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        _isDodge = false;
        _animator.SetBool(_hashRoll, _isDodge);
    }

    // ������ ���� �ִϸ��̼� Delegate �޼ҵ�
    public void Fall()
    {
        _isDodge = false;
    }
}
