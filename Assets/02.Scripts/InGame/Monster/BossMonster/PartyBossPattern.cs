using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ƽ ������ ���� ����� �����ϴ� �Լ�
/// </summary>
public class PartyBossPattern : MonoBehaviour
{
    [Header("=== Dialog Ballon ===")]
    [SerializeField]
    private TextAsset _dialogFile;

    [SerializeField]
    private Transform _floatPos;

    [SerializeField]
    [Tooltip("=== ��ǳ�� ��� ���ӽð� ===")]
    private float _floatTime;

    private float _currFloatTime;
    private bool _isTalking;
    private bool _stopLettering;

    [Header("=== Blink Particle ===")]
    [SerializeField]
    [Tooltip("��ũ �� �� & ��ũ �� Ŀ���� ������ ����Ʈ")]
    private GameObject[] _stoneHit;

    [SerializeField]
    [Tooltip("��ũ�� �ڿ��� ��Ÿ���� �÷��̾�� �̰��� �Ÿ�")]
    private float _blinkOffset;

    [Header("=== Jump Attack ===")]
    [SerializeField]
    [Tooltip("�� �� ������ ����")]
    private float _rayDist;

    [SerializeField]
    [Tooltip("���� ���� ��")]
    private float _jumpAccel;

    [SerializeField]
    [Tooltip("���� ����")]
    private float _jumpHeight;

    private float _jumpSpeedTimes;
    private bool _isJump;
    private Vector3 _endPos;
    private Vector3 _startPos;

    [Header("=== Mini Boss Summon ===")]
    [SerializeField]
    [Tooltip("�̴� ���� ��ȯ �ǽ��� �ϴ� ��ġ = ����")]
    private Transform[] _summonCastPos;

    [SerializeField]
    [Tooltip("��ȯ�ϱ� �� ����Ʈ")]
    private GameObject _summoningEffect;

    [SerializeField]
    [Tooltip("��ȯ ĳ���� ���� �� ���͸� ���ǵ�")]
    private float _letteringSpeed;

    [HideInInspector]
    [Tooltip("��ȯ�� �����ߴ���")]
    public bool _isSummonStart;

    [SerializeField]
    [Tooltip("��ȯ�� �� �¾����� ���׼� ���")]
    public string[] _fireHitReaction;

    [SerializeField]
    [Tooltip("��ȯ ���� ��ȭ��� ���� ������ Ƚ��")]
    private int _stopSummonCount;

    private WaitForSeconds _ws;
    private bool _summonReady; // ������ �̴� ���� ��ȯ �غ� ������
    private int _summonPosIndex; // �������� ��ġ �̵��� �ε���
    private bool _isFireHit; // �� �¾Ҵ���
    private int _fireHitCount; // �� ���� Ƚ��
    
    // �� ���� phase ���θ� �޾Ƴ��Ƶ� �ɵ�, ����� ���� ��ų�� ��ȭ�Ǵ� ��ȭ �ϱ� ����

    // Field
    private GameObject _target;
    private Rigidbody _rbody;
    private Animator _animator;
    private MonsterBase_1 _monsterBase;
    private BossDialog _bossDialog;
    private List<IndexingDict> _dialogData;

    private enum eDialogSituation 
    { 
        SummonPlace,            // �̴� ���� ��ȯ ��ҷ� �̵��ϸ鼭
        StartSummon,            // �̴� ���� ��ȯ�� ������ ��
        Summoning,              // �̴� ���� ��ȯ ��
        Complete_Summon,        // �̴� ���� ��ȯ �Ϸ�
        Run_Shy,                // �������� or ��ø���
        Take_a_Breath,          // ��������
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
    private readonly int _hashFireHitCount = Animator.StringToHash("FireHitCount");
    private readonly int _hashIsFireHit = Animator.StringToHash("isFireHit");

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
        // ���� ���� ��
        JumpParbola();

        // �̴� ���� ��ȯ�Ҷ��� ����
        GoSummonCastPos();

        // ��ǳ�� ���� ������Ű��
        MaintainDialog();
    }

    #region Anim �������� ���������� ���̴� �޼���

    #region 1. ���ݿ� ������ �ʿ��� �� ����ϴ� �������� �޼���

    /// <summary>
    /// �� �޼��带 ȣ���ϴ� �������� ������ �Ѵ�. ������ ������ ������ ��ǥ������ �Ÿ���, �Ǽ��� ������ �Ÿ����� ���µ� �ɸ��� �� �ð����̴�.
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
            _rbody.AddForce(force * transform.forward, ForceMode.Impulse);
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion 1. ���ݿ� ������ �ʿ��� �� ����ϴ� �������� �޼���

    #region 2. ������ �� �ݸ����� Ű�� ���� �޼���

    public void SetAttackCollActive(AnimationEvent myEvent)
    {
        GameObject coll = myEvent.objectReferenceParameter as GameObject;
        bool activeValue = myEvent.intParameter == 1 ? true : false;

        coll.gameObject.SetActive(activeValue);
    }

    #endregion 2. ������ �� �ݸ����� Ű�� ���� �޼���

    #region 3. ��ǳ��

