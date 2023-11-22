using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 파티 몬스터의 공격 기술을 정의하는 함수
/// </summary>
public class PartyBossPattern : MonoBehaviour
{
    #region 말풍선 관련 전역 변수들
    [Header("=== Dialog Ballon ===")]
    [SerializeField]
    private TextAsset _dialogFile;

    [SerializeField]
    [Tooltip("말풍선 띄울 위치")]
    private Transform _floatPos;

    [SerializeField]
    [Tooltip("말풍선 띄울 지속시간")]
    private float _floatTime;

    [SerializeField]
    [Tooltip("Rest 때 말 할 확률")]
    [Range(0.0f, 1.0f)]
    private float _restDialogPercentage;

    [SerializeField]
    [Tooltip("약올리기 도망가기때 말 할 확률")]
    [Range(0.0f, 1.0f)]
    private float _runShyDialogPercentage;

    private float _currFloatTime;
    private bool _isTalking;
    private bool _stopLettering;
    #endregion 말풍선 관련 전역 변수들

    #region 공격 콜라이더 관련 전역 변수들
    [Header("=== Attack Coll Arr ===")]
    [SerializeField]
    private GameObject[] _attackColls;

    private enum eAttackColl { Left, Right, Left_Right, Foot, Head, Drop_Kick, }
    #endregion 공격 콜라이더 관련 전역 변수들

    #region 블링크 공격 관련 전역 변수들
    [Header("=== Blink Particle ===")]
    [SerializeField]
    [Tooltip("블링크 할 때 & 블링크 후 커질때 나오는 이펙트")]
    private GameObject[] _stoneHit;

    [SerializeField]
    [Tooltip("블링크로 뒤에서 나타날때 플레이어와 이격할 거리")]
    private float _blinkOffset;
    #endregion 블링크 공격 관련 전역 변수들

    #region 점프 공격 관련 전역 변수들
    [Header("=== Jump Attack ===")]
    [SerializeField]
    [Tooltip("발 밑 레이의 길이")]
    private float _rayDist;

    [SerializeField]
    [Tooltip("점프 가속 값")]
    private float _jumpAccel;

    [SerializeField]
    [Tooltip("점프 높이")]
    private float _jumpHeight;

    private float _jumpSpeedTimes;
    private bool _isJump;
    private Vector3 _endPos;
    private Vector3 _startPos;
    #endregion 점프 공격 관련 전역 변수들

    #region 미니 보스 소환 관련 전역 변수들
    [Header("=== Mini Boss Summon ===")]
    [SerializeField]
    [Tooltip("미니 보스 소환 의식을 하는 위치 = 제단")]
    private Transform[] _summonCastPos;

    [SerializeField]
    [Tooltip("소환하기 전 이펙트")]
    private GameObject _summoningEffect;

    [SerializeField]
    [Tooltip("소환물")]
    private GameObject _summonObj;

    [SerializeField]
    [Tooltip("소환 캐스팅 중일 때 레터링 스피드")]
    private float _letteringSpeed;

    [SerializeField]
    [Tooltip("소환 하기위해 필요한 시간 = (주문의 글자 수 x 레터링 스피드 값)")]
    private float _castingTime;

    [Tooltip("현재 캐스팅 된 시간값")]
    public float _currCastingTime;

    [HideInInspector]
    [Tooltip("소환을 시작했는지")]
    public bool _isSummonStart;

    [SerializeField]
    [Tooltip("소환중 불 맞았을때 리액션 대사")]
    private string[] _fireHitReaction;

    [SerializeField]
    [Tooltip("소환중 맞았을 때 게이지를 흔들 세기")]
    private float[] _gaugeShakeAmount;

    [SerializeField]
    [Tooltip("소환중 맞았을 때 게이지 흔드는 지속 시간")]
    private float[] _gaugeShakeDur;

