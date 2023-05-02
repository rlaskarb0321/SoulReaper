using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 몬스터들의 다음 행동 실행을 위해 몬스터 뇌의 욕구를 설정하는 클래스
public class MonsterAI : MonoBehaviour
{
    // 몬스터가 하고자하는 욕구들의 종류
    public enum eMonsterDesires { Patrol, Idle, Trace, Attack, Defense, Delay, Dead }
    [SerializeField] private eMonsterDesires _monsterBrain;
    public eMonsterDesires MonsterBrain
    {
        get { return _monsterBrain; }
        set // MonsterBrain값을 변경하면서 각 상태의맞는 이동속도값으로 수정
        {
            if (value != eMonsterDesires.Attack && _monsterBase._isActing)
                _monsterBase._isActing = false;

            _monsterBrain = value;
            switch (value)
            {
                case eMonsterDesires.Patrol:
                    _monsterBase._nav.speed = _monsterBase._basicStat.patrolMovSpeed;
                    _monsterBase._movSpeed = _monsterBase._nav.speed;
                    break;
                case eMonsterDesires.Trace:
                    _monsterBase._nav.speed = _monsterBase._basicStat.traceMovSpeed;
                    _monsterBase._movSpeed = _monsterBase._nav.speed;
                    break;
                case eMonsterDesires.Defense:
                    _monsterBase._nav.speed = _monsterBase._basicStat.kitingMovSpeed;
                    _monsterBase._movSpeed = _monsterBase._nav.speed;
                    break;
            }
        }
    }
    [HideInInspector] public bool _isTargetSet;
    [HideInInspector] public Transform _target;
    [HideInInspector] public Vector3 _patrolPos;
    [Range(2.0f, 10.0f)] public float _idleTime; // 행동후 다음행동까지 기다리는 시간값
    [Range(0.0f, 1.0f)] public float _needDefenseHpPercentage; // 해당값 이하일때 방어(카이팅, 가드)가 필요하다고 생각하게되는 hp 퍼센티지
    [Range(5.0f, 10.0f)] public float _defenseEndure; // defense 무한지속을 방지하는 일정시간후에 자동으로 defense를 벗어나게해줄 값


    // Field
    Monster _monsterBase;
    NavMeshAgent _nav;
    float _originDelay;
    int _playerSearchLayer;
    float _currEndure;
    int _soulOrbSearchLayer;
    bool _isFindPatrolPos;
    [SerializeField] bool _needDefense;

    void Awake()
    {
        _monsterBase = GetComponent<Monster>();
        _nav = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        _playerSearchLayer = 1 << LayerMask.NameToLayer("PlayerTeam");
        _soulOrbSearchLayer = 1 << LayerMask.NameToLayer("SoulOrb");
        MonsterBrain = eMonsterDesires.Patrol;
        _originDelay = _monsterBase._basicStat.actDelay;
        _currEndure = _defenseEndure;
    }

    void Update()
    {
        if (MonsterBrain == eMonsterDesires.Dead)
            return;

        DetermineDesires();
    }