    /// <summary>
    /// ���� �ϰ��ϴ� �޼���
    /// </summary>
    private void ShowDialog(string text, bool turnOn)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatPos.position);

        _isTalking = turnOn;
        _currFloatTime = _floatTime;
        UIScene._instance.FloatTextUI(UIScene._instance._dialogBallon, turnOn, pos, text);
    }

    /// <summary>
    /// ���� �� ��, UI �� ���� �ϴ� �޼���
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
    /// �ѱ��ھ� ��ȭ���� �����
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator LetteringDialog(string text)
    {
        StringBuilder sb = new StringBuilder();
        int index = 0;

        ShowDialog("", false);
        while (index < text.Length)
        {
            if (_stopLettering)
            {
                _stopLettering = false;
                break;
            }

            // ���߿� �ҿ� ������
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

        ShowDialog("", false);
    }

    #endregion 3. ��ǳ��

    #endregion Anim �������� ���������� ���̴� �޼���

    #region �Ϲ����� ������ �� ������ ��ų��

    #region ��ũ ����

    public void Blink()
    {
        _animator.SetTrigger(_hashBlink);
    }

    public void BlinkAppear()
    {
        _animator.SetTrigger(_hashBlinkBack);
    }

    /// <summary>
    /// ��ũ �� �� ������ ��ƼŬ ����Ʈ�� ���ִ� �ִϸ��̼� ��������Ʈ
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

    #endregion ��ũ ����

    #region �̴� ���� ��ȯ�ϱ�

    public void SummonMiniBoss()
    {
        int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.SummonPlace]._dialogs.Count);
        string text = _dialogData[(int)eDialogSituation.SummonPlace]._dialogs[randomValue];

        _summonReady = true;
        _monsterBase._nav.enabled = true;
        ShowDialog(text, true);
    }

    /// <summary>
    /// �̴� ���� ��ȯ�������� �̵� & ��ȯ �ǽ� ����
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

                // ��ȯ �ǽ� ����
                case 1:
                    int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.StartSummon]._dialogs.Count);
                    string text = _dialogData[(int)eDialogSituation.StartSummon]._dialogs[randomValue];

                    _monsterBase._animator.SetBool(_monsterBase._hashMove, false);
                    _monsterBase._nav.enabled = false;
                    _animator.SetTrigger(_hashCeremony);
                    _animator.ResetTrigger(_hashIsFireHit);
                    _animator.SetInteger(_hashFireHitCount, _fireHitCount);
                    _summonReady = false;
                    ShowDialog(text, true);
                    return;
            }
        }

        _monsterBase.Move(_summonCastPos[_summonPosIndex].position, (_monsterBase._stat.movSpeed / (_summonPosIndex + 1)) + _summonPosIndex);
    }

    /// <summary>
    /// �ǽ� ���� �ִϸ��̼��� ������ �����ӿ� �޾Ƴ��� animation event
    /// </summary>
    public void SummonStart(int value)
    {
        bool isContinue = value == 1 ? true : false;
        _isSummonStart = isContinue;
        _summoningEffect.SetActive(isContinue);

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
    /// ��ȯ ���� ��ȭ�� ������ ���� �Լ�
    /// </summary>
    public void HitFireDuringSummon()
    {
        _isFireHit = true;
        _fireHitCount++;
        _animator.SetInteger(_hashFireHitCount, _fireHitCount);
        _animator.SetTrigger(_hashIsFireHit);

        if (_fireHitCount == _stopSummonCount)
        {
            _fireHitCount = 0;
            _isFireHit = false;
            _stopLettering = true;
            SummonStart(0);
        }
    }

    #endregion �̴� ���� ��ȯ�ϱ�

    #region ���ű ����

    public void DropKick()
    {
        _animator.SetTrigger(_hashDropKick);
    }

    #endregion ���ű ����

    #region �Ϲ� ���� ��ȯ�ϱ�

    public void SummonNormalMonster()
    {
        print("�븻 �� ��ȯ ��");
    }

    #endregion �Ϲ� ���� ��ȯ�ϱ�

    #region �����̵� ����

    public void Sliding()
    {
        _animator.SetTrigger(_hashSliding);
    }

    #endregion �����̵� ����

    #region ���� ����

    // ���� �� �������� ���� �˷��ִ� �׸��� ������Ʈ�� �ʿ�

    public void Jump()
    {
        _animator.SetBool(_hashJumpEnd, false);
        _animator.SetTrigger(_hashJump);
        _isJump = false;
    }

    /// <summary>
    /// JumpStart Anim�� ���� �����ӿ� ���� delegate �Լ�
    /// </summary>
    public void JumpStart()
    {
        _isJump = true;
        _startPos = transform.position;
        _endPos = _target.transform.position;
        _jumpSpeedTimes = 0.0f;
    }

    /// <summary>
    /// Update ���� ������ ü������ �� ���� ���� ���θ� �Ǵ��ϴ� �޼���
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

    #endregion ���� ����

    #region �ָ� ��� ����

    public void Fist()
    {
        _animator.SetTrigger(_hashFist);
    }

    #endregion �ָ� ��� ����

    #region �б� ����

    public void Push()
    {
        _animator.SetTrigger(_hashPush);
    }

    #endregion �б� ����

    #endregion �Ϲ����� ������ �� ������ ��ų��

    #region ��ģ ������ �� ������ ��ų��

    #region �޽� �ϱ�

    public void Rest()
    {
        print("���ƾ� ����");
    }

    #endregion �޽� �ϱ�

    #region ���� ġ��

    public void Run()
    {
        print("������~");
    }

    #endregion �޽� �ϱ�

    #endregion ��ģ ������ �� ������ ��ų��
}
