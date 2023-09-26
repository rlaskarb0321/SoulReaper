using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRange : MonsterBase_1
{
    [Header("=== Melee Range ===")]
    public BoxCollider _attackCollObj;

    private bool _needAiming;
    private float _originDelay;
    private readonly int _hashAttack = Animator.StringToHash("Attack");

    protected override void Start()
    {
        base.Start();

        _originDelay = _stat.actDelay;
    }

    private void Update()
    {
        if (_state == MonsterBase_1.eMonsterState.Dead)
            return;

        switch (_state)
        {
            case eMonsterState.Attack:
                AimingTarget(_target.transform.position, 2.0f);
                Attack();
                break;

            case eMonsterState.Delay:
                Delay();
                break;
        }
    }

    public override void Attack()
    {
        if (_animator.GetBool(_hashAttack))
            return;

        _nav.isStopped = true;
        _nav.velocity = Vector3.zero;

        _animator.SetBool(_hashMove, false);
        _animator.SetBool(_hashAttack, true);
    }

    public override void AimingTarget(Vector3 target, float rotMultiple)
    {
        if (_needAiming)
        {
            _nav.updatePosition = false;
            Vector3 dir = target - transform.position;
            transform.rotation = 
                Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _stat.rotSpeed * rotMultiple * Time.deltaTime);
        }
        else
        {
            _nav.updatePosition = true;
        }
    }

    public void SwitchNeedAiming(int value) => _needAiming = value == 1 ? true : false;

    public void ExecuteAtk() => _attackCollObj.enabled = !_attackCollObj.enabled;

    public void EndAttack()
    {
        _animator.SetBool(_hashAttack, false);
        _state = eMonsterState.Delay;
    }

    public override void Delay()
    {
        if (_stat.actDelay <= 0.0f)
        {
            _stat.actDelay = _originDelay;
            _state = eMonsterState.Trace;
            return;
        }

        _stat.actDelay -= Time.deltaTime;
    }
}
