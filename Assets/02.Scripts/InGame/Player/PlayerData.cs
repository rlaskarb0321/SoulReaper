using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("UI")]
    public UIScene _ui;

    [Header("Health Value")]
    public float _maxHP;
    public float _currHP;

    [Header("Mana Value")]
    public float _maxMP;
    public float _currMP;

    [Header("Health Seed Count")]
    public int _healthSeedCount;

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

    private void Start()
    {
        _currHP = _maxHP;
        _currMP = _maxMP;

        _ui.UpdateHPMP(UIScene.ePercentageStat.Hp, _currHP, _maxHP);
        _ui.UpdateHPMP(UIScene.ePercentageStat.Mp, _currMP, _maxMP);
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
        _ui.UpdateHPMP(UIScene.ePercentageStat.Hp, _currHP, _maxHP);
    }

    public void IncreaseHP(float amount)
    {
        _currHP += amount;
        if (_currHP >= _maxHP)
        {
            _currHP = _maxHP;
        }
        _ui.UpdateHPMP(UIScene.ePercentageStat.Hp, _currHP, _maxHP);
    }

    public bool DecreaseMP(float amount)
    {
        _currMP -= amount;
        _ui.UpdateHPMP(UIScene.ePercentageStat.Mp, _currMP, _maxMP);
        return false;
    }

    public void IncreaseMP(float amount)
    {
        _currMP += amount;
        if (_currMP >= _maxMP)
        {
            _currMP = _maxMP;
        }
        _ui.UpdateHPMP(UIScene.ePercentageStat.Mp, _currMP, _maxMP);
    }
}
