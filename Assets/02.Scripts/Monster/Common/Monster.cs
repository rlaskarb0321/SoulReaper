using System.Collections;
using UnityEngine;

[System.Serializable]
public struct MonsterBasicStat
{
    public int _health; // ü��
    public bool _isAttackFirst; // ����or�񼱰�����
    public float _attackDelay; // ������ �������ݱ��� ��ٷ����ϴ� �ð���
    public float _traceRadius; // �߰��� �����ϴ� ����
    public float _attakableRadius; // ���ݻ����Ÿ�
}

/// <summary>
/// ��� ���͵��� �������ϴ� �⺻���
/// </summary>
public class Monster : MonoBehaviour
{
    [Header("Basic Stat")]

    [Tooltip("������ ���� ���¸� ��Ÿ��")]
    public eMonsterState _state;
    
    [Tooltip("������ �ܰ�, ���")]
    public eMonsterLevel _level;
    
    [Tooltip("���͵� �⺻�� ���� ���")]
    public MonsterBasicStat _basicStat;

    public eMonsterType _monsterType;

    public enum eMonsterState { Patrol, Idle, Trace, Attack, Acting, Dead, }
    public enum eMonsterType { Melee, Range, Charge, MeleeAndRange, }
    public enum eMonsterLevel { Normal, Elite, MiddleBoss, Boss, }

    [HideInInspector]
    public MonsterAI _monsterAI;
    [HideInInspector]
    public PlayerCombat _playerCombat;

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
        Vector3 targetPos = _monsterAI._target.position;
        float angle;

        targetPos.y = 0.0f;
        myPos.y = 0.0f;
        angle = Quaternion.FromToRotation(transform.forward, targetPos - myPos).eulerAngles.y;

        // ������ transform.forward �������� �÷��̾ ���ʰ����������� �������� �����ϱ� ����
        if (angle - 180.0f > 0.0f)
            angle = -(360 - angle);

        while (Mathf.Abs(Quaternion.FromToRotation(transform.forward, targetPos - myPos).eulerAngles.y) >= 1.0f)
        {
            transform.eulerAngles = new Vector3(0.0f,
                Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + angle, 2.0f * Time.deltaTime), 0.0f);
            yield return null;
        }
    }
}
