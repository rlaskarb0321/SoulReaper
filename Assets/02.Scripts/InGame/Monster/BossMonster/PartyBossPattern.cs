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

    // Field
    private Animator _animator;
    private readonly int _hashBlink = Animator.StringToHash("Blink Trigger");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    #region 블링크

    public void Blink()
    {
        _animator.SetTrigger(_hashBlink);
        print("블링크 얍");
    }

    /// <summary>
    /// 블링크 할 때 나오는 파티클 이펙트를 켜주는 애니메이션 델리게이트
    /// </summary>
    public void ActiveBlinkParticle() => _stoneHit.SetActive(true);

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

    public void Sliding()
    {
        print("슬라이딩 공격 얍");
    }

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
