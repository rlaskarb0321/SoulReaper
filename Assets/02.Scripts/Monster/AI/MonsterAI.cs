using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public bool _isDef;
    [Header("Idle Delay")]
    [Tooltip("반드시 <= 0")]
    public float _randomMinScoutIdle;
    [Tooltip("반드시 > 0")]
    public float _randomMaxScoutIdle;
    [Space(10.0f)]
    public bool _isTargetConfirm; // 적 발견 여부값을 저장
    public Transform _targetTr;
    
    public enum eMonsterFSM { Idle, Patrol, Trace, Attack, Defense, Dead }
    public eMonsterFSM _fsm;
    public Vector3 _patrolPos;

    private int _playerSearchLayer;
    public bool _isSetPatrolPos;
    private float _combatIdleTime; // 전투 관련 대기할 시간값
    private float _scoutIdleTime; // 정찰 완료 후 대기할 시간값
    private float _currScoutIdle; // 현재 남아있는 정찰 대기시간값
    private MonsterStat _stat;
    private MonsterBase _monster;
    private NavMeshAgent _nav;
    private NavMeshPath _path;


    private void Awake()
    {
        _stat = this.GetComponent<MonsterBase>()._stat;
        _monster = GetComponent<MonsterBase>();
        _nav = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();
    }

    private void Start()
    {
        _playerSearchLayer = 1 << LayerMask.NameToLayer("PlayerTeam");
        _combatIdleTime = _stat.actDelay;
        _scoutIdleTime = _stat.actDelay;
        _currScoutIdle = _scoutIdleTime;
    }

    private void Update()
    {
        if (_fsm == eMonsterFSM.Dead)
        {
            return;
        }

        // 정찰지를 고름
        if (!_isSetPatrolPos)
        {
            _patrolPos = SetRandomPoint(transform.position, _patrolPos, _stat.traceDist * 0.5f);
            //_patrolPos = SetRandomPoint(transform.position, _patrolPos, _stat.traceDist * 0.5f);
            return;
        }

        // 선공옵션이 켜지기 전에는 정찰만
        if (!_stat.isInitiator)
        {
            Scout(_patrolPos);
            return;
        }

        if (_isTargetConfirm)
        {
            Combat();
        }
        else
        {
            Scout(_patrolPos);
        }
    }

    void Scout(Vector3 destination)
    {
        // 정찰하면서 목표(플레이어)가 도달할 수 있는 지역에있으면 추격시작
        Collider[] detectColls = Physics.OverlapSphere(transform.position, _stat.traceDist, _playerSearchLayer);
        // Collider[] detectColls = Physics.OverlapCapsule(, , _stat.traceDist, _playerSearchLayer);
        
        // 원본
        if (detectColls.Length >= 1)
        {
            _isTargetConfirm = true;
            _targetTr = detectColls[0].transform;
            _fsm = eMonsterFSM.Trace;
            return;
        }

        //// 2차수정
        //for (int i = 0; i < detectColls.Length; i++)
        //{
        //    _nav.destination = detectColls[0].gameObject.transform.position;
        //    if (_nav.pathStatus == NavMeshPathStatus.PathPartial)
        //    {
        //        print("x");
        //    }
        //    else
        //    {
        //        print("o");
        //    }
        //}

        // // 1차수정
        //for (int i = 0; i < detectColls.Length; i++)
        //{
        //    _nav.CalculatePath(detectColls[i].gameObject.transform.position, _path);
        //    _nav.SetPath(_path);
        //    if (_path.status == NavMeshPathStatus.PathPartial)
        //    {
        //    }
        //    else
        //    {
        //        print("o");
        //    }
        //}

        // 원본
        //if (detectColls.Length >= 1)
        //{
        //    _isTargetConfirm = true;
        //    _targetTr = detectColls[0].transform;
        //    _fsm = eMonsterFSM.Trace;
        //    return;
        //}

        switch (_fsm)
        {
            case eMonsterFSM.Idle:
                if (_scoutIdleTime == _currScoutIdle)
                {
                    _scoutIdleTime = _stat.actDelay + Random.Range(_randomMinScoutIdle, _randomMaxScoutIdle);
                    _currScoutIdle = _scoutIdleTime;
                }

                if (_currScoutIdle <= 0.0f)
                {
                    _isSetPatrolPos = false;
                    _currScoutIdle = _scoutIdleTime;
                    return;
                }

                if (!_monster._isIdle)
                {
                    _monster.Idle();
                }

                _currScoutIdle -= Time.deltaTime;
                break;

            case eMonsterFSM.Patrol:
                _patrolPos.y = transform.position.y;
                _monster.Move(_patrolPos, _stat.movSpeed);

                // 도착지점과의 거리에따라 idle 혹은 patrol 전환
                _fsm = Vector3.Distance(transform.position, _patrolPos) <= _monster._nav.stoppingDistance ?
                    eMonsterFSM.Idle : eMonsterFSM.Patrol;
                break;
        }

    }

    void Combat()
    {
        switch (_fsm)
        {
            case eMonsterFSM.Idle:
                if (_combatIdleTime <= 0.0f)
                {
                    _fsm = Vector3.Distance(transform.position, _targetTr.position) <= _stat.attakDist ?
                    eMonsterFSM.Attack : eMonsterFSM.Trace;
                    _combatIdleTime = _stat.actDelay;
                    return;
                }

                if (!_monster._isIdle)
                {
                    _monster.Idle();
                }

                _combatIdleTime -= Time.deltaTime;
                _monster.LookTarget(_targetTr.position);
                break;

            case eMonsterFSM.Trace:
                _monster.Move(_targetTr.position, _stat.movSpeed);
                _fsm = Vector3.Distance(transform.position, _targetTr.position) <= _stat.attakDist ?
                    eMonsterFSM.Attack : eMonsterFSM.Trace;
                break;

            case eMonsterFSM.Attack:
                if (_monster._isAtk)
                    return;

                _monster.Attack();
                break;

            case eMonsterFSM.Defense:
                break;
        }
    }

    Vector3 SetRandomPoint(Vector3 center, Vector3 destination, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            // 2차수정 코드
            //Vector3 randomPos = center + Random.insideUnitSphere * radius;
            //NavMeshHit hit;

            //_nav.destination = randomPos;
            //print(_path.corners.Length);

            //if (_path.status == NavMeshPathStatus.PathInvalid)
            //{
            //    continue;
            //}

            //if (NavMesh.SamplePosition(randomPos, out hit, 4.0f, NavMesh.AllAreas))
            //{
            //    destination = hit.position;
            //    _isSetPatrolPos = true;
            //    _fsm = eMonsterFSM.Patrol;
            //    return destination;
            //}

            // 1차수정 코드
            //Vector3 randomPos = center + Random.insideUnitSphere * radius;
            //NavMeshHit hit;

            //if (NavMesh.SamplePosition(randomPos, out hit, 4.0f, NavMesh.AllAreas))
            //{
            //    _nav.CalculatePath(hit.position, _path);
            //    if (_path.status == NavMeshPathStatus.PathInvalid)
            //    {
            //        destination = hit.position;
            //        _isSetPatrolPos = true;
            //        _fsm = eMonsterFSM.Patrol;
            //        return destination;
            //    }
            //}

            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;

            // 원래코드
            if (NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas))
            {
                destination = hit.position;
                _isSetPatrolPos = true;
                _fsm = eMonsterFSM.Patrol;
                return destination;
            }
        }

        return Vector3.zero;
    }
}
