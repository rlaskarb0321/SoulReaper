using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���͵��� ���� �ൿ ������ ���� ���� ���� �屸�� �����ϴ� Ŭ����
public class MonsterThink : MonoBehaviour
{
    public enum eMonsterDesires { Patrol, Trace, Attack, Run, Recover, Retreat, } // ���Ͱ� �ϰ����ϴ� �屸���� ����
    [SerializeField] private eMonsterDesires _monsterBrain;
    public eMonsterDesires MonsterBrain 
    {
        get { return _monsterBrain; }
        set
        {
            if (value != eMonsterDesires.Attack && _monsterBase._isAttack)
                _monsterBase._isAttack = false;

            _monsterBrain = value;
            switch (value)
            {
                case eMonsterDesires.Patrol:
                    _monsterBase._movSpeed = _monsterBase._basicStat._patrolMovSpeed;
                    break;
                case eMonsterDesires.Trace:
                    _monsterBase._movSpeed = _monsterBase._basicStat._traceMovSpeed;
                    break;
                case eMonsterDesires.Attack:
                    break;
                case eMonsterDesires.Run:
                    _monsterBase._movSpeed = _monsterBase._basicStat._kitingMovSpeed;
                    break;
                case eMonsterDesires.Recover:
                    break;
                case eMonsterDesires.Retreat:
                    _monsterBase._movSpeed = _monsterBase._basicStat._retreatMovSpeed;
                    break;
            }
        }
    }
    public bool _isTargetSet;
    [HideInInspector] public Transform _target;

    Monster _monsterBase;
    WaitForSeconds _ws;
    int _playerTeamLayer;

    void Awake()
    {
        _monsterBase = GetComponent<Monster>();
    }

    void Start()
    {
        _ws = new WaitForSeconds(_monsterBase._basicStat._thinkDelay);
        _playerTeamLayer = 1 << LayerMask.NameToLayer("PlayerTeam");

        StartCoroutine(DetermineDesires());
    }

    // ���Ͱ� ������ �ൿ�ϰ��� �� �ൿ�� �����ϱ����� ������ �屸�� ����
    // ����� ���Ͱ� Ÿ�����߰��ߴ�������, Ÿ�ٰ��� �Ÿ����� ���� �屸�� ����
    // �屸������ �����ؾ��ϴ°� : ���� ��������, ���� ����ü�°�, Ÿ ������ ��ȥ�� �ʵ忡�ִ���
    IEnumerator DetermineDesires()
    {
        Collider[] detectedColls;
        float targetDist;

        while (_monsterBase._state != Monster.eMonsterState.Dead)
        {
            yield return _ws;

            // Ÿ���� �������� ��������쿡�� Patrol
            if (!_isTargetSet)
            {
                // ��ü�� �ݸ����� ��°͵��߿� PlayerTeam�̶�� ���̾�� ���� ��ҵ鸸 �迭�� �߰�
                detectedColls = Physics.OverlapSphere(transform.position, _monsterBase._basicStat._traceRadius, _playerTeamLayer);

                if (detectedColls.Length >= 1)
                {
                    MonsterBrain = eMonsterDesires.Trace;
                    _isTargetSet = true;
                    _target = detectedColls[0].transform;
                }
                else
                {
                    MonsterBrain = eMonsterDesires.Patrol; 
                }
            }

            // Ÿ���� �����Ѱ��
            else
            {
                targetDist = Vector3.Distance(transform.position, _target.position);

                // Ÿ�ٰ��� �Ÿ������� Attack || Trace
                if (targetDist <= _monsterBase._basicStat._attakableRadius)
                    MonsterBrain = eMonsterDesires.Attack;
                else
                    MonsterBrain = eMonsterDesires.Trace;
            }
        }
    }
}
