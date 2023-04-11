using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterAttack : Monster
{
    public enum eNormalType { Melee, Range, Charge, MeleeAndRange, }

    [Header("Normal Level Monster")]
    [Range(1, 3)]
    public int _numOfAttacks;
    public GameObject _hitBoxCollObj;
    public eNormalType _monsterType;

    Animator _animator;
    MonsterAI _monsterAI;
    ProjectilePool _projectilePool;

    readonly int _hashDoAttack = Animator.StringToHash("DoAttack");

    void Awake()
    {
        _monsterAI = GetComponent<MonsterAI>();
        _animator = GetComponent<Animator>();
        _projectilePool = GetComponent<ProjectilePool>();
    }

    void Start()
    {
        if (_hitBoxCollObj != null)
            _hitBoxCollObj.SetActive(false);
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

        RigidbodyConstraints originConstraints; 
        float randWaitSeconds;

        originConstraints = _monsterAI._rbody.constraints;
        ManageMonsterNavigation(false, originConstraints);
        _state = eMonsterState.Acting;
        yield return StartCoroutine(LookTarget());

        _animator.SetTrigger(_hashDoAttack);
        randWaitSeconds = _basicStat._attackDelay + Random.Range(-0.8f, 0.8f);
        yield return new WaitForSeconds(randWaitSeconds);

        _state = eMonsterState.Trace;
        ManageMonsterNavigation(true, originConstraints);
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
                var projectile = _projectilePool._pool.Get();
                break;
            case eNormalType.Charge:
                break;
            case eNormalType.MeleeAndRange:
                break;
        }
    }

    void ManageMonsterNavigation(bool isOn, RigidbodyConstraints originConstValue)
    {
        _monsterAI._navAgent.enabled = isOn;

        if (isOn == false)
        {
            _monsterAI._rbody.constraints = RigidbodyConstraints.FreezePositionY | originConstValue;
        }
        else
        {
            _monsterAI._rbody.constraints = originConstValue;
        }
    }
}
