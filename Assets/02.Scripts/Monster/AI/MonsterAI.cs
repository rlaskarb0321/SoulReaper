using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public bool _isDef;
    [Header("Idle Delay")]
    [Tooltip("�ݵ�� <= 0")]
    public float _randomMinScoutIdle;
    [Tooltip("�ݵ�� > 0")]
    public float _randomMaxScoutIdle;
    [Space(10.0f)]
    public bool _isTargetConfirm; // �� �߰� ���ΰ��� ����
    public Transform _targetTr;
    
    public enum eMonsterFSM { Idle, Patrol, Trace, Attack, Defense, Dead }
    public eMonsterFSM _fsm;
    public Vector3 _patrolPos;

    private int _playerSearchLayer;
    public bool _isSetPatrolPos;
    private float _combatIdleTime; // ���� ���� ����� �ð���
    private float _scoutIdleTime; // ���� �Ϸ� �� ����� �ð���
    private float _currScoutIdle; // ���� �����ִ� ���� ���ð���
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

        // �������� ��
        if (!_isSetPatrolPos)
        {
            _patrolPos = SetRandomPoint(transform.position, _patrolPos, _stat.traceDist * 0.5f);
            //_patrolPos = SetRandomPoint(transform.position, _patrolPos, _stat.traceDist * 0.5f);
            return;
        }

        // �����ɼ��� ������ ������ ������
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
        // �����ϸ鼭 ��ǥ(�÷��̾�)�� ������ �� �ִ� ������������ �߰ݽ���
        Collider[] detectColls = Physics.OverlapSphere(transform.position, _stat.traceDist, _playerSearchLayer);
        // Collider[] detectColls = Physics.OverlapCapsule(, , _stat.traceDist, _playerSearchLayer);
        
        // ����
        if (detectColls.Length >= 1)
        {
            _isTargetConfirm = true;
            _targetTr = detectColls[0].transform;
            _fsm = eMonsterFSM.Trace;
            return;
        }

        //// 2������
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

        // // 1������
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

        // ����
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

                // ������������ �Ÿ������� idle Ȥ�� patrol ��ȯ
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
            // 2������ �ڵ�
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

            // 1������ �ڵ�
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

            // �����ڵ�
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
