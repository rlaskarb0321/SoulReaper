using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterStat
{
    public int health; // ü��
    public int damage;
    public int soul;
    public float rotSpeed; 
    public float movSpeed; 
    public bool isInitiator; // ����or�񼱰�����

    [Space(10.0f)]
    public float traceDist; // �߰� ���� �Ÿ�
    public float attakDist; // ���� ���� �Ÿ�
    public float actDelay; // �� ������ �ð�
}

public interface INormalMonster
{
    public IEnumerator KnockBack(Vector3 hitDir);
}

public abstract class MonsterBaseLegacy : MonoBehaviour
{
    [HideInInspector] public Animator _animator;
    [HideInInspector] public NavMeshAgent _nav;
    [HideInInspector] public SkinnedMeshRenderer _mesh;

    [Header("=== Total Stat ===")]
    public MonsterStat _stat;
    public INormalMonster _normalMonster;

    [Header("=== Curr Stat ===")]
    public float _currHp;
    public bool _isAtk;
    public bool _isIdle;

    [Header("=== Hit & Dead ===")]
    public Material[] _hitMats; // 0�� �ε����� �⺻ mat, 1�� �ε����� �ǰݽ� ���ٲ� mat
    public float _bodyBuryTime; // ��üó�������� ���۱��� ��ٸ� ��
    public Material _deadMat;

    [Header("=== Raid Monster ===")]
    public WaveMonster _waveMonster;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _nav = GetComponent<NavMeshAgent>();
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    protected virtual void Start()
    {
        _currHp = _stat.health;
    }

    #region Control Method
    /// <summary>
    /// ���Ͱ� �����̴� ����� ���� ������ �����̰� ���ִ� �Լ�
    /// </summary>
    public abstract void Move(Vector3 pos, float movSpeed);

    /// <summary>
    /// Ÿ���� �ٶ󺸰� ���ִ� �Լ�
    /// </summary>
    //public abstract void LookTarget(Vector3 target);

    /// <summary>
    /// ���� ����Ű�� �Լ�
    /// </summary>
    public abstract void Idle();

    /// <summary>
    /// �ڽ��� ü���� ��� �Լ�
    /// </summary>
    public abstract void DecreaseHp(float amount, Vector3 hitPos);

    /// <summary>
    /// ������ �¾��� �� ����Ʈ���� �Լ�
    /// </summary>
    public abstract IEnumerator OnHitEvent();

    /// <summary>
    /// ���͸� �װ� ���ְ� ���ú��� �ʱ�ȭ���ִ� �Լ�
    /// </summary>
    protected abstract void Dead();

    /// <summary>
    /// ���Ͱ� �׾����� ���� ���� �Լ�
    /// </summary>
    protected abstract IEnumerator OnMonsterDie();

    /// <summary>
    /// ���Ͱ� ������ �ϱ����� ����Ǵ� �Լ�
    /// </summary>
    public abstract void Attack();

    public virtual void LookTarget(Vector3 target, float multiple = 1.0f) { }

    #endregion Control Method
}
