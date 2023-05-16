using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterStat_2
{
    public int health; // 체력
    public float rotSpeed; 
    public float movSpeed; 
    public bool isInitiator; // 선공or비선공여부

    [Space(10.0f)]
    public float traceDist; // 추격 가능 거리
    public float attakDist; // 공격 가능 거리
    public float actDelay; // 멍 때리는 시간
}

public abstract class MonsterBase_2 : MonoBehaviour
{
    [HideInInspector] public Rigidbody _rbody;
    [HideInInspector] public Animator _animator;
    [HideInInspector] public NavMeshAgent _nav;
    [HideInInspector] public MonsterAI_2 _brain;

    public bool _isAtk;
    public MonsterStat_2 _stat;
    [Space(10.0f)][Tooltip("0번째 인덱스는 기본 mat, 1번째 인덱스는 피격시 잠깐바뀔 mat")]
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
    /// 몬스터가 움직이는 방법에대한 함수이다. 
    /// 평범하게 움직이는 몬스터는 구현을 하지않아도되고, 브루들링처럼 특이한 방식으로 움직이는 몬스터는 구현을 해야한다.
    /// </summary>
    public virtual void MovingBehaviour() { }

    /// <summary>
    /// 몬스터가 움직이는 방법을 토대로 실제로 움직이게 해주는 함수
    /// </summary>
    public abstract void Move(Vector3 pos, float movSpeed);

    /// <summary>
    /// 타겟을 바라보게 해주는 함수
    /// </summary>
    /// <param name="target"></param>
    public abstract void LookTarget(Vector3 target);

    /// <summary>
    /// 몬스터 대기시키는 함수
    /// </summary>
    public abstract void Idle();

    /// <summary>
    /// 자신의 체력을 깎는 함수
    /// </summary>
    /// <param name="amount"></param>
    public abstract void DecreaseHp(float amount);

    /// <summary>
    /// 공격을 맞았을 때 이펙트관련 함수
    /// </summary>
    public abstract IEnumerator OnHitEffect();

    /// <summary>
    /// 몬스터의 hp가 0이하일때 실행되는 함수
    /// </summary>
    public abstract void OnMonsterDie();

    /// <summary>
    /// 몬스터가 공격을 하기위해 실행되는 함수
    /// </summary>
    public abstract void Attack();
    #endregion Control Method
}
