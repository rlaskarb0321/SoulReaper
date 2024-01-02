using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterStat
{
    public int health; // 체력
    public int damage;
    public int soul;
    public float rotSpeed; 
    public float movSpeed; 
    public bool isInitiator; // 선공or비선공여부

    [Space(10.0f)]
    public float traceDist; // 추격 가능 거리
    public float attakDist; // 공격 가능 거리
    public float actDelay; // 멍 때리는 시간
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
    public Material[] _hitMats; // 0번 인덱스는 기본 mat, 1번 인덱스는 피격시 잠깐바뀔 mat
    public float _bodyBuryTime; // 시체처리연출의 시작까지 기다릴 값
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
    /// 몬스터가 움직이는 방법을 토대로 실제로 움직이게 해주는 함수
    /// </summary>
    public abstract void Move(Vector3 pos, float movSpeed);

    /// <summary>
    /// 타겟을 바라보게 해주는 함수
    /// </summary>
    //public abstract void LookTarget(Vector3 target);

    /// <summary>
    /// 몬스터 대기시키는 함수
    /// </summary>
    public abstract void Idle();

    /// <summary>
    /// 자신의 체력을 깎는 함수
    /// </summary>
    public abstract void DecreaseHp(float amount, Vector3 hitPos);

    /// <summary>
    /// 공격을 맞았을 때 이펙트관련 함수
    /// </summary>
    public abstract IEnumerator OnHitEvent();

    /// <summary>
    /// 몬스터를 죽게 해주고 관련변수 초기화해주는 함수
    /// </summary>
    protected abstract void Dead();

    /// <summary>
    /// 몬스터가 죽었을때 연출 관련 함수
    /// </summary>
    protected abstract IEnumerator OnMonsterDie();

    /// <summary>
    /// 몬스터가 공격을 하기위해 실행되는 함수
    /// </summary>
    public abstract void Attack();

    public virtual void LookTarget(Vector3 target, float multiple = 1.0f) { }

    #endregion Control Method
}
