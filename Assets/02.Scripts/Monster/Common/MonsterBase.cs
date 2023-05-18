using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterStat
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
    public Material[] _hitMats; // 0번 인덱스는 기본 mat, 1번 인덱스는 피격시 잠깐바뀔 mat
    public Material[] _deadMat; // 0번 인덱스는 기본 Opaque Mat, 1번 인덱스는 Fade Mat
    public float _bodyBuryTime; // 시체처리연출의 시작까지 기다릴 값

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
    /// 몬스터를 죽게 해주고 관련변수 초기화해주는 함수
    /// </summary>
    protected abstract void Dead();

    /// <summary>
    /// 몬스터가 죽었을때 연출 관련 함수
    /// </summary>
    protected virtual IEnumerator OnMonsterDie() { yield return null; }

    /// <summary>
    /// 몬스터가 공격을 하기위해 실행되는 함수
    /// </summary>
    public abstract void Attack();
    #endregion Control Method
}
