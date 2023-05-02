using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterBasicStat
{
    [Header("Combat")]
    public int health; // ü��
    public bool isAttackFirst; // ����or�񼱰�����
    public float traceRadius; // �߰��� �����ϴ� ����
    public float attakableRadius; // ���ݻ����Ÿ�
    public float actDelay; // ������ �����ൿ���� �ɸ����� �ð���
    public float defenseCoolTime; // ������ ����ڼ� ���ϱ� ���� ��Ÿ��

    [Header("Mov Speed Variable")]
    public float kitingMovSpeed;
    public float patrolMovSpeed;
    public float traceMovSpeed;
}

/// <summary>
/// ��� ���͵��� �������ϴ� �⺻���
/// </summary>
public class Monster : MonoBehaviour
{
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

    public bool _isActing;
    [HideInInspector] public bool _isFindPatrolPos;
    [HideInInspector] public Vector3 _patrolPos;

    public float _currHp;
    public float _movSpeed;
    [Header("Defense State")]
    public float _currDefenseCool;
    public float _currActDelay;

    [Tooltip("0��° �ε����� �⺻ mat, 1��° �ε����� �ǰݽ� ���ٲ� mat")]
    public Material[] _materials;

    [Header("Soul Orb")]
    public GameObject _soulOrb;
    [Range(0.0f, 1.0f)]
    public float _orbSpawnPercentage; // ���Ͱ� �׾����� ���긦 ��ȯ��ų Ȯ��

    protected Rigidbody _rbody;
    
    SkinnedMeshRenderer _mesh;
    BoxCollider _mainColl;
    readonly int _hashDead = Animator.StringToHash("Dead");

    protected virtual void Awake()
    {
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _mainColl = GetComponent<BoxCollider>();
        _rbody = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        _currDefenseCool = _basicStat.defenseCoolTime;
        _currHp = _basicStat.health;
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
        StopNav(false);

        if (!_nav.pathPending)
            _nav.SetDestination(_brain._target.position);
    }

    void Dead()
    {
        float randomValue = Random.Range(0.0f, 1.0f);

        _animator.SetTrigger(_hashDead);
        if (randomValue < _orbSpawnPercentage)
            Instantiate(_soulOrb, transform.position, Quaternion.identity);

        StartCoroutine(BuryBody());
        _mainColl.enabled = false;
        _brain.MonsterBrain = MonsterAI.eMonsterDesires.Dead;
        _nav.isStopped = true;
        _nav.enabled = false;
    }

    IEnumerator BuryBody()
    {
        yield return new WaitForSeconds(3.5f);

        Color color = _materials[0].color;
        while (_materials[0].color.a >= 0.05f)
        {
            color.a -= Time.deltaTime * 1.5f;
            _materials[0].color = color;
            yield return null;
        }

        this.gameObject.SetActive(false);
        color = Color.white;
        _materials[0].color = color;
    }

    IEnumerator OnHitEffect()
    {
        _mesh.material = _materials[1];

        yield return new WaitForSeconds(Time.deltaTime * 3.0f);
        
        _mesh.material = _materials[0];
    }

    // �׺���̼� ������Ʈ�� Ű������۾�
    public void StopNav(bool isStopNav)
    {
        if (isStopNav)
        {
            _nav.velocity = Vector3.zero;
            _rbody.isKinematic = false;
        }    
        else
        {
            _rbody.isKinematic = true;
        }

        _nav.isStopped = isStopNav;
    }
}
