using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SentryMonster : MonsterType
{
    [Header("=== Sentry ===")]
    public Transform _eyePos;
    [SerializeField] private bool _isSetPatrolPos;
    [SerializeField] private float _missTargetMulti;
    [SerializeField] private float _idleTime;

    [Header("=== MonsterBase ===")]
    public MonsterBase_1 _monsterBase;

    private Vector3 _movPos;
    private float _movSpeed;
    private float _originIdleTime;
    private float _missTargetDist;

    private void Start()
    {
        _missTargetDist = _monsterBase._stat.traceDist * _missTargetMulti;
        _originIdleTime = _idleTime;
    }

    private void Update()
    {
        if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead)
            return;

        switch (_monsterBase._state)
        {
            case MonsterBase_1.eMonsterState.Idle:
                Idle();
                break;

            case MonsterBase_1.eMonsterState.Scout:
                Scout();
                break;

            case MonsterBase_1.eMonsterState.Trace:
                Trace();
                break;
        }
    }

    /// <summary>
    /// �������� ������ ��, �����ð� ��ٸ�
    /// </summary>
    private void Idle()
    {
        _monsterBase._target = SearchTarget(_monsterBase._stat.traceDist);
        if (_monsterBase._target != null)
        {
            _isSetPatrolPos = false;
            _idleTime = _originIdleTime;
            _monsterBase._state = MonsterBase_1.eMonsterState.Trace;
        }

        if (_idleTime <= 0.0f)
        {
            _isSetPatrolPos = false;
            _idleTime = _originIdleTime;
            _monsterBase._state = MonsterBase_1.eMonsterState.Scout;
            return;
        }

        _monsterBase._animator.SetBool(_monsterBase._hashMove, false);
        _idleTime -= Time.deltaTime;
    }

    /// <summary>
    /// Ÿ���� �Ѵ� �޼���, �ʹ� �־����� Ÿ���� ��ģ��.
    /// </summary>
    public override void Trace()
    {
        float distance = Vector3.Distance(transform.position, _monsterBase._target.transform.position);

        if (distance >= _missTargetDist || _monsterBase.IsPathPartial(_monsterBase._target.transform.position))
        {
            _monsterBase._target = null;
            _isSetPatrolPos = false;
            _monsterBase._state = MonsterBase_1.eMonsterState.Scout;
            return;
        }

        if (distance <= _monsterBase._stat.attakDist)
        {
            _monsterBase._state = MonsterBase_1.eMonsterState.Attack;
            return;
        }

        _movPos = _monsterBase._target.transform.position;
        _movSpeed = _monsterBase._stat.movSpeed;
        _monsterBase.Move(_movPos, _movSpeed);
    }

    /// <summary>
    /// ������ �ϵ��� �ϴ� �޼���
    /// </summary>
    private void Scout()
    {
        _monsterBase._target = SearchTarget(_monsterBase._stat.traceDist);

        if (_monsterBase._target != null)
        {
            _monsterBase._state = MonsterBase_1.eMonsterState.Trace;
            return;
        }

        if (!_isSetPatrolPos)
        {
            _movPos = SetRandomScout(transform.position, _movPos, _monsterBase._stat.traceDist);
            if (_monsterBase.IsPathPartial(_movPos))
                return;

            _isSetPatrolPos = true;
        }

        // ������ ���� Ȯ��
        float distance = Vector3.Distance(transform.position, _movPos);
        if (distance <= _monsterBase._nav.stoppingDistance + _monsterBase._nav.baseOffset)
        {
            _monsterBase._state = MonsterBase_1.eMonsterState.Idle;
            return;
        }

        _movSpeed = _monsterBase._stat.movSpeed * 0.5f;
        _monsterBase.Move(_movPos, _movSpeed);
    }

    public override GameObject SearchTarget(float searchRange)
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, searchRange, 1 << LayerMask.NameToLayer("PlayerTeam"));
        return DetectTarget(colls);
    }

    /// <summary>
    /// �ڽ� ������ ���� ����Ʈ�� ��ȯ
    /// </summary>
    /// <param name="center">�ֺ��� Ž���� ������ ��ġ</param>
    /// <param name="destination">��ǥ ��ǥ</param>
    /// <param name="radius">Ž�� ����</param>
    /// <returns>Ž�� ������ ��ǥ ��ġ, ���н� Vector3.zero</returns>
    private Vector3 SetRandomScout(Vector3 center, Vector3 destination, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas))
            {
                destination = hit.position;
                return destination;
            }
        }

        return Vector3.zero;
    }

    /// <summary>
    /// ���� �ݸ��� �����ȿ��� �÷��̾ Ž���ǰ�, �ڽŰ� ����� ��ġ�� �ִ��� & ��ü�� ���������ʾҴ��� �˻�
    /// </summary>
    /// <param name="colls">�˻��� �ݸ��� �迭</param>
    /// <returns>�÷��̾ Ž���Ǹ� �÷��̾ �׷��������� null �� ��ȯ</returns>
    private GameObject DetectTarget(Collider[] colls)
    {
        if (colls.Length < 1)
            return null;

        // �ڽŰ� ����� ���̿��ִ���, Ÿ���� �繰�� �������ִ��� ���� �Ǵ� �� Ÿ�� ����
        Vector3 targetVector = colls[0].transform.position - _eyePos.position;
        float distance = targetVector.magnitude;
        Vector3 dir = new Vector3(targetVector.x, 0.0f, targetVector.z);
        RaycastHit hit;
        bool isHit = Physics.Raycast(_eyePos.position, dir, out hit, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("PlayerTeam"));

        if (!isHit)
        {
            // print("nothing");
            return null;
        }

        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            return hit.transform.gameObject;
        }

        return null;
    }
}
