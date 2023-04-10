using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

public class NormalMonster : Monster
{
    public enum eNormalType { Melee, Range, Charge, MeleeAndRange, }

    [Header("Normal Level Monster")]
    public int _numOfAttacks;
    public GameObject _hitBoxCollObj;
    public GameObject _projectile;
    public Transform _projectileSpawnPos;
    public eNormalType _monsterType;

    Animator _animator;
    MonsterAI _monsterAI;
    WaitForSeconds _ws;
    IObjectPool<EnemyProjectile> _pool;

    readonly int _hashDoAttack = Animator.StringToHash("DoAttack");

    void Awake()
    {
        _monsterAI = GetComponent<MonsterAI>();
        _animator = GetComponent<Animator>();
        _pool = new ObjectPool<EnemyProjectile>(CreateProjectile, OnGetProjectile, OnReleaseProjectile, DestroyProjectile, maxSize : 5);
    }

    void Start()
    {
        if (_hitBoxCollObj != null)
            _hitBoxCollObj.SetActive(false);

        _ws = new WaitForSeconds(_monsterAI._nextActDelay);
    }

    void Update()
    {
        if (_state == Monster.eMonsterState.Dead)
            return;

        
    }

    public override IEnumerator DoAttack()
    {
        if (_state == eMonsterState.Dead)
            yield break;

        RigidbodyConstraints originConstraints = _monsterAI._rbody.constraints;

        _monsterAI._navAgent.enabled = false;
        _monsterAI._rbody.constraints = RigidbodyConstraints.FreezePositionY | originConstraints;
        _state = eMonsterState.Acting;
        yield return StartCoroutine(LookTarget());

        _animator.SetTrigger(_hashDoAttack);
        yield return _ws;

        _state = eMonsterState.Trace;
        _monsterAI._navAgent.enabled = true;
        _monsterAI._rbody.constraints = originConstraints;
    }

    // 공격범위안에서 플레이어를 향해 바라봄
    public override IEnumerator LookTarget()
    {
        Vector3 myPos = transform.position;
        Vector3 targetPos = _monsterAI._target.position;
        float angle;

        targetPos.y = 0.0f;
        myPos.y = 0.0f;
        angle = Quaternion.FromToRotation(transform.forward, targetPos - myPos).eulerAngles.y;

        // 몬스터의 transform.forward 기준으로 플레이어가 왼쪽각도에있으면 왼쪽으로 돌게하기 위함
        if (angle - 180.0f > 0.0f)
            angle = -(360 - angle);

        while (Mathf.Abs(Quaternion.FromToRotation(transform.forward, targetPos - myPos).eulerAngles.y) >= 1.0f)
        {
            transform.eulerAngles = new Vector3(0.0f, 
                Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + angle, 2.0f * Time.deltaTime), 0.0f);
            yield return null;
        }
    }

    // 몬스터의 애니메이션 델리게이트로 공격을 실행할 시점 등을 설정한다.
    public override void ExecuteAttack(int attackNum)
    {
        switch (_monsterType)
        {
            case eNormalType.Melee:
                break;
            case eNormalType.Range:
                var projectile = _pool.Get();
                break;
            case eNormalType.Charge:
                break;
            case eNormalType.MeleeAndRange:
                break;
        }
    }

    EnemyProjectile CreateProjectile()
    {
        EnemyProjectile projectile = Instantiate(_projectile, _projectileSpawnPos.position, transform.rotation).GetComponent<EnemyProjectile>();
        projectile.SetManagedPool(_pool);

        return projectile;
    }

    void OnGetProjectile(EnemyProjectile projectile)
    {
        projectile.gameObject.SetActive(true);

        projectile._isReleased = false;
        projectile.transform.position = _projectileSpawnPos.position;
        projectile.transform.rotation = transform.rotation;
    }

    void OnReleaseProjectile(EnemyProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    void DestroyProjectile(EnemyProjectile projectile)
    {
        Destroy(projectile.gameObject);
    }
}
