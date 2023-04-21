using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterBasicStat
{
    public int _health; // 체력
    public bool _isAttackFirst; // 선공or비선공여부
    public float _traceRadius; // 추격을 인지하는 범위
    public float _attakableRadius; // 공격사정거리
    public float _actDelay; // 몬스터의 다음행동까지 걸리게할 시간값

    public float _kitingMovSpeed;
    public float _patrolMovSpeed;
    public float _traceMovSpeed;
    public float _retreatMovSpeed;
}

/// <summary>
/// 모든 몬스터들이 가져야하는 기본요소
/// </summary>
public class Monster : MonoBehaviour
{
    [Header("Basic Stat")]
    //[Tooltip("몬스터의 현재 상태를 나타냄")]
    // [HideInInspector] public eMonsterState _state;
    
    [Tooltip("몬스터의 단계, 계급")]
    public eMonsterLevel _level;
    
    [Tooltip("몬스터들 기본적 스텟 요소")]
    public MonsterBasicStat _basicStat;

    [Tooltip("몬스터의 공격 타입")]
    public eMonsterType _monsterType;

    // public enum eMonsterState { Patrol, Idle, Trace, Attack, Acting, Dead, }
    public enum eMonsterType { Melee, Range, Charge, MeleeAndRange, }
    public enum eMonsterLevel { Normal, Elite, MiddleBoss, Boss, }

    // [HideInInspector] public MonsterAI _monsterAI;
    [HideInInspector] public PlayerCombat _playerCombat;
    [HideInInspector] public NavMeshAgent _nav;
    [HideInInspector] public MonsterThink _brain;
    [HideInInspector] public Animator _animator;

    public float _currHp;
    public bool _isActing;
    public float _movSpeed;
    public WaitForSeconds _ws;

    public virtual void DecreaseHp(float amount)
    {

    }

    public virtual void Dead()
    {

    }

    public virtual IEnumerator DoAttack()
    {
        yield return null;
    }

    public virtual void ExecuteAttack()
    {

    }

    // 몬스터가 플레이어를 바라보게 해줌
    public IEnumerator LookTarget()
    {
        Vector3 myPos = transform.position;
        Vector3 targetPos = _brain._target.position;
        float angle;

        targetPos.y = 0.0f;
        myPos.y = 0.0f;
        angle = Quaternion.FromToRotation(transform.forward, (targetPos - myPos).normalized).eulerAngles.y;
        
        // 몬스터의 transform.forward 기준으로 플레이어가 왼쪽각도에있으면 왼쪽으로 돌게하기 위함
        if (angle > 180.0f)
            angle = -(360 - angle);

        while (Mathf.Abs(Quaternion.FromToRotation(transform.forward, (targetPos - myPos).normalized).eulerAngles.y) >= 1.0f)
        {
            transform.eulerAngles = new Vector3(0.0f,
                Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + (angle - 1.0f), 1.1f * Time.deltaTime), 0.0f);
            yield return null;
        }
    }

    // 몬스터가 타겟을 쫓아가게 함
    public void TraceTarget()
    {
        _nav.isStopped = false;

        if (!_nav.pathPending)
            _nav.SetDestination(_brain._target.position);
    }
}
