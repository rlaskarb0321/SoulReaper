using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterBasicStat
{
    public int _health; // ü��
    public bool _isAttackFirst; // ����or�񼱰�����
    public float _traceRadius; // �߰��� �����ϴ� ����
    public float _attakableRadius; // ���ݻ����Ÿ�
    public float _actDelay; // ������ �����ൿ���� �ɸ����� �ð���

    public float _kitingMovSpeed;
    public float _patrolMovSpeed;
    public float _traceMovSpeed;
    public float _retreatMovSpeed;
}

/// <summary>
/// ��� ���͵��� �������ϴ� �⺻���
/// </summary>
public class Monster : MonoBehaviour
{
    [Header("Basic Stat")]
    //[Tooltip("������ ���� ���¸� ��Ÿ��")]
    // [HideInInspector] public eMonsterState _state;
    
    [Tooltip("������ �ܰ�, ���")]
    public eMonsterLevel _level;
    
    [Tooltip("���͵� �⺻�� ���� ���")]
    public MonsterBasicStat _basicStat;

    [Tooltip("������ ���� Ÿ��")]
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

    // ���Ͱ� �÷��̾ �ٶ󺸰� ����
    public IEnumerator LookTarget()
    {
        Vector3 myPos = transform.position;
        Vector3 targetPos = _brain._target.position;
        float angle;

        targetPos.y = 0.0f;
        myPos.y = 0.0f;
        angle = Quaternion.FromToRotation(transform.forward, (targetPos - myPos).normalized).eulerAngles.y;
        
        // ������ transform.forward �������� �÷��̾ ���ʰ����������� �������� �����ϱ� ����
        if (angle > 180.0f)
            angle = -(360 - angle);

        while (Mathf.Abs(Quaternion.FromToRotation(transform.forward, (targetPos - myPos).normalized).eulerAngles.y) >= 1.0f)
        {
            transform.eulerAngles = new Vector3(0.0f,
                Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + (angle - 1.0f), 1.1f * Time.deltaTime), 0.0f);
            yield return null;
        }
    }

    // ���Ͱ� Ÿ���� �Ѿư��� ��
    public void TraceTarget()
    {
        _nav.isStopped = false;

        if (!_nav.pathPending)
            _nav.SetDestination(_brain._target.position);
    }
}
