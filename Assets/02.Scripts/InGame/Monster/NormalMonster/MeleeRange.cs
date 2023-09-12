using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRange : MonsterBase, INormalMonster
{
    [Header("=== Melee ===")]
    public GameObject _atkCollObj;
    public float _atkDmg;

    [Header("=== Knock Back ===")]
    public int _knockBackFrame;

    [HideInInspector]
    public MonsterAI _brain;
    private BoxCollider _atkColl;
    private readonly int _hashAtk1 = Animator.StringToHash("Attack1");
    private WaitForFixedUpdate _wfs;

    protected override void Awake()
    {
        base.Awake();

        _brain = GetComponent<MonsterAI>();
        _atkColl = _atkCollObj.GetComponent<BoxCollider>();
        _normalMonster = this;
    }

    protected override void Start()
    {
        base.Start();

        _wfs = new WaitForFixedUpdate();
    }

    public override void Attack()
    {
        _isAtk = true;
        _nav.isStopped = true;
        _nav.velocity = Vector3.zero;

        _animator.SetBool(_hashMove, false);
        _animator.SetTrigger(_hashAtk1);
    }

    #region 공격애니메이션 델리게이트 함수
    public void ExecuteAtk() => _atkColl.enabled = !_atkColl.enabled;
    public void EndAttack()
    {
        _isAtk = !_isAtk;
        _brain._fsm = MonsterAI.eMonsterFSM.Idle;
    }
    #endregion 공격애니메이션 델리게이트 함수

    public override void DecreaseHp(float amount, Vector3 hitPos)
    {
        StartCoroutine(KnockBack(hitPos));
        StartCoroutine(OnHitEvent());

        _currHp -= amount;
        if (_currHp <= 0.0f)
        {
            _currHp = 0.0f;
            Dead();
        }
    }

    public override void Idle()
    {
        _animator.SetBool(_hashMove, false);
        _animator.SetTrigger(_hashIdle);
    }

    public void LookTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _stat.rotSpeed * Time.deltaTime);
    }

    public IEnumerator KnockBack(Vector3 hitPos)
    {
        hitPos.y = transform.position.y;
        Vector3 knockBackDir = (transform.position - hitPos).normalized;
        int knockBackFrame = _knockBackFrame;

        while (--knockBackFrame >= 0)
        {
            transform.position += knockBackDir * Time.fixedDeltaTime;
            yield return _wfs;
        }
    }

    public override void Move(Vector3 pos, float movSpeed)
    {
        if (_nav.pathPending)
        {
            return;
        }

        _animator.SetBool(_hashMove, true);
        _nav.isStopped = false;
        _nav.speed = movSpeed;
        _nav.SetDestination(pos);
    }

    public override IEnumerator OnHitEvent()
    {
        Material newMat;

        newMat = _hitMats[1];
        _mesh.material = newMat;
        yield return new WaitForSeconds(Time.deltaTime * 4.0f);

        newMat = _hitMats[0];
        _mesh.material = newMat;
    }

    

    protected override void Dead()
    {
        BoxCollider boxColl = this.GetComponent<BoxCollider>();

        _brain._fsm = MonsterAI.eMonsterFSM.Dead;
        _animator.SetTrigger(_hashDead);
        _nav.velocity = Vector3.zero;
        _nav.isStopped = true;
        _nav.baseOffset = 0.0f;
        boxColl.size = Vector3.one;
        boxColl.enabled = false;

        StartCoroutine(OnMonsterDie());
    }

    protected override IEnumerator OnMonsterDie()
    {
        yield return new WaitForSeconds(_bodyBuryTime);

        //Material newMat = _mesh.material;
        Material newMat = Instantiate(_deadMat);
        Color color = newMat.color;

        while (newMat.color.a >= 0.05f)
        {
            color.a -= Time.deltaTime;
            newMat.color = color;
            _mesh.material = newMat;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
