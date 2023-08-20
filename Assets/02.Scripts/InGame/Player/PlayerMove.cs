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
    [SerializeField] private GameObject[] _raySet;
    public bool _isGrounded;
    public bool _isOnSlope;
    public GameObject _nextPosRay;
    public Vector3 _dir; // �÷��̾��� wasd�������� ���Ե� ���⺤�Ͱ��� ����
    private Vector3 _gravity; // rigidbody.velocity �� ���� �����ϱ⶧���� �߷¶��� ���� ����
    private Ray _groundRay;
    private RaycastHit _groundHit;

    [Header("=== Dodge ===")]
    public float _dodgeSpeed; // ������ ���ӽ�ų �� (dodgeSpeed = _dodgeSpeed * _movSpeed)
    public float _dodgeDur; // ��������°� ���ӵ� �ð�
    public float _currDodgeDur;
    public float _dodgeCoolDown;
    private float _originDodgeCoolDown;
    private bool _dodgeAttackEnd;
    public Vector3 _dodgeDir;

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
    readonly int _hashRoll = Animator.StringToHash("isRoll");
    readonly int _hashFall = Animator.StringToHash("isFall");

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
        _currDodgeDur = 0.0f;
        _wfs = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (_state.State == PlayerFSM.eState.Hit)
            return;
        if (_state.State == PlayerFSM.eState.Dead)
            return;

        if (_state.State == PlayerFSM.eState.Dodge)
        {
            if (_currDodgeDur > _dodgeDur)
            {
                _currDodgeDur = 0.0f;
                _state.State = PlayerFSM.eState.Idle;
                _animator.SetBool(_hashRoll, false);
                return;
            }

            _currDodgeDur += Time.deltaTime;
            _rbody.velocity = _dodgeDir * _movSpeed * _dodgeSpeed + _gravity;
        }

        // idle, fall, move���°� �ƴ϶�� �����̰� ������ �� ����
        if (_state.State != PlayerFSM.eState.Idle && _state.State != PlayerFSM.eState.Fall && _state.State != PlayerFSM.eState.Move)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        ManipulPhysics();
        _animator.SetBool(_hashFall, !_isGrounded);

        // h�� v�� �Է��� ��
        //if (_h != 0.0f || _v != 0.0f)
        //{
        //    _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        //    MovePlayer(_dir, _movSpeed);
        //    RotatePlayer();
        //}
        //// �ƴ� ��
        //else
        //{
        //    // ���̳� ������� ������, ��ǲ�� �ԷµǾ��ٰ� ��������!
        //    if (_isGrounded || _isOnSlope)
        //    {
        //        _dir = Vector3.zero;
        //        _rbody.velocity = Vector3.zero;
        //    }
        //}

        _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        MovePlayer(_dir, _movSpeed);
        RotatePlayer();

        // �ƴ� ��
        if (_h == 0.0f && _v == 0.0f)
        {
            // ���̳� ������� ������, ��ǲ�� �ԷµǾ��ٰ� ��������!
            if (_isGrounded || _isOnSlope)
            {
                _dir = Vector3.zero;
                _rbody.velocity = Vector3.zero;
            }
        }

        // h�� v�� �ϳ��� �Էµ����ʰ�, �������»��¿� ���ݻ��¿� ������°� �ƴ϶�� = idle����
        if ((_h == 0.0f && _v == 0.0f) &&
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
        {
            if (_dir.Equals(Vector3.zero))
            {
                _dodgeDir = transform.forward;
            }
            else
            {
                _dodgeDir = _dir;
            }

            transform.forward = _dodgeDir;
            _state.State = PlayerFSM.eState.Dodge;
            StartCoroutine(CoolDownDodge());
            _animator.SetBool(_hashRoll, true);
            //StartCoroutine(Dodge(_h, _v));
        }
    }

    void MovePlayer(Vector3 dir, float movSpeed)
    {
        // //�������� �ӷ��� ������ �����̸� fall���·� ��ȯ
        //if (_animator.GetBool(_hashFall))
        //    _state.State = PlayerFSM.eState.Fall;
        //else
        //    _state.State = PlayerFSM.eState.Move;

        //_animator.SetBool(_hashMove, _dir != Vector3.zero);
        //_dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        //_dir = _isOnSlope ? GetSlopeDir(_dir) : _dir;

        //_rbody.velocity = _dir * _movSpeed + _gravity;

        // �������� �ӷ��� ������ �����̸� fall���·� ��ȯ
        if (_animator.GetBool(_hashFall))
            _state.State = PlayerFSM.eState.Fall;
        else
            _state.State = PlayerFSM.eState.Move;

        _animator.SetBool(_hashMove, dir != Vector3.zero);
        dir = _isOnSlope ? GetSlopeDir(dir) : dir;
        _rbody.velocity = dir * movSpeed + _gravity;
    }

    private bool OnGround()
    {
        Vector3 boxSize = _groundCheckBox.transform.lossyScale;
        return Physics.CheckBox(_groundCheckBox.transform.position, boxSize, Quaternion.identity, 1 << LayerMask.NameToLayer("Ground"));
    }

    private bool OnSlope(Ray ray)
    {
        if (Physics.Raycast(ray, out _groundHit, 1 << LayerMask.NameToLayer("Ground")))
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
        // 1. ���� �ӵ��� �̵��ϸ� ��Ե� ���������ӿ� ���������� ���� ������ ���ϰ� �ö� �� ������ �� �ö󰡰�
        // �׸��� �ö󰥶� ��ĩ�ϴ� ���� �ذ��� �� ������
        // 2. �����̴��� ����

        _groundRay = new Ray(transform.position, Vector3.down);
        _isGrounded = OnGround();
        _isOnSlope = OnSlope(_groundRay);

        _gravity = _isGrounded ? Vector3.zero : Vector3.down * Mathf.Abs(_rbody.velocity.y);
        _rbody.useGravity = !_isOnSlope || !_isGrounded;
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
        bool defalutGravity = _rbody.useGravity;

        // �����⵿��
        StartCoroutine(CoolDownDodge());
        _rbody.useGravity = true;
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

            // �����ٰ� ����� ���� ������ ��, ������ üũ �� ����� �� �ִ� ���� �����̸� �����Ⱒ�� ����
            if (Physics.Raycast(_rbody.position, -transform.up, out hit, _capsuleColl.radius * _wallRayDintance,
                1 << LayerMask.NameToLayer("Ground")))
            {
                if (hit.transform.tag.Equals("Wall"))
                {
                    dodgeDir = Vector3.zero;
                }
                else
                {
                    wallAngle = Vector3.Angle(Vector3.up, hit.normal);
                    if (wallAngle <= _maxSlope)
                    {
                        // �ش簢���� �������̵�
                        wallClimbDir = Vector3.ProjectOnPlane(Vector3.up, hit.normal);
                        wallClimbDir += transform.forward;

                        dodgeDir = wallClimbDir;
                    }
                    else
                    {
                        dodgeDir = Vector3.zero;
                    }
                }
            }

            _rbody.MovePosition(_rbody.position + dodgeDir * dodgeSpeed * Time.deltaTime);
            //print(dodgeDir);
            //_rbody.velocity = dodgeDir * dodgeSpeed + _gravity;
            yield return _wfs;
        }
        _rbody.useGravity = defalutGravity;

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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Wall") && (_h != 0.0f || _v != 0.0f))
        {
            Vector3 wallNorm = collision.contacts[0].normal;
            Vector3 projection = Vector3.ProjectOnPlane(transform.forward - wallNorm, wallNorm); // ���� ������ �����̰� �� ����
            float power = _movSpeed * projection.magnitude * 2.0f;

            //Debug.DrawRay(collision.contacts[0].point, wallNorm, Color.green);
            //Debug.DrawRay(transform.position, transform.forward - wallNorm, Color.red);
            //Debug.DrawRay(collision.contacts[0].point, projection, Color.blue);
            _rbody.AddForce(projection.normalized * power, ForceMode.Impulse);
        }
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
