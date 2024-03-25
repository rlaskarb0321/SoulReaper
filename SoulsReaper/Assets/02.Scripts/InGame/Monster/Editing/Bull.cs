using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonsterBase 를 상속
public class Bull : MeleeRange
{
    public SphereCollider[] _weaponColl;

    private readonly int _hashAtkCombo = Animator.StringToHash("AtkCombo");
    private readonly int _hashIdle = Animator.StringToHash("Idle");

    [Header("=== Boss Sound ===")]
    public AudioClip[] _bossSound;

    [Header("=== Cam Shake ===")]
    [SerializeField]
    private float _camShakeAmount;

    [SerializeField]
    private float _camShakeDur;

    public enum eBossSound { Step_1, Step_2, Dead, Roar_1, Roar_2, Attack_1, Attack_2, Attack_3, Walk, Idle }

    protected override void Start() => base.Start();

    protected override void Update()
    {
        if (_state == MonsterBase_1.eMonsterState.Dead)
            return;

        switch (_state)
        {
            case eMonsterState.Trace:
                if (!_audio.isPlaying && _audio.enabled)
                {
                    _audio.clip = _bossSound[(int)eBossSound.Walk];
                    _audio.Play();
                }
                break;

            case eMonsterState.Attack:
                AimingTarget(_target.transform.position, 2.0f);
                Attack();
                break;

            // 공격 후 쿨타임 돌리는 시간, 쿨타임이 도는 동안 플레이어가 가까워지는지의 여부에따라 애니메이션 상태가 다르게 전이된다.
            case eMonsterState.Delay:
                bool targetNearAttackDist = TargetNearbyRange(_target.transform, _stat.attakDist);

                if (_stat.actDelay <= 0.0f)
                {
                    _stat.actDelay = _originDelay;
                    _animator.SetBool(_hashAttack, false);
                    _state = targetNearAttackDist ? eMonsterState.Attack : eMonsterState.Trace;
                    return;
                }
                _stat.actDelay -= Time.deltaTime;
                _animator.SetBool(_hashIdle, targetNearAttackDist);
                _animator.SetBool(_hashMove, !targetNearAttackDist);

                if (!targetNearAttackDist)
                {
                    Move(_target.transform.position, _stat.movSpeed * 0.2f);
                }
                break;
        }
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

    public override void Attack()
    {
        _nav.isStopped = true;
        _nav.velocity = Vector3.zero;
        _animator.SetBool(_hashAttack, true);
    }

    public override void SwitchNeedAiming(int value) => base.SwitchNeedAiming(value);

    public void ExecuteAttack(int weaponIndex)
    {
        if (weaponIndex == 2)
        {
            for (int i = 0; i < _weaponColl.Length; i++)
            {
                _weaponColl[i].enabled = !_weaponColl[i].enabled;
            }

            return;
        }

        _weaponColl[weaponIndex].enabled = !_weaponColl[weaponIndex].enabled;
    }

    /// <summary>
    // 콤보를 이어 갈 수 있다면 다음 콤보를 실행, 마지막 콤보에 다다르거나 콤보가 불가능하면 공격 취소, 공격 애니메이션에 붙히는 델리게이트 함수
    /// </summary>
    public void MakingComboAttack()
    {
        bool canCombo = TargetNearbyRange(_target.transform, _stat.attakDist * 1.75f);
        int combo = 0;

        if (canCombo)
        {
            combo = _animator.GetInteger(_hashAtkCombo);
            combo++;
            combo %= 3;
        }

        if (combo == 0 || !canCombo)
        {
            _state = eMonsterState.Delay;
            _animator.SetBool(_hashAttack, false);
        }

        _animator.SetInteger(_hashAtkCombo, combo);
    }

    public bool TargetNearbyRange(Transform target, float range)
    {
        return Vector3.Distance(target.position, transform.position) <= range;
    }

    // 보스몬스터가 죽었을 때 무기 콜리전 등 관련 변수들 끄기
    public override void Dead()
    {
        for (int i = 0; i < _weaponColl.Length; i++)
        {
            _weaponColl[i].gameObject.SetActive(false);
        }

        GetComponent<CapsuleCollider>().enabled = false;
        _audio.PlayOneShot(_bossSound[(int)eBossSound.Dead], _audio.volume * SettingData._sfxVolume);
        _nav.enabled = false;
        _currHp = 0.0f;
        _animator.SetTrigger(_hashDead);
        _animator.SetInteger(_hashAtkCombo, 0);
        _animator.SetBool(_hashAttack, false);
        _state = eMonsterState.Dead;

        UIScene._instance.UpdateSoulCount(_stat.soul);
        StartCoroutine(OnMonsterDead());
    }

    // 애니메이션 델리게이트, 달리는 소리
    public void PlayRunSound()
    {
        int randomSound = Random.Range(0, 2);
        _audio.PlayOneShot(_bossSound[randomSound], 0.2f * SettingData._sfxVolume);
    }

    // 애니메이션 델리게이트, 3단 공격 후, 포효 소리
    public void Roar()
    {
        int randomSound = Random.Range((int)eBossSound.Roar_1, (int)eBossSound.Roar_2 + 1);
        _audio.PlayOneShot(_bossSound[randomSound], _audio.volume * SettingData._sfxVolume);
    }

    // 애니메이션 델리게이트, 3단 공격이 땅에 닿았을 때 흔들림
    public void ShakeCam()
    {
        StartCoroutine(FollowCamera._instance.ShakeCamera(_camShakeAmount, _camShakeDur));
    }

    public void PlayAttackCry(int index)
    {
        _audio.PlayOneShot(_bossSound[(int)((eBossSound)index)], _audio.volume * SettingData._sfxVolume);
    }
}
