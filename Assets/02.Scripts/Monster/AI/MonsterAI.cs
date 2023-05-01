using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ���͵��� ���� �ൿ ������ ���� ���� ���� �屸�� �����ϴ� Ŭ����
public class MonsterAI : MonoBehaviour
{
    // ���Ͱ� �ϰ����ϴ� �屸���� ����
    public enum eMonsterDesires { Patrol, Idle, Trace, Attack, Defense, Dead }
    [SerializeField] private eMonsterDesires _monsterBrain;
    public eMonsterDesires MonsterBrain
    {
        get { return _monsterBrain; }
        set // MonsterBrain���� �����ϸ鼭 �� �����Ǹ´� �̵��ӵ������� ����
        {
            if (value != eMonsterDesires.Attack && _monsterBase._isActing)
                _monsterBase._isActing = false;

            _monsterBrain = value;
            switch (value)
            {
                case eMonsterDesires.Patrol:
                    _monsterBase._nav.speed = _monsterBase._basicStat._patrolMovSpeed;
                    break;
                case eMonsterDesires.Trace:
                    _monsterBase._nav.speed = _monsterBase._basicStat._traceMovSpeed;
                    break;
                case eMonsterDesires.Attack:
                    break;
                case eMonsterDesires.Defense:
                    _monsterBase._nav.speed = _monsterBase._basicStat._kitingMovSpeed;
                    break;
            }
        }
    }
    [HideInInspector] public bool _isTargetSet;
    [HideInInspector] public Transform _target;
    [HideInInspector] public Vector3 _patrolPos;
    [Range(2.0f, 10.0f)] public float _idleTime; // �ൿ�� �����ൿ���� ��ٸ��� �ð���
    [Range(0.0f, 1.0f)] public float _needDefenseHpPercentage; // �ش簪 �����϶� ���(ī����, ����)�� �ʿ��ϴٰ� �����ϰԵǴ� hp �ۼ�Ƽ��


    // Field
    Monster _monsterBase;
    NavMeshAgent _nav;
    int _playerSearchLayer;
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
    }

    void Update()
    {
        if (MonsterBrain == eMonsterDesires.Dead)
            return;

        DetermineDesires();
    }

    // ���Ͱ� ������ �ൿ�ϰ��� �� �ൿ�� �����ϱ����� ������ �屸�� ����
    // �屸������ �����ؾ��ϴ°� : ���� ��������, ���� ����ü�°�, Ÿ ������ ��ȥ�� �ʵ忡�ִ���
    void DetermineDesires()
    {
        #region 23/04/17 ���� Brain �۵���� ��ȯ
        //Collider[] detectedColls;
        //float targetDist;

        //// Ÿ���� �������� ��������쿡�� Patrol
        //if (!_isTargetSet)
        //{
        //    // ��ü�� �ݸ����� ��°͵��߿� PlayerTeam�̶�� ���̾�� ���� ��ҵ鸸 �迭�� �߰�
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

        //// Ÿ���� �����Ѱ��
        //else
        //{
        //    targetDist = Vector3.Distance(transform.position, _target.position);

        //    // Ÿ�ٰ��� �Ÿ������� Attack || Trace
        //    if (targetDist <= _monsterBase._basicStat._attakableRadius)
        //        MonsterBrain = eMonsterDesires.Attack;
        //    else
        //        MonsterBrain = eMonsterDesires.Trace;

        //    // Ÿ�ٰ��� �Ÿ������� Attack || Trace
        //    //if (_monsterBase._nav.remainingDistance > _monsterBase._basicStat._attakableRadius)
        //    //    MonsterBrain = eMonsterDesires.Trace;
        //    //else
        //    //    print("���ݰ���");
        //    //    //MonsterBrain = eMonsterDesires.Attack;
        //}
        #endregion 23/04/17 ���� Brain �۵���� ��ȯ
        float targetDist;

        switch (MonsterBrain)
        {
            case eMonsterDesires.Idle:
            case eMonsterDesires.Patrol:
                Collider[] detectColls = Physics.OverlapSphere(transform.position, _monsterBase._basicStat._traceRadius,
                    _playerSearchLayer);

                if (detectColls.Length >= 1)
                {
                    MonsterBrain = eMonsterDesires.Trace;
                    _target = detectColls[0].transform;
                }
                else
                {
                    if (!_isFindPatrolPos)
                    {
                        if (SetRandomPoint(transform.position, out _patrolPos, _monsterBase._basicStat._traceRadius))
                        {
                            _isFindPatrolPos = true;
                            MonsterBrain = eMonsterDesires.Patrol;
                        }
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
                }
                break;

            case eMonsterDesires.Attack:
            case eMonsterDesires.Trace:
                if (!_monsterBase._basicStat._isAttackFirst)
                    return;
                if (_monsterBase._isActing)
                    return;

                targetDist = Vector3.Distance(transform.position, _target.position);
                if (targetDist <= _monsterBase._basicStat._attakableRadius)
                    MonsterBrain = eMonsterDesires.Attack;
                else
                    MonsterBrain = eMonsterDesires.Trace;
                break;

            case eMonsterDesires.Defense:
                targetDist = Vector3.Distance(transform.position, _target.position);
                if (!DetermineWhethereNeedDefense(targetDist, (int)_monsterBase._monsterType))
                {
                    MonsterBrain = eMonsterDesires.Trace;
                }
                
                break;
            case eMonsterDesires.Dead:
                return;
        }
    }

    bool SetRandomPoint(Vector3 center, out Vector3 destination, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                destination = hit.position;
                return true;
            }
        }
        destination = Vector3.zero;
        return false;
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

    // ����Ÿ�Ժ��� ������ ����嵹�� ����
    public bool DetermineWhethereNeedDefense(float targetDist, int monsterType)
    {
        bool needDefense = false;

        switch ((Monster.eMonsterType)monsterType)
        {
            case Monster.eMonsterType.Melee:
                needDefense = _monsterBase._currHp / _monsterBase._basicStat._health < _needDefenseHpPercentage ? true : false;
                return needDefense;

            case Monster.eMonsterType.Range:
                needDefense = targetDist <= _monsterBase._basicStat._attakableRadius * 0.5f ? true : false;
                return needDefense;

            case Monster.eMonsterType.Charge:
                return needDefense;

            case Monster.eMonsterType.MeleeAndRange:
                return needDefense;

            default:
                return needDefense;
        }
    }
}