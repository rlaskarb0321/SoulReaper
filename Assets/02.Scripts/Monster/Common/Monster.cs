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

    public Transform _projectileSpawnPos;
    public GameObject _projectile;

    public enum eMonsterState { Patrol, Trace, Attack, Acting, Dead, }
    public enum eMonsterLevel { Normal, Elite, MiddleBoss, Boss, }

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

    public virtual IEnumerator LookTarget()
    {
        yield return null;
    }

    public virtual void ExecuteAttack(int attackNum)
    {

    }
}
