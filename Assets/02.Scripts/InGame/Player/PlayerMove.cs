using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("=== Move, Rotate ===")]
    public float _movSpeed;
    public float _rotSpeed;
    [Range(0.0f, 90.0f)] public float _maxSlope;
    [HideInInspector] public float _v;
    [HideInInspector] public float _h;
    [SerializeField] private float _wallRayDintance;
    [SerializeField] private GameObject _groundCheckBox;
    private bool _isGrounded;
    private bool _isOnSlope;
    private Vector3 _dir; // �÷��̾��� wasd�������� ���Ե� ���⺤�Ͱ��� ����
    private Vector3 _gravity; // rigidbody.velocity �� ���� �����ϱ⶧���� �߷¶��� ���� ����
    private Ray _groundRay;
    private RaycastHit _groundHit;

    [Header("=== Dodge ===")]
    public float _dodgeSpeed; // ������ ���ӽ�ų �� (dodgeSpeed = _dodgeSpeed * _movSpeed)
    public float _dodgeDur; // ��������°� ���ӵ� �ð�
    public float _dodgeCoolDown;
    private float _originDodgeCoolDown;
    private bool _dodgeAttackEnd;
    
    [Header("=== Follow Cam ===")]
    public GameObject _followCamObj;

    [Header("=== Component ===")]
    private Animator _animator;
    private Rigidbody _rbody;
    private PlayerCombat _combat;
    private PlayerFSM _state;
    private FollowCamera _followCam;
    private FallBehaviour _fallBehaviour;
    private CapsuleCollider _capsuleColl;
    private SmoothDodgeBehaviour _smoothDodgeBehaviour;
    private WaitForFixedUpdate _wfs;

    [Header("=== Animator Params ===")]
    readonly int _hashMove = Animator.StringToHash("isMove");
    readonly int _hashYVelocity = Animator.StringToHash("yVelocity");
    readonly int _hashRoll = Animator.StringToHash("isRoll");

    void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _state = GetComponent<PlayerFSM>();
        _combat = GetComponent<PlayerCombat>();
        _followCam = _followCamObj.GetComponent<FollowCamera>();
        _fallBehaviour = _animator.GetBehaviour<FallBehaviour>();
        _smoothDodgeBehaviour = _animator.GetBehaviour<SmoothDodgeBehaviour>();
        _capsuleColl = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        _originDodgeCoolDown = _dodgeCoolDown;
        _wfs = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (_state.State == PlayerFSM.eState.Hit)
            return;
        if (_state.State == PlayerFSM.eState.Dead)
            return;

        // idle, fall, move���°� �ƴ϶�� �����̰� ������ �� ����
        if (_state.State != PlayerFSM.eState.Idle && _state.State != PlayerFSM.eState.Fall && _state.State != PlayerFSM.eState.Move)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);
        
        ManipulPhysics();

        // h�� v�� �Է��� ��
        if (_h != 0.0f || _v != 0.0f)
        {
            MovePlayer();
            RotatePlayer();
        }
        else
        {
            // ���̳� ������� ������, ��ǲ�� �ԷµǾ��ٰ� ��������!
            if (_isGrounded || _isOnSlope)
            {
                _dir = Vector3.zero;
                _rbody.velocity = Vector3.zero;
            }
        }

        // h�� v�� �ϳ��� �Էµ����ʰ�, �������»��¿� ���ݻ��¿� ������°� �ƴ϶�� = idle����
        if (_h == 0.0f && _v == 0.0f
            &&
            !_fallBehaviour._isFall && _state.State != PlayerFSM.eState.Attack && _state.State != PlayerFSM.eState.Dead)
        {
            _state.State = PlayerFSM.eState.Idle;
            _animator.SetBool(_hashMove, false);
        }
    }

    void Update()
    {
        if (_state.State == PlayerFSM.eState.Hit)
            return;
        if (_state.State == PlayerFSM.eState.Dead)
            return;

        // _groundRay = new Ray(transform.position, -transform.up * 0.3f);
        if (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move 
            || _state.State == PlayerFSM.eState.Fall || _state.State == PlayerFSM.eState.Charging)
        {
            _h = Input.GetAxisRaw("Horizontal");
            _v = Input.GetAxisRaw("Vertical");
        }

        // ȸ��Ű �Է°���
        if (Input.GetKeyDown(KeyCode.Space) && 
            (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move ||
            _state.State == PlayerFSM.eState.Charging) && _dodgeCoolDown == _originDodgeCoolDown)
            StartCoroutine(Dodge(_h, _v));
    }

    void MovePlayer()
    {
        // �������� �ӷ��� ������ �����̸� fall���·� ��ȯ
        if (_animator.GetFloat(_hashYVelocity) <= -5.0f)
            _state.State = PlayerFSM.eState.Fall;
        else
            _state.State = PlayerFSM.eState.Move;

        _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        _dir = _isOnSlope ? GetSlopeDir(_dir) : _dir;
        _animator.SetBool(_hashMove, _dir != Vector3.zero);
        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);
        
        _rbody.velocity = _dir * _movSpeed + _gravity;
    }

    private bool OnGround()
    {
        Vector3 boxSize = _groundCheckBox.transform.lossyScale;
        return Physics.CheckBox(_groundCheckBox.transform.position, boxSize, Quaternion.identity, 1 << LayerMask.NameToLayer("Ground"));
    }

    private bool OnSlope()
    {
        // �� �Ǵ��� ����ĳ��Ʈ��� �ݸ����ְ� isGrounded ��ũ��Ʈ ����, ���߿� ���� ���������� �߷²���
        if (Physics.Raycast(_groundRay, out _groundHit, 0.3f, 1 << LayerMask.NameToLayer("Ground")))
        {
            float angle = Vector3.Angle(transform.up, _groundHit.normal);
            return angle != 0 && angle < _maxSlope;
        }

        return false;
    }

    private Vector3 GetSlopeDir(Vector3 dir)
    {
        return Vector3.ProjectOnPlane(dir, _groundHit.normal).normalized;
    }

    /// <summary>
    /// velocity�� �̿��� �÷��̾� ���ۿ� ���� �߻��ϴ� ���� ���� �ɾ��ϴ� �޼���
    /// </summary>
    private void ManipulPhysics()
    {
        _groundRay = new Ray(transform.position, Vector3.down);
        _isGrounded = OnGround();
        _isOnSlope = OnSlope();

        _gravity = _isGrounded ? Vector3.zero : Vector3.down * Mathf.Abs(_rbody.velocity.y);
        _rbody.useGravity = !_isOnSlope;
    }

    public IEnumerator Dodge(float h, float v)
    {
        if (_combat._curLongRangeChargingTime >= 0.0f)
            _combat.InitChargingGauge();
        if (_followCam.CamState != FollowCamera.eCameraState.Follow)
            _followCam.CamState = FollowCamera.eCameraState.Follow;

        bool isDodgeAttackInput = false;
        Vector3 dodgeDir; 
        float currDur = 0.0f;
        float dodgeSpeed = _movSpeed * _dodgeSpeed;
        RaycastHit hit;
        float wallAngle;
        Vector3 wallClimbDir;

        // �����⵿��
        StartCoroutine(CoolDownDodge());
        _state.State = PlayerFSM.eState.Dodge;
        _animator.SetBool(_hashRoll, true);

        // Ű�Է��� �� �����̸� ����������� �Է��ѹ�������, �ƴ϶�� ĳ���ͱ��� ��������
        if (_h != 0.0f || _v != 0.0f)
            dodgeDir = ((h * Vector3.right) + (v * Vector3.forward)).normalized;
        else
            dodgeDir = transform.forward;

        transform.forward = dodgeDir;

        while (currDur < _dodgeDur)
        {
            // �������߰�Ÿ �Է°� ����
            if (Input.GetMouseButton(0) && !isDodgeAttackInput)
                isDodgeAttackInput = true;

            currDur += Time.deltaTime;

            // 3���� ����(�Ӹ� ��� �߳�)�� ���� ������� �� = ���� ����
            //if (IsDeadEndRoad())
            //{
            //    _rbody.MovePosition(_rbody.position + Vector3.zero * Time.deltaTime);
            //    yield return _wfs;
            //}

            // �����ٰ� Wall �� ������ ��, ������ üũ �� ����� �� �ִ� ���� �����̸� �ش� ������������ ������
            if (Physics.Raycast(_rbody.position, transform.forward, out hit, _capsuleColl.radius * _wallRayDintance,
                1 << LayerMask.NameToLayer("Ground")))
            {
                if (hit.collider.tag.Equals("Wall"))
                {
                    wallAngle = Vector3.Angle(Vector3.up, hit.normal);
                    if (wallAngle <= _maxSlope)
                    {
                        // �ش簢���� �������̵�
                        wallClimbDir = Vector3.ProjectOnPlane(Vector3.up, hit.normal);
                        wallClimbDir += transform.forward;
                        _rbody.MovePosition(_rbody.position + wallClimbDir * dodgeSpeed * Time.deltaTime);
                    }
                    else
                    {
                        _rbody.MovePosition(_rbody.position + Vector3.zero * Time.deltaTime);
                    }
                }
            }
            // ������ ������
            else
            {
                _rbody.MovePosition(_rbody.position + dodgeDir * dodgeSpeed * Time.deltaTime);
            }

            #region 23.08.13 �Ӹ� ��� �߳� 3���� ���̰� ���� ������� �� �������� �Ǵ��ϰ� ����
            //// ������ ���⿡ ���� �ʹ������������� ���� �����ʵ����ϱ����� ª�� ray�߻� ��, �浹���������� ������� �̵�
            //// Ray�� ���� ���� �ʾ��� ��
            //if (!Physics.Raycast(_rbody.position, transform.forward, out hit, _capsuleColl.radius))
            //{
            //    // ������ �̵�
            //    _rbody.MovePosition(_rbody.position + dodgeDir * dodgeSpeed * Time.deltaTime);
            //}
            //// Ray�� ���� ����� ��
            //else
            //{
            //    // ���� ������ ���ϰ� �ش� ���� ������ ����� �� �ִ� �� �����̸� �ش� ���� �������� rbody MovePosition
            //    Physics.Raycast(_rbody.position, transform.forward, out hit, _capsuleColl.radius * 2.0f, 1 << LayerMask.NameToLayer("Ground"));

            //    wallAngle = Vector3.Angle(Vector3.up, hit.normal);
            //    if (wallAngle <= _maxSlope)
            //    {
            //        // �ش簢���� �������̵�
            //        wallClimbDir = Vector3.ProjectOnPlane(Vector3.up, hit.normal);
            //        wallClimbDir += transform.forward;
            //        _rbody.MovePosition(_rbody.position + wallClimbDir * dodgeSpeed * Time.deltaTime);
            //    }
            //}
            #endregion 23.08.13 �Ӹ� ��� �߳� 3���� ���̰� ���� ������� �� �������� �Ǵ��ϰ� ����
            yield return _wfs;
        }

        // �������߰�Ÿ �Է¿��ο� ����ó��
        if (isDodgeAttackInput)
        {
            _animator.SetBool(_hashRoll, false);
            _smoothDodgeBehaviour._isDodgeInput = false;
            _combat.ActDodgeAttack(); // ȸ���߰�Ÿ Ʈ���� �ߵ�
            _state.State = PlayerFSM.eState.Attack;
            _combat.RotateToClickDir();

            yield return new WaitUntil(() => _dodgeAttackEnd);
            _state.State = PlayerFSM.eState.Idle;
            _dodgeAttackEnd = false;
        }
        else
        {
            _state.State = PlayerFSM.eState.Idle;
            _animator.SetBool(_hashRoll, false);
            _smoothDodgeBehaviour._isDodgeInput = false;
        }
    }

    // �÷��̾ �̵���Ű���� �������� ĳ���͸� �������ϰ� ȸ��������
    void RotatePlayer()
    {
        if (_dir == Vector3.zero)
            return;

        Quaternion newRot = Quaternion.LookRotation(_dir);

        newRot = Quaternion.Slerp(_rbody.rotation, newRot, _rotSpeed * Time.deltaTime);
        newRot = Quaternion.Euler(transform.rotation.eulerAngles.x, newRot.eulerAngles.y, transform.rotation.eulerAngles.z);
        _rbody.rotation = newRot;
    }

    IEnumerator CoolDownDodge()
    {
        _dodgeCoolDown = 0.0f;
        while (_dodgeCoolDown <= _originDodgeCoolDown)
        {
            _dodgeCoolDown += Time.deltaTime;
            yield return null;
        }

        _dodgeCoolDown = _originDodgeCoolDown;
    }

    // ������ �߰�Ÿ �ִϸ��̼� �������� �޾Ƴ��� delegate
    public void OutDodgeAttack()
    {
        _dodgeAttackEnd = true;
        _combat._attackStyle = PlayerCombat.eAttackStyle.NonCombat;
    }

    /// <summary>
    /// �Ӹ�, ���, �߿��� �߻��ϴ� ���� ��ΰ� ���� �浹�ϰ��ִ���
    /// </summary>
    /// <returns></returns>
    //private bool IsDeadEndRoad()
    //{
    //    Ray ray;
    //    RaycastHit rayHit;
        
    //    // i��° ���̰� ���� �浹���� �Ǵ�, �ϳ��� ���� �浹�ϰ����������� return false
    //    for (int i = 0; i < _wallHitRays.Length; i++)
    //    {
    //        ray = new Ray(_wallHitRays[i].transform.position, _wallHitRays[i].transform.forward);

    //        if (Physics.Raycast(ray, out rayHit, _capsuleColl.radius * _wallRayDintance, 1 << LayerMask.NameToLayer("Ground")))
    //        {
    //            if (!rayHit.collider.tag.Equals("Wall"))
    //            {
    //                return false;
    //            }
    //            Debug.DrawRay(_wallHitRays[i].transform.position, _wallHitRays[i].transform.forward * _capsuleColl.radius * _wallRayDintance, Color.green);
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}
}
