using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("=== Idle Delay ===")]
    [Tooltip("�ݵ�� <= 0")]
    public float _randomMinScoutIdle;
    [Tooltip("�ݵ�� > 0")]
    public float _randomMaxScoutIdle;

    [Header("=== Target & Scout ===")]
    public bool _isTargetConfirm; // �� �߰� ���ΰ��� ����
    public Transform _targetTr;
    public bool _isSetPatrolPos;
    
    [Header("=== State ===")]
    public bool _isDef;
    public enum eMonsterFSM { Idle, Patrol, Trace, Attack, Defense, Dead }
    public eMonsterFSM _fsm;

    private Vector3 _patrolPos;
    private int _playerSearchLayer;
    private float _combatIdleTime; // ���� ���� ����� �ð���
    private float _scoutIdleTime; // ���� �Ϸ� �� ����� �ð���
    private float _currScoutIdle; // ���� �����ִ� ���� ���ð���
    private MonsterStat _stat;
    private MonsterBase _monster;


    private void Awake()
    {
        _stat = this.GetComponent<MonsterBase>()._stat;
        _monster = GetComponent<MonsterBase>();
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
            return;
        }

        // �����ɼ��� ������ ������ ������
        if (!_stat.isInitiator)
        {
            Scout(_patrolPos);
            return;
        }

        // �� �߰�������, �߰����� �� ������
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
        // �����ϸ鼭 ��ǥ(�÷��̾�)�� ���� �����ȿ� ���� �� �÷��̾� ���� �����ϴ� �뵵�� ����
        Collider[] detectColls = Physics.OverlapSphere(transform.position, _stat.traceDist, _playerSearchLayer);
        
        // ����
        if (detectColls.Length >= 1)
        {
            _isTargetConfirm = true;
            _targetTr = detectColls[0].transform;
            _fsm = eMonsterFSM.Trace;
            return;
        }

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
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;

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
