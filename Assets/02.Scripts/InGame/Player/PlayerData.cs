using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("=== Health Value ===")]
    public float _maxHP;
    public float _currHP;

    [Header("=== Mana Value ===")]
    public float _maxMP;
    public float _currMP;

    [Header("=== Basic Atk Power ===")]
    public float _basicAtkDamage;

    [Header("=== Health Seed Count ===")]
    public int _healthSeedCount;

    [Header("=== Soul Total Count ===")]
    public int _soulCount;

    PlayerFSM _fsm;
    Animator _animator;
    PlayerCombat _combat;
    PlayerMove_1 _move;
    AudioSource _audio;
    readonly int _hashHit = Animator.StringToHash("Hit");
    readonly int _hashDead = Animator.StringToHash("Dead");

    private void Awake()
    {
        _fsm = GetComponent<PlayerFSM>();
        _animator = GetComponent<Animator>();
        _combat = GetComponent<PlayerCombat>();
        _move = GetComponent<PlayerMove_1>();
        _audio = GetComponent<AudioSource>();
    }

    public void DecreaseHP(Vector3 attackDir, float damage, AudioClip hitSound)
    {
        if (_fsm.State == PlayerFSM.eState.Hit || _fsm.State == PlayerFSM.eState.Dead)
            return;
        if (_currHP - damage <= 0.0f)
        {
            _currHP = 0.0f;
            _fsm.State = PlayerFSM.eState.Dead;
            _animator.SetTrigger(_hashDead);
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<CapsuleCollider>().enabled = false;
            UIScene._instance.DeadPlayer();
        }
        else
        {
            _currHP -= damage;
            _fsm.State = PlayerFSM.eState.Hit;
            _animator.SetTrigger(_hashHit);
        }

        StartCoroutine(FollowCamera._instance.ShakeCamera(_combat._hitCamShakeAmount, _combat._hitCamShakeDur));
        _audio.PlayOneShot(hitSound, 0.6f);
        _combat._weapon.ResetHitEnemy();
        _combat.EndComboAtk();
        _combat.InitChargingGauge();
        _move._animator.SetBool(_move._hashRoll, false);

        attackDir = new Vector3(attackDir.x, 0.0f, attackDir.z);
        attackDir = attackDir.normalized;
        transform.forward = -attackDir;
        _fsm.AtkDir = attackDir;

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.HP, _currHP, _maxHP, false);
    }

    public void DecreaseMP(float amount)
    {
        _currMP -= amount;
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.MP, (int)_currMP, _maxMP, false);
    }
}
