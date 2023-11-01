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
    readonly int _hashHit = Animator.StringToHash("Hit");
    readonly int _hashDead = Animator.StringToHash("Dead");

    private void Awake()
    {
        _fsm = GetComponent<PlayerFSM>();
        _animator = GetComponent<Animator>();
        _combat = GetComponent<PlayerCombat>();
    }

    public void DecreaseHP(Vector3 attackDir, float damage)
    {
        if (_fsm.State == PlayerFSM.eState.Hit || _fsm.State == PlayerFSM.eState.Dead)
            return;
        if (_currHP - damage <= 0.0f)
        {
            _currHP = 0.0f;
            _fsm.State = PlayerFSM.eState.Dead;
            _animator.SetTrigger(_hashDead);
            UIScene._instance.DeadPlayer();
        }
        else
        {
            _currHP -= damage;
            _fsm.State = PlayerFSM.eState.Hit;
            _animator.SetTrigger(_hashHit);
        }

        StartCoroutine(FollowCamera._instance.ShakeCamera(_combat._hitCamShakeAmount, _combat._hitCamShakeDur));
        _combat.EndComboAtk();

        attackDir = new Vector3(attackDir.x, 0.0f, attackDir.z);
        attackDir = attackDir.normalized;
        transform.forward = -attackDir;
        _fsm.AtkDir = attackDir;

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.HP, _currHP, _maxHP, false);
    }

    public bool DecreaseMP(float amount)
    {
        _currMP -= amount;
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.MP, _currMP, _maxMP, false);
        return false;
    }
}
