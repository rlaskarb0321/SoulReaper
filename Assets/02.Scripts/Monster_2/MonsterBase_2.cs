using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterStat_2
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

public abstract class MonsterBase_2 : MonoBehaviour
{
    [HideInInspector] public Rigidbody _rbody;
    [HideInInspector] public Animator _animator;
    [HideInInspector] public NavMeshAgent _nav;
    [HideInInspector] public MonsterAI_2 _brain;

    public bool _isAtk;
    public MonsterStat_2 _stat;
    [Space(10.0f)][Tooltip("0��° �ε����� �⺻ mat, 1��° �ε����� �ǰݽ� ���ٲ� mat")]
    public Material[] _materials;

    protected readonly int _hashMove = Animator.StringToHash("Move");
    protected readonly int _hashIdle = Animator.StringToHash("Idle");
    protected readonly int _hashAtk1 = Animator.StringToHash("Attack1");

    protected virtual void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _nav = GetComponent<NavMeshAgent>();
        _brain = GetComponent<MonsterAI_2>();
    }

    protected virtual void Start()
    {
        
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
    /// ������ hp�� 0�����϶� ����Ǵ� �Լ�
    /// </summary>
    public abstract void OnMonsterDie();

    /// <summary>
    /// ���Ͱ� ������ �ϱ����� ����Ǵ� �Լ�
    /// </summary>
    public abstract void Attack();
    #endregion Control Method
}