    // 몬스터가 다음에 행동하고자 할 행동을 실행하기위해 몬스터의 욕구를 결정
    void DetermineDesires()
    {
        #region 23/04/17 몬스터 Brain 작동방식 전환
        //Collider[] detectedColls;
        //float targetDist;

        //// 타겟을 감지하지 못했을경우에는 Patrol
        //if (!_isTargetSet)
        //{
        //    // 구체형 콜리더와 닿는것들중에 PlayerTeam이라는 레이어값을 가진 요소들만 배열에 추가
        //    detectedColls = Physics.OverlapSphere(transform.position, _monsterBase._basicStat._traceRadius, _playerTeamLayer);

        //    if (detectedColls.Length >= 1)
        //    {
        //        MonsterBrain = eMonsterDesires.Trace;
        //        _isTargetSet = true;
        //        _target = detectedColls[0].transform;
        //    }
        //    else
        //    {
        //        MonsterBrain = eMonsterDesires.Patrol;
        //    }
        //}

        //// 타겟을 감지한경우
        //else
        //{
        //    targetDist = Vector3.Distance(transform.position, _target.position);

        //    // 타겟과의 거리에따라 Attack || Trace
        //    if (targetDist <= _monsterBase._basicStat._attakableRadius)
        //        MonsterBrain = eMonsterDesires.Attack;
        //    else
        //        MonsterBrain = eMonsterDesires.Trace;

        //    // 타겟과의 거리에따라 Attack || Trace
        //    //if (_monsterBase._nav.remainingDistance > _monsterBase._basicStat._attakableRadius)
        //    //    MonsterBrain = eMonsterDesires.Trace;
        //    //else
        //    //    print("공격가능");
        //    //    //MonsterBrain = eMonsterDesires.Attack;
        //}
        #endregion 23/04/17 몬스터 Brain 작동방식 전환
        float targetDist;

        switch (MonsterBrain)
        {
            // 적을 발견하기전에는 정찰~휴식 반복
            case eMonsterDesires.Idle:
            case eMonsterDesires.Patrol:
                Collider[] detectColls = Physics.OverlapSphere(transform.position, _monsterBase._basicStat.traceRadius,
                    _playerSearchLayer);

                if (detectColls.Length >= 1)
                {
                    _target = detectColls[0].transform;
                    MonsterBrain = eMonsterDesires.Trace;
                    break;
                }

                if (!_isFindPatrolPos)
                {
                    _patrolPos = SetRandomPoint(transform.position, _patrolPos, _monsterBase._basicStat.traceRadius);
                }
                else
                {
                    if (!_nav.pathPending)
                    {
                        if (_nav.remainingDistance <= _nav.stoppingDistance)
                        {
                            if (!_nav.hasPath || _nav.velocity.sqrMagnitude == 0f)
                            {
                                StartCoroutine(IdlePatrol());
                            }
                        }
                        else
                        {
                            MonsterBrain = eMonsterDesires.Patrol;
                        }
                    }
                }
                break;

            // 적을발견했을때 내 공격사거리 안에 플레이어가 들어와있는지 여부에따라 추격~공격
            case eMonsterDesires.Attack:
            case eMonsterDesires.Trace:
                if (!_monsterBase._basicStat.isAttackFirst || _monsterBase._isActing)
                    return;

                targetDist = Vector3.Distance(transform.position, _target.position);
                MonsterBrain = DetermineAttackOrTrace(targetDist);
                break;

            // 공격 or 방어자세를 취한후 일정시간동안 Delay주기위한상태
            case eMonsterDesires.Delay:
                if (_monsterBase._currActDelay < 0.0f)
                {
                    // 딜레이값을 다 지낸후에 방어를할지 다시 추격or공격을 할 지 결정
                    targetDist = Vector3.Distance(transform.position, _target.position);

                    if (DetermineWhethereNeedDefense(targetDist, (int)_monsterBase._monsterType) &&
                        _monsterBase._currDefenseCool == _monsterBase._basicStat.defenseCoolTime)
                    {
                        MonsterBrain = eMonsterDesires.Defense;
                        StartCoroutine(CoolDownDefense());
                    }
                    else
                    {
                        MonsterBrain = DetermineAttackOrTrace(targetDist);
                        _monsterBase.StopNav(false);
                    }

                    _monsterBase._currActDelay = _originDelay;
                    return;
                }
                _monsterBase._currActDelay -= Time.deltaTime;
                break;

            case eMonsterDesires.Defense:
                if (_currEndure <= 0.0f)
                {
                    float newDelayValue = _monsterBase._currActDelay * 0.5f;
                    _monsterBase._currActDelay = newDelayValue;
                    MonsterBrain = eMonsterDesires.Delay;
                    _currEndure = _defenseEndure;
                    return;
                }

                _currEndure -= Time.deltaTime;
                break;
        }
    }

    Vector3 SetRandomPoint(Vector3 center, Vector3 destination, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                destination = hit.position;
                _isFindPatrolPos = true;
                MonsterBrain = eMonsterDesires.Patrol;
                return destination;
            }
        }

        return Vector3.zero;
    }

    IEnumerator IdlePatrol()
    {
        if (MonsterBrain == eMonsterDesires.Idle)
            yield break;

        float randomValue = Random.Range(-1.0f, 0.5f);
        WaitForSeconds waitSeconds = new WaitForSeconds(_idleTime + randomValue);
        MonsterBrain = eMonsterDesires.Idle;

        yield return waitSeconds;
        _isFindPatrolPos = false;
    }

    // 몬스터타입별로 상이한 방어모드돌입 조건
    public bool DetermineWhethereNeedDefense(float targetDist, int monsterType)
    {
        bool needDefense = false;

        switch ((Monster.eMonsterType)monsterType)
        {
            case Monster.eMonsterType.Melee:
                needDefense = _monsterBase._currHp / _monsterBase._basicStat.health < _needDefenseHpPercentage ? true : false;
                return needDefense;

            case Monster.eMonsterType.Range:
                needDefense = targetDist <= _monsterBase._basicStat.attakableRadius * 0.5f ? true : false;
                return needDefense;

            case Monster.eMonsterType.Charge:
                return needDefense;

            case Monster.eMonsterType.MeleeAndRange:
                return needDefense;

            default:
                return needDefense;
        }
    }

    // 공격or추격 결정
    eMonsterDesires DetermineAttackOrTrace(float targetDist)
    {
        if (targetDist <= _monsterBase._basicStat.attakableRadius)
            return eMonsterDesires.Attack;
        else
            return eMonsterDesires.Trace;
    }

    IEnumerator CoolDownDefense()
    {
        while (_monsterBase._currDefenseCool >= 0.0f)
        {
            _monsterBase._currDefenseCool -= Time.deltaTime;
            yield return null;
        }

        _monsterBase._currDefenseCool = _monsterBase._basicStat.defenseCoolTime;
    }
}