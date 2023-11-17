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
    private GameObject _dialogCanvas;

    [SerializeField]
    private float _floatTime;

    [Header("=== Blink Particle ===")]
    [SerializeField]
    [Tooltip("��ũ �� �� & ��ũ �� Ŀ���� ������ ����Ʈ")]
    private GameObject[] _stoneHit;

    [SerializeField]
    [Tooltip("��ũ�� �ڿ��� ��Ÿ���� �÷��̾�� �̰��� �Ÿ�")]
    private float _blinkOffset;

    [Header("=== Jump Attack ===")]
    [SerializeField]
    [Tooltip("���� �Ŀ�")]
    private float _jumpForce;

    [SerializeField]
    [Tooltip("���� �� ü������ �� ���� ������ �Ǵ��ϱ� ���� �� �� ������ ��ġ")]
    private Transform _groundRayPos;

    [SerializeField]
    [Tooltip("�� �� ������ ����")]
    private float _rayDist;

    [Header("=== Mini Boss Summon ===")]
    [SerializeField]
    [Tooltip("�̴� ���� ��ȯ �ǽ��� �ϴ� ��ġ = ����")]
    private Transform[] _summonCastPos;

    // �� ���� phase ���θ� �޾Ƴ��Ƶ� �ɵ�, ����� ���� ��ų�� ��ȭ�Ǵ� ��ȭ �ϱ� ����

    // Field
    private GameObject _target;
    private Animator _animator;
    private MonsterBase_1 _monsterBase;
    private Rigidbody _rbody;
    private bool _isMiniBossSummon; // ������ �̴� ���� ��ȯ������
    private int _summonPosIndex; // �������� ��ġ �̵��� �ε���
    private BossDialog _bossDialog;
    private Dictionary<string, List<string>> _dialogDict; // ��Ȳ�� key��, ��ȭ ���� list�� value�� ���� ������ ��ǳ�� �ؽ�Ʈ ������ ��ųʸ�
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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _monsterBase = GetComponent<MonsterBase_1>();
        _rbody = GetComponent<Rigidbody>();
        _bossDialog = new BossDialog();

        _target = _monsterBase._target;
        _dialogDict = _bossDialog.Parsing(_dialogFile);
    }

    private void Update()
    {
        // ���� ���϶��� ����
        JudgeGrounded();

        // �̴� ���� ��ȯ�Ҷ��� ����
        GoSummonCastPos();
    }

    #region Anim �������� ���������� ���̴� �޼���

    #region 1. ���ݿ� ������ �ʿ��� �� ����ϴ� �������� �޼���

    /// <summary>
    /// �� �޼��带 ȣ���ϴ� �������� ������ �Ѵ�. ������ ������ ������ ��ǥ������ �Ÿ���, �Ǽ��� ������ �Ÿ����� ���µ� �ɸ��� �� �ð����̴�.
    /// </summary>
    /// <param name="myEvent"></param>
    public void AddRush(AnimationEvent myEvent)
    {
        int dist = myEvent.intParameter;
        float time = myEvent.floatParameter;
        Vector3 target = transform.position + transform.forward * dist;
        StartCoroutine(RushToTarget(target, time));
    }

    /// <summary>
    /// ��ǥ���� �ð��� ���� ����
    /// </summary>
    private IEnumerator RushToTarget(Vector3 target, float time)
    {
        Vector3 refVector = Vector3.zero;
        while (Vector3.Distance(transform.position, target) >= 0.1f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target, ref refVector, time); // ��ũ ��ġ�� 0.08, �����̵��� ���� ����
            yield return null;
        }

        transform.position = target;
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
        _isMiniBossSummon = true;
    }

    /// <summary>
    /// �̴� ���� ��ȯ�������� �̵� & ��ȯ �ǽ� ����
    /// </summary>
    private void GoSummonCastPos()
    {
        if (!_isMiniBossSummon)
            return;

        float dist = Vector3.Distance(transform.position, _summonCastPos[_summonPosIndex].position);
        if (dist <= _monsterBase._nav.stoppingDistance + _monsterBase._nav.baseOffset * 0.5f)
        {
            transform.position = _summonCastPos[_summonPosIndex].position;
            switch (_summonPosIndex)
            {
                case 0:
                    _summonPosIndex++;
                    return;

                case 1:
                    _monsterBase._animator.SetBool(_monsterBase._hashMove, false);

                    // ���⼭ ��ǳ������ ������� �ǽ� ����
                    //_animator.SetTrigger(_hashCeremony);
                    return;
            }
        }

        _monsterBase.Move(_summonCastPos[_summonPosIndex].position, _monsterBase._stat.movSpeed / (_summonPosIndex + 1));
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

    // ������ ���ư��� �����̵� �����ϰ� �ؾ� ��

    public void Sliding()
    {
        _animator.SetTrigger(_hashSliding);
    }

    #endregion �����̵� ����

    #region ���� ����

    // ���� �� �������� ���� �˷��ִ� �׸��� ������Ʈ��
    // �������� ������ �̵��ϴ� ����� �ʿ�
    // ������ �ӵ��� ������

    public void Jump()
    {
        _animator.ResetTrigger(_hashJumpEnd);
        _animator.SetTrigger(_hashJump);
    }

    /// <summary>
    /// JumpStart Anim�� ���� �����ӿ� ���� delegate �Լ�
    /// </summary>
    public void JumpStart()
    {
        _rbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// ���� �� or ���� ���� ���·� ����� �޼���
    /// </summary>
    /// <param name="value"></param>
    public void SetJumpState(int value)
    {
        bool setValue = value == 1 ? true : false;
        _groundRayPos.gameObject.SetActive(setValue);
    }

    /// <summary>
    /// Update ���� ������ ü������ �� ���� ���� ���θ� �Ǵ��ϴ� �޼���
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
