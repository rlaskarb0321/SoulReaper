using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRange : MonsterBase
{
    [Header("=== Long Range & Object Pool ===")]
    public ObjectPooling _projectilePool;
    public Transform _firePos;
    public float _projectileSpeed;

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
        if (_state == MonsterBase.eMonsterState.Dead)
            return;

        switch (_state)
        {
            case eMonsterState.Attack:
                AimingTarget(_target.transform.position, 2.0f);
                Attack();
                break;

            case eMonsterState.Delay:
                SwitchNeedAiming(1);
                Delay();
                AimingTarget(_target.transform.position, 1.0f);
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

    public override void AimingTarget(Vector3 target, float rotMulti)
    {
        if (_needAiming)
        {
            _nav.updatePosition = false;
            Vector3 dir = target - transform.position;
            transform.rotation =
                Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _stat.rotSpeed * rotMulti * Time.deltaTime);
        }
        else
        {
            _nav.updatePosition = true;
        }
    }

    public override void SwitchNeedAiming(int value) => _needAiming = value == 1 ? true : false;

    public void ExecuteAtk()
    {
        Vector3 launchAngle = _target.transform.position - transform.position;
        LaunchData launchData = new LaunchData(launchAngle, _firePos.position, _stat.damage, _projectileSpeed);
        VFXPool projectile = _projectilePool.PullOutObject();

        projectile.gameObject.SetActive(true);
        projectile.SetPoolData(launchData);
        _audio.PlayOneShot(_sound[(int)eSound.Attack], _audio.volume * SettingData._sfxVolume);
    }

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
            _needAiming = false;
            return;
        }

        _stat.actDelay -= Time.deltaTime;
    }
}
