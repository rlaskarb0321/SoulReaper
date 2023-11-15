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
    [Tooltip("블링크 할 때 & 블링크 후 커질때 나오는 이펙트")]
    private GameObject _stoneHit;

    [SerializeField]
    [Tooltip("블링크로 뒤에서 나타날때 플레이어와 이격할 거리")]
    private float _blinkOffset;

    [Header("=== Jump Attack ===")]
    [SerializeField]
    [Tooltip("점프 파워")]
    private float _jumpForce;

    [SerializeField]
    [Tooltip("점프 후 체공중일 때 땅에 닿음을 판단하기 위한 발 밑 레이의 위치")]
    private Transform _groundRayPos;

    [SerializeField]
    [Tooltip("발 밑 레이의 길이")]
    private float _rayDist;

    // 이 곳에 phase 여부를 달아놓아도 될듯

    // Field
    private GameObject _target;
    private Animator _animator;
    private MonsterBase_1 _monsterBase;
    private Rigidbody _rbody;
    private readonly int _hashBlink = Animator.StringToHash("Blink Trigger");
    private readonly int _hashBlinkBack = Animator.StringToHash("Blink Back Pos");
    private readonly int _hashSliding = Animator.StringToHash("Sliding Trigger");
    private readonly int _hashJump = Animator.StringToHash("Jump Trigger");
    private readonly int _hashJumpEnd = Animator.StringToHash("Jump End");
    private readonly int _hashFist = Animator.StringToHash("Fist Trigger");
    private readonly int _hashPush = Animator.StringToHash("Push Trigger");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _monsterBase = GetComponent<MonsterBase_1>();
        _rbody = GetComponent<Rigidbody>();
        _target = _monsterBase._target;
    }

    private void Update()
    {
        JudgeGrounded();
    }

    #region 공격에 돌진이 필요할 때 사용하는 공통적인 메서드


    public void AddRush(AnimationEvent myEvent)
    {
        int dist = myEvent.intParameter;
        float time = myEvent.floatParameter;
        Vector3 target = transform.position + transform.forward * dist;
        StartCoroutine(RushToTarget(target, time));
    }

    private IEnumerator RushToTarget(Vector3 target, float time)
    {
        Vector3 refVector = Vector3.zero;
        while (Vector3.Distance(transform.position, target) >= 0.1f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target, ref refVector, time); // 블링크 펀치는 0.08, 슬라이딩은 조금 높게
            yield return null;
        }

        transform.position = target;
    }

    #endregion 공격에 돌진이 필요할 때 사용하는 공통적인 메서드

    #region 일반적인 상태일 때 보스의 스킬들

    #region 블링크 공격

    // 블링크 후 나오면서 주먹 찌르기 할 때 전진시켜야 함

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

    #endregion 블링크 공격

    #region 미니 보스 소환하기

    public void SummonMiniBoss()
    {
        print("미니보스 소환 얍");
    }

    #endregion 미니 보스 소환하기

    #region 드랍킥 공격

    public void DropKick()
    {
        print("드랍 킥 얍");
    }

    #endregion 드랍킥 공격

    #region 일반 몬스터 소환하기

    public void SummonNormalMonster()
    {
        print("노말 몹 소환 얍");
    }

    #endregion 일반 몬스터 소환하기

    #region 슬라이딩 공격

    // 앞으로 나아가며 슬라이딩 공격하게 해야 함

    public void Sliding()
    {
        _animator.SetTrigger(_hashSliding);
    }

    #endregion 슬라이딩 공격

    #region 점프 공격

    // 점프 후 내려찍힐 곳을 알려주는 그림자 오브젝트와
    // 내려찍힐 곳으로 이동하는 기능이 필요
    // 점프의 속도도 빠르게

    public void Jump()
    {
        _animator.ResetTrigger(_hashJumpEnd);
        _animator.SetTrigger(_hashJump);
    }

    /// <summary>
    /// JumpStart Anim의 시작 프레임에 붙힐 delegate 함수
    /// </summary>
    public void JumpStart()
    {
        _rbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// 점프 중 or 땅에 닿음 상태로 만드는 메서드
    /// </summary>
    /// <param name="value"></param>
    public void SetJumpState(int value)
    {
        bool setValue = value == 1 ? true : false;
        _groundRayPos.gameObject.SetActive(setValue);
    }

    /// <summary>
    /// Update 에서 점프후 체공중일 때 땅에 닿음 여부를 판단하는 메서드
    /// </summary>
    private void JudgeGrounded()
    {
        if (!_groundRayPos.gameObject.activeSelf)
            return;

        if (Physics.Raycast(_groundRayPos.position, -_groundRayPos.up, _rayDist, 1 << LayerMask.NameToLayer("Ground")))
        {
            _animator.SetTrigger(_hashJumpEnd);
        }
    }

    #endregion 점프 공격

    #region 주먹 찌르기 공격

    public void Fist()
    {
        _animator.SetTrigger(_hashFist);
    }

    #endregion 주먹 찌르기 공격

    #region 밀기 공격

    public void Push()
    {
        _animator.SetTrigger(_hashPush);
    }

    #endregion 밀기 공격

    #endregion 일반적인 상태일 때 보스의 스킬들

    #region 지친 상태일 때 보스의 스킬들

    #region 휴식 하기

    public void Rest()
    {
        print("지쳤어 쉴래");
    }

    #endregion 휴식 하기

    #region 도망 치기

    public void Run()
    {
        print("도망쳐~");
    }

    #endregion 휴식 하기

    #endregion 지친 상태일 때 보스의 스킬들
}
