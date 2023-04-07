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
    public enum eMonsterState { Patrol, Trace, Attack, Dead, }
    public enum eMonsterLevel { Normal, Elite, MiddleBoss, Boss, }

    [Tooltip("������ ���� ���¸� ��Ÿ��")]
    public eMonsterState _monsterState;
    
    [Tooltip("������ �ܰ�, ���")]
    public eMonsterLevel _level;
    
    [Tooltip("���͵� �⺻�� ���� ���")]
    public MonsterBasicStat _monsterBasicStat;

    [Tooltip("���͵� ���� �ൿ���� �ɸ��� �� �ð�")]
    public float _nextActDelay;

    [Header("Target")]
    [Tooltip("���͵��� ������ ��(= �÷��̾�)")]
    public Transform _target;

    protected string _playerName;

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
}