    private WaitForSeconds _ws;
    private bool _summonReady; // 보스가 미니 보스 소환 준비 중인지
    private int _summonPosIndex; // 제단으로 위치 이동용 인덱스
    private bool _isFireHit; // 불 맞았는지
    #endregion 미니 보스 소환 관련 전역 변수들

    #region 도망치기 & 약올리기 관련 전역 변수들

    [Header("=== Run & shy ===")]
    [SerializeField]
    [Tooltip("랜덤으로 도망칠 곳 최소 범위")]
    private float _runRangeMin;

    [SerializeField]
    [Tooltip("랜덤으로 도망칠 곳 최대 범위")]
    private float _runRangeMax;

    #endregion 도망치기 & 약올리기 관련 전역 변수들

    // Field
    private GameObject _target;
    private Rigidbody _rbody;
    private Animator _animator;
    private MonsterBase_1 _monsterBase;
    private BossDialog _bossDialog;
    private List<IndexingDict> _dialogData;

    private enum eDialogSituation 
    { 
        SummonPlace,            // 미니 보스 소환 장소로 이동하면서
        StartSummon,            // 미니 보스 소환을 시작할 때
        Summoning,              // 미니 보스 소환 중
        Complete_Summon,        // 미니 보스 소환 완료
        Run_Shy,                // 도망가기 or 약올리기
        Take_a_Breath,          // 숨돌리기
    }

