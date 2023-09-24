 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase_1 : MonoBehaviour
{
    public enum eMonsterState { Idle, Move, Attack, Hit, Delay, Dead, }
    public enum eMonsterType { Wave, Patrol, }

    [Header("=== Type ===")]
    public eMonsterType _type;

    [Header("=== Stat ===")]
    public MonsterStat _stat;

    [Header("=== FSM ===")]
    public eMonsterState _state;

    [Header("=== Target ===")]
    public Transform _eyePos;
    public GameObject _target;

    [Header("=== Hit & Dead ===")]
    public Material[] _hitMats; // 0�� �ε����� �⺻ mat, 1�� �ε����� �ǰݽ� ���ٲ� mat
    public float _bodyBuryTime; // ��üó�������� ���۱��� ��ٸ� ��
    public Material _deadMat;

    [Header("=== Monster Type ===")]
    public WaveMonster _waveMonster;
    public SentryMonster_1 _sentryMonster;

    protected void Awake()
    {
        if (_waveMonster == null && _sentryMonster == null)
        {
            Debug.LogError(gameObject.name + "������Ʈ�� �ʺ� �Ǵ� ���̺� �� ���� ������ �������� ����");
            return;
        }

        if (_type == eMonsterType.Wave)
        {
            _stat.traceDist = 150.0f;
        }
    }

    protected void Start()
    {

    }

    public virtual void SearchTarget() { }
    public virtual void Move(Vector3 pos, float movSpeed) { }
    public virtual void DecreaseHP(float amount) { }
    public virtual IEnumerator OnHitEvent() { yield return null; }
    public virtual void Dead() { }
    public virtual IEnumerator OnMonsterDead() { yield return null; }
}