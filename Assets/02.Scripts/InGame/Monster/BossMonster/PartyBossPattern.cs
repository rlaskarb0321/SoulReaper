using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 파티 몬스터의 공격 기술을 정의하는 함수
/// </summary>
public class PartyBossPattern : MonoBehaviour
{
    [Header("=== Blink Particle ===")]
    [SerializeField]
    private GameObject _stoneHit;

    [SerializeField]
    private GameObject _target;

    [SerializeField]
    private float _blinkOffset;

    // 이 곳에 phase 여부를 달아놓아도 될듯

    // Field
    private Animator _animator;
    private MonsterBase_1 _monsterBase;
    private readonly int _hashBlink = Animator.StringToHash("Blink Trigger");
    private readonly int _hashBlinkBack = Animator.StringToHash("Blink Back Pos");
    private readonly int _hashSliding = Animator.StringToHash("Sliding Trigger");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _monsterBase = GetComponent<MonsterBase_1>();
        _target = _monsterBase._target;
    }

    #region 블링크

    public void Blink()
    {
        _animator.SetTrigger(_hashBlink);
    }

    public void BlinkAppear()
    {
        _animator.SetTrigger(_hashBlinkBack);
    }

    /// <summary>
    /// 블링크 할 때 나오는 파티클 이펙트를 켜주는 애니메이션 델리게이트
    /// </summary>
    public void ActiveBlinkParticle()
    {
        _stoneHit.transform.position = transform.position + Vector3.up * 2.0f;
        _stoneHit.SetActive(true);
    }

    public void MoveToTargetBehind()
    {
        Vector3 blinkPos = _target.transform.position + (_target.transform.forward * -_blinkOffset);
        transform.forward = (_target.transform.position - blinkPos).normalized;
        transform.position = blinkPos;
    }

    #endregion 블링크

    public void SummonMiniBoss()
    {
        print("미니보스 소환 얍");
    }

    public void DropKick()
    {
        print("드랍 킥 얍");
    }

    public void SummonNormalMonster()
    {
        print("노말 몹 소환 얍");
    }

    #region 슬라이딩 공격

    public void Sliding()
    {
        _animator.SetTrigger(_hashSliding);
    }

    #endregion 슬라이딩 공격

    public void Jump()
    {
        print("점프 공격 얍");
    }

    public void Fist()
    {
        print("주먹 공격 얍");
    }

    public void Push()
    {
        print("밀기 공격 얍");
    }
}