    // Anim Params
    private readonly int _hashBlink = Animator.StringToHash("Blink Trigger");
    private readonly int _hashBlinkBack = Animator.StringToHash("Blink Back Pos");
    private readonly int _hashSliding = Animator.StringToHash("Sliding Trigger");
    private readonly int _hashJump = Animator.StringToHash("Jump Trigger");
    private readonly int _hashJumpEnd = Animator.StringToHash("Jump End");
    private readonly int _hashFist = Animator.StringToHash("Fist Trigger");
    private readonly int _hashPush = Animator.StringToHash("Push Trigger");
    private readonly int _hashDropKick = Animator.StringToHash("Drop Kick Trigger");
    private readonly int _hashCeremony = Animator.StringToHash("Ceremony Trigger");
    private readonly int _hashIsFireHit = Animator.StringToHash("isFireHit");
    private readonly int _hashCompleteSummon = Animator.StringToHash("Complete Summon");
    private readonly int _hashFailSummon = Animator.StringToHash("Failed Summon");
    private readonly int _hashRest = Animator.StringToHash("Rest");
    private readonly int _hashRun = Animator.StringToHash("Run");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _monsterBase = GetComponent<MonsterBase_1>();
        _bossDialog = new BossDialog();
        _rbody = GetComponent<Rigidbody>();
        _target = _monsterBase._target;
        _dialogData = _bossDialog.DialogParsing(_dialogFile);
        _ws = new WaitForSeconds(_letteringSpeed);
    }

    private void Update()
    {
        // 점프 중일 때
        JumpParbola();

        // 미니 보스 소환할때만 실행
        GoSummonCastPos();

        // 미니 보스 소환중
        Summon();

        // 말풍선 띄우고 유지시키기
        MaintainDialog();
    }

    #region Anim 여러개에 공통적으로 쓰이는 메서드

    #region 1. 공격에 돌진이 필요할 때 사용하는 공통적인 메서드

    /// <summary>
    /// 이 메서드를 호출하는 순간부터 돌진을 한다. 정수형 변수는 돌진할 목표까지의 거리고, 실수형 변수는 거리까지 가는데 걸리게 할 시간값이다.
    /// </summary>
    /// <param name="myEvent"></param>
    public void AddRush(AnimationEvent myEvent)
    {
        int y0 = myEvent.intParameter;
        float time = myEvent.floatParameter;

        StartCoroutine(Rush(y0, time));
    }

    private IEnumerator Rush(int y0, float time)
    {
        int count = (int)(time / Time.fixedDeltaTime);
        for (int i = 0; i < count; i++)
        {
            float force = -(y0 / count) * i + y0;
            _rbody.AddForce(force * transform.forward, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion 1. 공격에 돌진이 필요할 때 사용하는 공통적인 메서드

    #region 2. 공격할 때 콜리더를 키고 끄는 메서드

    // index 번째의 게임오브젝트가 꺼져있으면 켜주고, 켜져있으면 꺼주기
    public void SetAttackCollActive(int collIndex)
    {
        switch ((eAttackColl)collIndex)
        {
            case eAttackColl.Left_Right:
                bool leftEnable = _attackColls[(int)eAttackColl.Left].activeSelf;
                bool rightEnable = _attackColls[(int)eAttackColl.Right].activeSelf;

                _attackColls[(int)eAttackColl.Left].SetActive(!leftEnable);
                _attackColls[(int)eAttackColl.Right].SetActive(!rightEnable);
                return;
        }

        bool activeSelf = _attackColls[collIndex].activeSelf;
        _attackColls[collIndex].SetActive(!activeSelf);
    }

    #endregion 2. 공격할 때 콜리더를 키고 끄는 메서드

    #region 3. 말풍선

    /// <summary>
    /// 말을 하게하는 메서드
    /// </summary>
    private void ShowDialog(string text, bool turnOn)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatPos.position);

        _isTalking = turnOn;
        _currFloatTime = _floatTime;
        UIScene._instance.FloatTextUI(UIScene._instance._dialogBallon, turnOn, pos, text);
    }

    /// <summary>
    /// 말을 할 시, UI 를 띄우게 하는 메서드
    /// </summary>
    private void MaintainDialog()
    {
        if (!_isTalking)
            return;
        if (_currFloatTime <= 0.0f)
        {
            _currFloatTime = _floatTime;
            _isTalking = false;
            UIScene._instance.FloatTextUI(UIScene._instance._dialogBallon, false, Vector3.zero, "");
            return;
        }

        Vector3 pos = Camera.main.WorldToScreenPoint(_floatPos.position);
        UIScene._instance.FloatTextUI(UIScene._instance._dialogBallon, true, pos, "");
        _currFloatTime -= Time.deltaTime;
    }

    /// <summary>
    /// 한글자씩 대화문을 띄워줌
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator LetteringDialog(string text)
    {
        StringBuilder sb = new StringBuilder();
        int index = 0;

        ShowDialog("", false);
        while (true)
        {
            if (_stopLettering)
            {
                _stopLettering = false;
                break;
            }

            if (index >= text.Length)
            {
                ShowDialog("", false);
                index = 0;
                sb.Clear();
            }

            // 도중에 불에 맞으면
            if (_isFireHit)
            {
                int randomValue = Random.Range(0, _fireHitReaction.Length);
                string reaction = _fireHitReaction[randomValue];

                _isFireHit = false;
                sb.Append(reaction);
                ShowDialog(sb.ToString(), true);
                index++;

                continue;
            }

            sb.Append(text[index]);
            ShowDialog(sb.ToString(), true);
            index++;

            yield return _ws;
        }
    }

    #endregion 3. 말풍선

    #region 4. 플레이어에게 이동하기

    public void GoToTarget()
    {

    }

    #endregion 4. 플레이어에게 이동하기

    #endregion Anim 여러개에 공통적으로 쓰이는 메서드

    #region 일반적인 상태일 때 보스의 스킬들

    #region 블링크 공격

    public void Blink()
    {
        _animator.SetTrigger(_hashBlink);
    }

    /// <summary>
    /// 블링크로 나타나는 애니메이션 전환용 트리거
    /// </summary>
    public void BlinkAppear()
    {
        _animator.SetTrigger(_hashBlinkBack);
    }

    /// <summary>
    /// 블링크 할 때 나오는 파티클 이펙트를 켜주는 애니메이션 델리게이트
    /// </summary>
    public void ActiveBlinkParticle(int index)
    {
        _stoneHit[index].transform.position = transform.position + Vector3.up * 2.0f;
        _stoneHit[index].SetActive(true);
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
        int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.SummonPlace]._dialogs.Count);
        string text = _dialogData[(int)eDialogSituation.SummonPlace]._dialogs[randomValue];

        _summonReady = true;
        _monsterBase._nav.enabled = true;
        _animator.SetBool(_hashFailSummon, false);
        ShowDialog(text, true);
    }

    /// <summary>
    /// 미니 보스 소환지역으로 이동 & 소환 의식 시작
    /// </summary>
    private void GoSummonCastPos()
    {
        if (!_summonReady)
            return;

        float dist = Vector3.Distance(transform.position, _summonCastPos[_summonPosIndex].position);
        if (dist <= _monsterBase._nav.radius * 0.5f)
        {
            transform.position = _summonCastPos[_summonPosIndex].position;
            switch (_summonPosIndex)
            {
                case 0:
                    _summonPosIndex++;
                    return;

                // 소환 의식 시작
                case 1:
                    int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.StartSummon]._dialogs.Count);
                    string text = _dialogData[(int)eDialogSituation.StartSummon]._dialogs[randomValue];

                    _monsterBase._animator.SetBool(_monsterBase._hashMove, false);
                    _monsterBase._nav.enabled = false;
                    _animator.SetTrigger(_hashCeremony);
                    _animator.ResetTrigger(_hashIsFireHit);
                    _summonReady = false;
                    ShowDialog(text, true);
                    return;
            }
        }

        _monsterBase.Move(_summonCastPos[_summonPosIndex].position, (_monsterBase._stat.movSpeed / (_summonPosIndex + 1)) + _summonPosIndex);
    }

    /// <summary>
    /// 의식 시작 애니메이션의 마지막 프레임에 달아놓는 animation event
    /// </summary>
    public void SummonStart(int value)
    {
        bool isContinue = value == 1 ? true : false;
        _isSummonStart = isContinue;
        _summoningEffect.SetActive(isContinue);

        UIScene._instance.SetGaugeUI(isContinue);
        if (value != 1 && value != 0)
            return;

        if (isContinue)
        {
            StartCoroutine(LetteringDialog(_dialogData[(int)eDialogSituation.Summoning]._dialogs[0]));
        }
        else
        {
            _currFloatTime = _floatTime;
            _isTalking = false;
            UIScene._instance.FloatTextUI(UIScene._instance._dialogBallon, false, Vector3.zero, "");
        }
    }

    /// <summary>
    /// 소환 진행중일때 호출하는 메서드
    /// </summary>
    private void Summon()
    {
        if (!_isSummonStart)
        {
            _currCastingTime = 0.0f;
            return;
        }

        if (_currCastingTime >= _castingTime)
        {
            CompleteSummonMiniBoss();
            return;
        }

        _currCastingTime += Time.deltaTime;
        UIScene._instance.SetGaugeFill(_currCastingTime, _castingTime);
    }

    /// <summary>
    /// 소환 도중 맞을때 관련 함수
    /// </summary>
    public void HitDuringSummon(bool isFire, float decreaseCasting)
    {
        float shakeAmount;
        float shakeDur;

        if (isFire)
        {
            shakeAmount = _gaugeShakeAmount[(int)eArrowState.Fire];
            shakeDur = _gaugeShakeDur[(int)eArrowState.Fire];

            _animator.SetTrigger(_hashIsFireHit);
            _isFireHit = true;
        }
        else
        {
            shakeAmount = _gaugeShakeAmount[(int)eArrowState.Normal];
            shakeDur = _gaugeShakeDur[(int)eArrowState.Normal];
        }

        _currCastingTime -= decreaseCasting * Time.deltaTime;
        if (_currCastingTime < 0.0f)
        {
            _animator.SetBool(_hashFailSummon, true);
            _isFireHit = false;
            _stopLettering = true;
            _currCastingTime = 0.0f;
            SummonStart(0);
            return;
        }

        StartCoroutine(UIScene._instance.ChangeGaugeColor("FFFFFF"));
        StartCoroutine(UIScene._instance.ShakeGaugeUI(shakeAmount, shakeDur));
    }

    /// <summary>
    /// 소환 완료시 호출하는 메서드
    /// </summary>
    private void CompleteSummonMiniBoss()
    {
        int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.Complete_Summon]._dialogs.Count);
        string text = _dialogData[(int)eDialogSituation.Complete_Summon]._dialogs[randomValue];

        _animator.SetTrigger(_hashCompleteSummon);
        _currCastingTime = 0.0f;
        _stopLettering = true;
        _isFireHit = false;
        _summonObj.SetActive(true);

        ShowDialog(text, true);
        SummonStart(-1);
    }

    #endregion 미니 보스 소환하기

    #region 드랍킥 공격

    public void DropKick()
    {
        _animator.SetTrigger(_hashDropKick);
    }

    #endregion 드랍킥 공격

    #region 일반 몬스터 소환하기

    public void SummonNormalMonster()
    {

    }

    #endregion 일반 몬스터 소환하기

    #region 슬라이딩 공격

    public void Sliding()
    {
        _animator.SetTrigger(_hashSliding);
    }

    #endregion 슬라이딩 공격

    #region 점프 공격

    // 점프 후 내려찍힐 곳을 알려주는 그림자 오브젝트가 필요

    public void Jump()
    {
        _animator.SetBool(_hashJumpEnd, false);
        _animator.SetTrigger(_hashJump);
        _isJump = false;
    }

    /// <summary>
    /// JumpStart Anim의 시작 프레임에 붙힐 delegate 함수
    /// </summary>
    public void JumpStart()
    {
        _isJump = true;
        _startPos = transform.position;
        _endPos = _target.transform.position;
        _jumpSpeedTimes = 0.0f;
    }

    /// <summary>
    /// Update 에서 점프후 체공중일 때 땅에 닿음 여부를 판단하는 메서드
    /// </summary>
    private void JumpParbola()
    {
        if (!_isJump)
            return;

        Vector3 startPos = _startPos;
        Vector3 endPos = _endPos;
        Vector3 center = (startPos + endPos) * 0.5f;

        center = new Vector3(center.x, center.y - _jumpHeight, center.z);
        startPos = startPos - center;
        endPos = endPos - center;

        transform.position = Vector3.Slerp(startPos, endPos, _jumpSpeedTimes);
        center.y += _jumpHeight * Mathf.Sin(Mathf.PI * _jumpSpeedTimes);
        transform.position += center;

        _jumpSpeedTimes += Time.deltaTime * _jumpAccel;
        _animator.SetBool(_hashJumpEnd, _jumpSpeedTimes >= 0.9f);
        if (_jumpSpeedTimes >= 1.0f)
        {
            _isJump = false;
            _jumpSpeedTimes = 1.0f;
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
        if (Random.Range(0.0f, 1.0f) <= _restDialogPercentage)
        {
            int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.Take_a_Breath]._dialogs.Count);
            string text = _dialogData[(int)eDialogSituation.Take_a_Breath]._dialogs[randomValue];
            ShowDialog(text, true);
        }

        _animator.SetTrigger(_hashRest);
    }

    #endregion 휴식 하기

    #region 도망 치기

    public void Run()
    {
        _animator.SetTrigger(_hashRun);
    }

    public void MoveToRandomPos()
    {
        float range = Random.Range(_runRangeMin, _runRangeMax);
        Vector3 randomPos = transform.position + Random.insideUnitSphere * range;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPos, out hit, range, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            transform.forward = _monsterBase._target.transform.position - transform.position;
        }
    }

    public void Say(int value)
    {
        int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.Run_Shy]._dialogs.Count);
        string text = _dialogData[(int)eDialogSituation.Run_Shy]._dialogs[randomValue];
        bool isOn = value == 1 ? true : false;

        ShowDialog(text, isOn);
    }

    #endregion 도망 치기

    #endregion 지친 상태일 때 보스의 스킬들
}
