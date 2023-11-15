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
    private GameObject _stoneHit;

    [SerializeField]
    private GameObject _target;

    [SerializeField]
    private float _blinkOffset;

    // �� ���� phase ���θ� �޾Ƴ��Ƶ� �ɵ�

    // Field
    private Animator _animator;
    private MonsterBase_1 _monsterBase;
    private readonly int _hashBlink = Animator.StringToHash("Blink Trigger");
    private readonly int _hashBlinkBack = Animator.StringToHash("Blink Back Pos");
    private readonly int _hashSliding = Animator.StringToHash("Sliding Trigger");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _monsterBase = GetComponent<MonsterBase_1>();
        _target = _monsterBase._target;
    }

    #region ��ũ

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

    #endregion ��ũ

    public void SummonMiniBoss()
    {
        print("�̴Ϻ��� ��ȯ ��");
    }

    public void DropKick()
    {
        print("��� ű ��");
    }

    public void SummonNormalMonster()
    {
        print("�븻 �� ��ȯ ��");
    }

    #region �����̵� ����

    public void Sliding()
    {
        _animator.SetTrigger(_hashSliding);
    }

    #endregion �����̵� ����

    public void Jump()
    {
        print("���� ���� ��");
    }

    public void Fist()
    {
        print("�ָ� ���� ��");
    }

    public void Push()
    {
        print("�б� ���� ��");
    }
}
