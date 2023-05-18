using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterStat
{
    public int health; // ü��
    public float rotSpeed; 
    public float movSpeed; 
    public bool isInitiator; // ����or�񼱰�����

    [Space(10.0f)]
    public float traceDist; // �߰� ���� �Ÿ�
    public float attakDist; // ���� ���� �Ÿ�
    public float actDelay; // �� ������ �ð�
}

public abstract class MonsterBase : MonoBehaviour
{
    [HideInInspector] public Rigidbody _rbody;
    [HideInInspector] public Animator _animator;
    [HideInInspector] public NavMeshAgent _nav;
    [HideInInspector] public MonsterAI _brain;
    [HideInInspector] public SkinnedMeshRenderer _mesh;

    public MonsterStat _stat;
    public float _currHp;
    public bool _isAtk;
    public bool _isIdle;
    [Space(10.0f)]
    public Material[] _hitMats; // 0�� �ε����� �⺻ mat, 1�� �ε����� �ǰݽ� ���ٲ� mat
    public Material[] _deadMat; // 0�� �ε����� �⺻ Opaque Mat, 1�� �ε����� Fade Mat
    public float _bodyBuryTime; // ��üó�������� ���۱��� ��ٸ� ��

    protected readonly int _hashMove = Animator.StringToHash("Move");
    protected readonly int _hashIdle = Animator.StringToHash("Idle");
    protected readonly int _hashDead = Animator.StringToHash("Dead");

    protected virtual void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _nav = GetComponent<NavMeshAgent>();
        _brain = GetComponent<MonsterAI>();
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    protected virtual void Start()
    {
        _currHp = _stat.health;
    }

    #region Control Method
    /// <summary>
    /// ���Ͱ� �����̴� ��������� �Լ��̴�. 
    /// ����ϰ� �����̴� ���ʹ� ������ �����ʾƵ��ǰ�, ���鸵ó�� Ư���� ������� �����̴� ���ʹ� ������ �ؾ��Ѵ�.
    /// </summary>
    public virtual void MovingBehaviour() { }

    /// <summary>
    /// ���Ͱ� �����̴� ����� ���� ������ �����̰� ���ִ� �Լ�
    /// </summary>
    public abstract void Move(Vector3 pos, float movSpeed);

    /// <summary>
    /// Ÿ���� �ٶ󺸰� ���ִ� �Լ�
    /// </summary>
    /// <param name="target"></param>
    public abstract void LookTarget(Vector3 target);

    /// <summary>
    /// ���� ����Ű�� �Լ�
    /// </summary>
    public abstract void Idle();

    /// <summary>
    /// �ڽ��� ü���� ��� �Լ�
    /// </summary>
    /// <param name="amount"></param>
    public abstract void DecreaseHp(float amount);

    /// <summary>
    /// ������ �¾��� �� ����Ʈ���� �Լ�
    /// </summary>
    public abstract IEnumerator OnHitEffect();

    /// <summary>
    /// ���͸� �װ� ���ְ� ���ú��� �ʱ�ȭ���ִ� �Լ�
    /// </summary>
    protected abstract void Dead();

    /// <summary>
    /// ���Ͱ� �׾����� ���� ���� �Լ�
    /// </summary>
    protected virtual IEnumerator OnMonsterDie() { yield return null; }

    /// <summary>
    /// ���Ͱ� ������ �ϱ����� ����Ǵ� �Լ�
    /// </summary>
    public abstract void Attack();
    #endregion Control Method
}
