using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("Health Value")]
    public float _maxHP;
    public float _currHP;

    [Header("Mana Value")]
    public float _maxMP;
    public float _currMP;

    PlayerFSM _fsm;
    Animator _animator;
    PlayerCombat _combat;
    FollowCamera _followCam;
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
        _followCam = _combat._followCamObj.GetComponent<FollowCamera>();

        _currHP = _maxHP;
        _currMP = _maxMP;
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

        StartCoroutine(_followCam.ShakingCamera(_combat._hitCamShakeDur, _combat._hitCamShakeAmount));
        _combat.EndComboAtk();
        attackDir = attackDir.normalized;
        transform.forward = -attackDir;
        _fsm.AtkDir = attackDir;
    }

    public void IncreaseHP(float amount)
    {

    }
}
