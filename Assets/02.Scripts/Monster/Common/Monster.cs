using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterBasicStat
{
    [Header("Combat")]
    public int _health; // ü��
    public bool _isAttackFirst; // ����or�񼱰�����
    public float _traceRadius; // �߰��� �����ϴ� ����
    public float _attakableRadius; // ���ݻ����Ÿ�
    public float _actDelay; // ������ �����ൿ���� �ɸ����� �ð���

    [Header("Mov Speed Variable")]
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

    public enum eMonsterType { Melee, Range, Charge, MeleeAndRange, }
    public enum eMonsterLevel { Normal, Elite, MiddleBoss, Boss, }

    [HideInInspector] public PlayerCombat _playerCombat;
    [HideInInspector] public NavMeshAgent _nav;
    [HideInInspector] public MonsterAI _brain;
    [HideInInspector] public Animator _animator;

    [HideInInspector] public bool _isActing;
    [HideInInspector] public bool _isFindPatrolPos;
    [HideInInspector] public Vector3 _patrolPos;
    public float _currHp;
    public float _movSpeed;
    public WaitForSeconds _actWaitSeconds;
    [Tooltip("0��° �ε����� �⺻ mat, 1��° �ε����� �ǰݽ� ���ٲ� mat")]
    public Material[] _materials;
    
    SkinnedMeshRenderer _mesh;
    BoxCollider _mainColl;
    Rigidbody _rbody;

    readonly int _hashDead = Animator.StringToHash("Dead");

    protected virtual void Awake()
    {
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _mainColl = GetComponent<BoxCollider>();
        _rbody = GetComponent<Rigidbody>();
    }

    public virtual void DecreaseHp(float amount)
    {
        // ������ �ǰݰ��� ����Ʈ�۾���
        _currHp -= amount;
        StartCoroutine(OnHitEffect());

        if (_currHp == 0.0f)
        {
            Dead();
        }
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

    void Dead()
    {
        StartCoroutine(BuryBody());
        _mainColl.enabled = false;
        _brain.MonsterBrain = MonsterAI.eMonsterDesires.Dead;
        _nav.isStopped = true;
        _nav.enabled = false;

        _animator.SetTrigger(_hashDead);
    }

    IEnumerator BuryBody()
    {
        yield return new WaitForSeconds(4.5f);
        _rbody.isKinematic = false;

        yield return new WaitForSeconds(1.2f);
        this.gameObject.SetActive(false);
    }

    IEnumerator OnHitEffect()
    {
        _mesh.material = _materials[1];

        yield return new WaitForSeconds(Time.deltaTime * 3.0f);
        
        _mesh.material = _materials[0];
    }
}
