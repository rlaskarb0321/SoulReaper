using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRange_2 : MonsterBase_2
{
    public GameObject _projectile;
    public Transform _firePos;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void DecreaseHp(float amount)
    {
        
    }

    public override IEnumerator OnHitEffect()
    {
        yield return null;
    }

    public override void Attack()
    {
        _isAtk = true;
        _nav.isStopped = true;
        _nav.velocity = Vector3.zero;
        _animator.SetTrigger(_hashAtk1);
    }

    public override void Move(Vector3 pos, float movSpeed)
    {
        if (_nav.pathPending)
        {
            return;
        }

        _nav.isStopped = false;
        _nav.speed = movSpeed;
        _nav.SetDestination(pos);
    }

    public override void Idle()
    {
        
    }

    public override void OnMonsterDie()
    {

    }

    public override void LookTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _stat.rotSpeed * Time.deltaTime);
    }

    #region 공격 애니메이션 델리게이트 함수
    public void LaunchMissile() => Instantiate(_projectile, _firePos.position, transform.rotation);
    public void EndAttack()
    {
        _isAtk = !_isAtk;
        _brain._fsm = MonsterAI_2.eMonsterFSM.Idle;
    }
    #endregion 공격 애니메이션 델리게이트 함수

    
}
