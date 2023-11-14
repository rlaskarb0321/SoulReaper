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

    // Field
    private Animator _animator;
    private readonly int _hashBlink = Animator.StringToHash("Blink Trigger");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    #region ��ũ

    public void Blink()
    {
        _animator.SetTrigger(_hashBlink);
        print("��ũ ��");
    }

    /// <summary>
    /// ��ũ �� �� ������ ��ƼŬ ����Ʈ�� ���ִ� �ִϸ��̼� ��������Ʈ
    /// </summary>
    public void ActiveBlinkParticle() => _stoneHit.SetActive(true);

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

    public void Sliding()
    {
        print("�����̵� ���� ��");
    }

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
