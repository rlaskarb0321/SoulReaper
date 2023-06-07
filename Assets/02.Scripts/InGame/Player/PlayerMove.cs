using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Move & Rotate")]
    public float _movSpeed;
    public float _rotSpeed;
    [Range(0.0f, 90.0f)] public float _maxSlope;
    [HideInInspector] public float _v;
    [HideInInspector] public float _h;

    [Header("Dodge")]
    public float _dodgeSpeed; // ������ ���ӽ�ų �� (dodgeSpeed = _dodgeSpeed * _movSpeed)
    public float _dodgeDur; // ��������°� ���ӵ� �ð�
    public float _dodgeCoolDown;
    
    [Header("Follow Cam")]
    public GameObject _followCamObj;

    Vector3 _dir; // �÷��̾��� wasd�������� ���Ե� ���⺤�Ͱ��� ����
    Animator _animator;
    Rigidbody _rbody;
    bool _dodgeAttackEnd;
    float _originDodgeCoolDown;
    WaitForFixedUpdate _wfs;

    [Header("Component")]
    PlayerCombat _combat;
    PlayerFSM _state;
    FollowCamera _followCam;
    FallBehaviour _fallBehaviour;
    CapsuleCollider _capsuleColl;
    SmoothDodgeBehaviour _smoothDodgeBehaviour;

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

        // �ֱ������� �߷°�������� �������� ����� ������ΰ���
        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);

        // idle, fall, move���°� �ƴ϶�� �����̰� ������ �� ����
        if (_state.State != PlayerFSM.eState.Idle && _state.State != PlayerFSM.eState.Fall && _state.State != PlayerFSM.eState.Move)
        {
            _animator.SetBool(_hashMove, false);
            return;
        }

        // h�� v�� ��� �ϳ��� �Է��̵ȴٸ�
        if ((_h != 0.0f || _v != 0.0f))
        {
            MovePlayer(); 
            RotatePlayer();
        }
        // h�� v�� �ϳ��� �Էµ����ʰ�, �������»��¿� ���ݻ��¿� ������°� �ƴ϶�� = idle����
        else if (!_fallBehaviour._isFall && _state.State != PlayerFSM.eState.Attack && _state.State != PlayerFSM.eState.Dead)
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

        if (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move || _state.State == PlayerFSM.eState.Fall)
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

    // �÷��̾��� ���¸� idle�� �ٲٰ� �����̴¸�����, ���� ������ ����
    void MovePlayer()
    {
        // �������� �ӷ��� ������ �����̸� fall���·� ��ȯ
        if (_animator.GetFloat(_hashYVelocity) <= -0.4f)
            _state.State = PlayerFSM.eState.Fall;
        else
            _state.State = PlayerFSM.eState.Move;

        RaycastHit groundHit;
        Ray ray = new Ray(transform.position, -transform.up);

        #region Ray �߰��� �����ڵ� ��Ȱ��ȭ
        // ���������������ƴ��� ���η� move�ִϸ��̼� ����
        // bool isFall = _state.State == PlayerFSM.eState.Fall ? true : false;
        //_animator.SetBool(_hashMove, !isFall);  
        # endregion Ray �߰��� �����ڵ� ��Ȱ��ȭ

        if (Physics.Raycast(ray, out groundHit, 1.35f, 1 << LayerMask.NameToLayer("Ground")))
        {
            _animator.SetBool(_hashMove, true);
            _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        
            if (Vector3.Angle(transform.up, groundHit.normal) != 0)
            {
                Vector3 slopeMoveDir = Vector3.ProjectOnPlane(_dir, groundHit.normal);
                _dir = slopeMoveDir.normalized;
            }
        }
        else
        {
            _animator.SetBool(_hashMove, false);
            _dir = ((_h * Vector3.right) + (_v * Vector3.forward)).normalized;
        }

        _rbody.MovePosition(_rbody.position + _dir * _movSpeed * Time.deltaTime);
    }

    public IEnumerator Dodge(float h, float v)
    {
        if (_combat._curLongRangeChargingTime >= 0.0f)
            _combat.InitChargingGauge();
        if (_followCam.CamState != FollowCamera.eCameraState.Follow)
        {
            _followCam.CamState = FollowCamera.eCameraState.Follow;
            Input.ResetInputAxes();
        }

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

            // ������ ���⿡ ���� �ʹ������������� ���� �����ʵ����ϱ����� ª�� ray�߻� ��, �浹���������� ������� �̵�
            currDur += Time.deltaTime;
            if (!Physics.Raycast(_rbody.position, transform.forward, out hit, _capsuleColl.radius))
            {
                // �÷��̾� ĸ��Coll�� ������������ Ray�� ���� �浹���� �ʾ������� ������ �̵�
                _rbody.MovePosition(_rbody.position + dodgeDir * dodgeSpeed * Time.deltaTime);
            }
            else
            {
                // �÷��̾� ĸ��Coll ������������ Ray�� ���� �浹�� ��쿡��
                // ���� ������ ���ϰ� �ش� ���� ������ ����� �� �ִ� �� �����̸� �ش� ���� �������� rbody MovePosition

                Physics.Raycast(_rbody.position, transform.forward, out hit, _capsuleColl.radius, 1 << LayerMask.NameToLayer("Ground"));
                wallAngle = Vector3.Angle(Vector3.up, hit.normal);
                // print(wallAngle);
                if (wallAngle <= _maxSlope)
                {
                    // �ش簢���� �������̵�
                    wallClimbDir = Vector3.ProjectOnPlane(Vector3.up, hit.normal);
                    wallClimbDir += transform.forward;
                    _rbody.MovePosition(_rbody.position + wallClimbDir * dodgeSpeed * Time.deltaTime);
                }
            }
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
}
