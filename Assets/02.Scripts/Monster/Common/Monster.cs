using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct MonsterBasicStat
{
    [Header("Combat")]
    public int health; // 체력
    public bool isAttackFirst; // 선공or비선공여부
    public float traceRadius; // 추격을 인지하는 범위
    public float attakableRadius; // 공격사정거리
    public float actDelay; // 몬스터의 다음행동까지 걸리게할 시간값
    public float defenseCoolTime; // 몬스터의 방어자세 취하기 관련 쿨타임

    [Header("Mov Speed Variable")]
    public float kitingMovSpeed;
    public float patrolMovSpeed;
    public float traceMovSpeed;
}

/// <summary>
/// 모든 몬스터들이 가져야하는 기본요소
/// </summary>
public class Monster : MonoBehaviour
{
    [Tooltip("몬스터의 단계, 계급")]
    public eMonsterLevel _level;
    
    [Tooltip("몬스터들 기본적 스텟 요소")]
    public MonsterBasicStat _basicStat;

    [Tooltip("몬스터의 공격 타입")]
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

    [Tooltip("0번째 인덱스는 기본 mat, 1번째 인덱스는 피격시 잠깐바뀔 mat")]
    public Material[] _materials;

    [Header("Soul Orb")]
    public GameObject _soulOrb;
    [Range(0.0f, 1.0f)]
    public float _orbSpawnPercentage; // 몬스터가 죽었을때 오브를 소환시킬 확률

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
        // 몬스터의 피격관련 이펙트작업들
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

    // 몬스터가 플레이어를 바라보게 해줌
    public IEnumerator LookTarget()
    {
        Vector3 myPos = transform.position;
        Vector3 targetPos = _brain._target.position;
        float angle;

        targetPos.y = 0.0f;
        myPos.y = 0.0f;
        angle = Quaternion.FromToRotation(transform.forward, (targetPos - myPos).normalized).eulerAngles.y;
        
        // 몬스터의 transform.forward 기준으로 플레이어가 왼쪽각도에있으면 왼쪽으로 돌게하기 위함
        if (angle > 180.0f)
            angle = -(360 - angle);

        while (Mathf.Abs(Quaternion.FromToRotation(transform.forward, (targetPos - myPos).normalized).eulerAngles.y) >= 1.0f)
        {
            transform.eulerAngles = new Vector3(0.0f,
                Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + (angle - 1.0f), 1.1f * Time.deltaTime), 0.0f);
            yield return null;
        }
    }

    // 몬스터가 타겟을 쫓아가게 함
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

    // 네비게이션 컴포넌트를 키고끄는작업
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
