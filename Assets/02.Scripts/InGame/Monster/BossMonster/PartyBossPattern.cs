using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ƽ ������ ���� ����� �����ϴ� �Լ�
/// </summary>
public class PartyBossPattern : MonoBehaviour
{
    [Header("=== Blink Particle ===")]
    [SerializeField]
    [Tooltip("��ũ �� �� & ��ũ �� Ŀ���� ������ ����Ʈ")]
    private GameObject _stoneHit;

    [SerializeField]
    [Tooltip("��ũ�� �ڿ��� ��Ÿ���� �÷��̾�� �̰��� �Ÿ�")]
    private float _blinkOffset;

    [Header("=== Jump Attack ===")]
    [SerializeField]
    [Tooltip("���� �Ŀ�")]
    private float _jumpForce;

    [SerializeField]
    [Tooltip("���� �� ü������ �� ���� ������ �Ǵ��ϱ� ���� �� �� ������ ��ġ")]
    private Transform _groundRayPos;

    [SerializeField]
    [Tooltip("�� �� ������ ����")]
    private float _rayDist;

    // �� ���� phase ���θ� �޾Ƴ��Ƶ� �ɵ�

    // Field
    private GameObject _target;
    private Animator _animator;
    private MonsterBase_1 _monsterBase;
    private Rigidbody _rbody;
    private readonly int _hashBlink = Animator.StringToHash("Blink Trigger");
    private readonly int _hashBlinkBack = Animator.StringToHash("Blink Back Pos");
    private readonly int _hashSliding = Animator.StringToHash("Sliding Trigger");
    private readonly int _hashJump = Animator.StringToHash("Jump Trigger");
    private readonly int _hashJumpEnd = Animator.StringToHash("Jump End");
    private readonly int _hashFist = Animator.StringToHash("Fist Trigger");
    private readonly int _hashPush = Animator.StringToHash("Push Trigger");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _monsterBase = GetComponent<MonsterBase_1>();
        _rbody = GetComponent<Rigidbody>();
        _target = _monsterBase._target;
    }

    private void Update()
    {
        JudgeGrounded();
    }

    #region ���ݿ� ������ �ʿ��� �� ����ϴ� �������� �޼���


    public void AddRush(AnimationEvent myEvent)
    {
        int dist = myEvent.intParameter;
        float time = myEvent.floatParameter;
        Vector3 target = transform.position + transform.forward * dist;
        StartCoroutine(RushToTarget(target, time));
    }

    private IEnumerator RushToTarget(Vector3 target, float time)
    {
        Vector3 refVector = Vector3.zero;
        while (Vector3.Distance(transform.position, target) >= 0.1f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target, ref refVector, time); // ��ũ ��ġ�� 0.08, �����̵��� ���� ����
            yield return null;
        }

        transform.position = target;
    }

    #endregion ���ݿ� ������ �ʿ��� �� ����ϴ� �������� �޼���

    #region �Ϲ����� ������ �� ������ ��ų��

    #region ��ũ ����

    // ��ũ �� �����鼭 �ָ� ��� �� �� �������Ѿ� ��

    public void Blink()
    {
        _animator.SetTrigger(_hashBlink);
    }

    public void BlinkAppear()
    {
        _animator.SetTrigger(_hashBlinkBack);
    }

    /// <summary>
    /// ��ũ �� �� ������ ��ƼŬ ����Ʈ�� ���ִ� �ִϸ��̼� ��������Ʈ
    /// </summary>
    public void ActiveBlinkParticle()
    {
        _stoneHit.transform.position = transform.position + Vector3.up * 2.0f;
        _stoneHit.SetActive(true);
    }

    public void MoveToTargetBehind()
    {
        Vector3 blinkPos = _target.transform.position + (_target.transform.forward * -_blinkOffset);
        transform.forward = (_target.transform.position - blinkPos).normalized;
        transform.position = blinkPos;
    }

    #endregion ��ũ ����

    #region �̴� ���� ��ȯ�ϱ�

    public void SummonMiniBoss()
    {
        print("�̴Ϻ��� ��ȯ ��");
    }

    #endregion �̴� ���� ��ȯ�ϱ�

    #region ���ű ����

    public void DropKick()
    {
        print("��� ű ��");
    }

    #endregion ���ű ����

    #region �Ϲ� ���� ��ȯ�ϱ�

    public void SummonNormalMonster()
    {
        print("�븻 �� ��ȯ ��");
    }

    #endregion �Ϲ� ���� ��ȯ�ϱ�

    #region �����̵� ����

    // ������ ���ư��� �����̵� �����ϰ� �ؾ� ��

    public void Sliding()
    {
        _animator.SetTrigger(_hashSliding);
    }

    #endregion �����̵� ����

    #region ���� ����

    // ���� �� �������� ���� �˷��ִ� �׸��� ������Ʈ��
    // �������� ������ �̵��ϴ� ����� �ʿ�
    // ������ �ӵ��� ������

    public void Jump()
    {
        _animator.ResetTrigger(_hashJumpEnd);
        _animator.SetTrigger(_hashJump);
    }

    /// <summary>
    /// JumpStart Anim�� ���� �����ӿ� ���� delegate �Լ�
    /// </summary>
    public void JumpStart()
    {
        _rbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// ���� �� or ���� ���� ���·� ����� �޼���
    /// </summary>
    /// <param name="value"></param>
    public void SetJumpState(int value)
    {
        bool setValue = value == 1 ? true : false;
        _groundRayPos.gameObject.SetActive(setValue);
    }

    /// <summary>
    /// Update ���� ������ ü������ �� ���� ���� ���θ� �Ǵ��ϴ� �޼���
    /// </summary>
    private void JudgeGrounded()
    {
        if (!_groundRayPos.gameObject.activeSelf)
            return;

        if (Physics.Raycast(_groundRayPos.position, -_groundRayPos.up, _rayDist, 1 << LayerMask.NameToLayer("Ground")))
        {
            _animator.SetTrigger(_hashJumpEnd);
        }
    }

    #endregion ���� ����

    #region �ָ� ��� ����

    public void Fist()
    {
        _animator.SetTrigger(_hashFist);
    }

    #endregion �ָ� ��� ����

    #region �б� ����

    public void Push()
    {
        _animator.SetTrigger(_hashPush);
    }

    #endregion �б� ����

    #endregion �Ϲ����� ������ �� ������ ��ų��

    #region ��ģ ������ �� ������ ��ų��

    #region �޽� �ϱ�

    public void Rest()
    {
        print("���ƾ� ����");
    }

    #endregion �޽� �ϱ�

    #region ���� ġ��

    public void Run()
    {
        print("������~");
    }

    #endregion �޽� �ϱ�

    #endregion ��ģ ������ �� ������ ��ų��
}
