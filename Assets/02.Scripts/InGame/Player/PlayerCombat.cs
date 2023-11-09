using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCombat : MonoBehaviour
{
    [Header("Follow Cam")]
    public GameObject _followCamObj;

    [Header("Attack")]
    public float _needChargingTime; // ���Ÿ� ���� ����ϴµ� �ʿ��� ��¡ �ð�
    [HideInInspector] public float _curLongRangeChargingTime; // ���� ���Ÿ� ���� �����ð�
    public float _fallAttackSpeed; // ���ϰ��ݽ� �������� �ӵ�

    [Header("Cam Shake")]
    public float _hitCamShakeAmount;
    public float _hitCamShakeDur;
    public float _fallAttackCamShakeAmount;
    public float _fallAttackCamShakeDur;

    [Header("Weapon")]
    public GameObject _meleeWeaponObj;
    public GameObject _longRangeProjectile;
    public Transform _firePos;
    public enum eAttackStyle { NonCombat, Normal, DodgeAttack, FallAttack }
    public eAttackStyle _attackStyle;

    [Header("Field")]
    Transform _player;
    Animator _animator;
    Rigidbody _rbody;
    private IOnOffSwitchSkill _onOffSkill;

    // Weapon
    private BoxCollider _weaponColl;
    private TrailRenderer _weaponTrail;
    private MeleeWeaponMgr _weapon;

    int _combo;
    bool _unFreeze;

    [Header("Component")]
    AttackComboBehaviour _atkBehaviour;
    SmoothDodgeBehaviour _smoothDodgeBehaviour;
    PlayerMove_1 _mov;
    PlayerFSM _state;
    FollowCamera _followCam;
    SoundEffects _sfx;
    PlayerData _stat;

    readonly int _hashCombo = Animator.StringToHash("AttackCombo");
    readonly int _hashChargingValue = Animator.StringToHash("ChargingValue");
    readonly int _hashChargingBurst = Animator.StringToHash("ChargingBurst");
    readonly int _hashFallAttack = Animator.StringToHash("FallAttack");
    readonly int _hashDodgeAttack = Animator.StringToHash("DodgeAttack");

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _state = GetComponent<PlayerFSM>();
        _atkBehaviour = _animator.GetBehaviour<AttackComboBehaviour>();
        _followCam = _followCamObj.GetComponent<FollowCamera>();
        _rbody = GetComponent<Rigidbody>();
        _mov = GetComponent<PlayerMove_1>();
        _smoothDodgeBehaviour = _animator.GetBehaviour<SmoothDodgeBehaviour>();
        _sfx = GetComponent<SoundEffects>();
        _stat = GetComponent<PlayerData>();

        // Weapon
        _weaponColl = _meleeWeaponObj.GetComponentInChildren<BoxCollider>();
        _weaponTrail = _meleeWeaponObj.GetComponentInChildren<TrailRenderer>();
        _weapon = _meleeWeaponObj.GetComponentInChildren<MeleeWeaponMgr>();
    }

    void Start()
    {
        _player = this.transform;
        _combo = 0;
        _attackStyle = eAttackStyle.NonCombat;

        InitChargingGauge();
    }

    void Update()
    {
        if (ProductionMgr._isPlayingProduction)
            return;

        if (_state.State == PlayerFSM.eState.Hit || _state.State == PlayerFSM.eState.Dead)
            return;
        if (_state.State == PlayerFSM.eState.Ladder || _state.State == PlayerFSM.eState.LadderOut)
            return;

        // ��or���Ÿ��������� �����ȯ����
        if (Input.GetMouseButtonDown(0) && 
            (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move))
        {
            // �ٰŸ� ����
            _state.State = PlayerFSM.eState.Attack;
            RotateToClickDir();
            _weapon._sfx.PlayOneShotUsingDict("Slash Air");
            _animator.SetInteger(_hashCombo, ++_combo);

            // �ٰŸ� �Ϲݰ��� ����
            _attackStyle = eAttackStyle.Normal;
        }

        // ��¡������� ��ȯ ����
        else if (Input.GetMouseButton(1) &&
            (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move
            || _state.State == PlayerFSM.eState.Charging))
        {
            if (_stat._currMP - 10.0f < 0.0f)
            {
                print("������ ����");
                return;
            }

            // ���Ÿ�����
            if (_curLongRangeChargingTime < _needChargingTime)
            {
                _curLongRangeChargingTime += Time.deltaTime;
                _animator.SetFloat(_hashChargingValue, _curLongRangeChargingTime);
            }

            _state.State = PlayerFSM.eState.Charging;
            RotateToClickDir();
            _followCam.CamState = FollowCamera.eCameraState.Charging;
        }

        // ��¡Ÿ�ӿ����� ���Ÿ������� �ϰų� ��Ҹ� ����
        else if (Input.GetMouseButtonUp(1))
        {
            
            // ���Ÿ����� �߻�or���
            if (_curLongRangeChargingTime > _needChargingTime)
            {
                _animator.SetTrigger(_hashChargingBurst);
            }

            InitChargingGauge();
            _followCam.CamState = FollowCamera.eCameraState.Follow;
            _state.State = PlayerFSM.eState.Idle;
        }

        #region 04.26 ���ݿ� ������ ���� ���õ� ��ҵ鿡���� �۾�
        //if (_hitEnemiesList.Count > 0)
        //{
        //    _hitEnemiesList = _hitEnemiesList.Distinct().ToList();

        //    for (int i = _hitEnemiesList.Count - 1; i >= 0; i--)
        //    {
        //        if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
        //        {
        //            Debug.Log(_hitEnemiesList[i].name + "ü�� ��");
        //            _hitEnemiesList.Remove(_hitEnemiesList[i]);

        //            //Monster monster = _hitEnemiesList[i].GetComponent<Monster>();
        //            //monster.DecreaseHp(CalcDamage());
        //            //_hitEnemiesList.Remove(monster.gameObject);
        //        }
        //        else if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
        //        {
        //            Debug.Log(_hitEnemiesList[i].name + "�ݻ�");
        //            _hitEnemiesList.Remove(_hitEnemiesList[i]);
        //        }
        //    }
        //    _hitEnemiesList.Clear();
        //}
        #endregion
    }

    /// <summary>
    /// Ŭ���� ���콺 �������� ĳ���͸� ȸ����Ŵ
    /// </summary>
    public void RotateToClickDir()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2); // ��ũ�� �߾� ��ǥ
        Vector2 baseVector = new Vector2(Screen.width / 2, Screen.height) - screenCenter; // ��ũ�� �߾� ~ 12�� ��ũ�� �������� ����
        Vector2 inputVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - screenCenter; // ��ũ�� �߾� ~ ���콺 �Է¹��� ����

        float angle = Vector2.SignedAngle(baseVector, inputVector);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, -angle, transform.eulerAngles.z);
    }

    public void InitChargingGauge()
    {
        _curLongRangeChargingTime = 0.0f;
        _animator.SetFloat(_hashChargingValue, _curLongRangeChargingTime);
    }

    public IEnumerator ActFallAttack(Rigidbody rbody, Animator animator)
    {
        RaycastHit hit;
        Vector3 landingPoint;
        float originAnimSpeed;

        rbody.isKinematic = true;
        animator.SetBool(_hashFallAttack, true);
        _attackStyle = eAttackStyle.FallAttack;
        originAnimSpeed = animator.speed;
        yield return new WaitUntil(() => _unFreeze);

        animator.speed = 0.0f;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            landingPoint = hit.point;
            rbody.isKinematic = false;

            while (true)
            {
                if (Vector3.Distance(transform.position, landingPoint) <= 0.5f)
                {
                    animator.speed = originAnimSpeed;
                    GenShockWave(_attackStyle);
                    // StartCoroutine(_followCam.ShakingCamera(_fallAttackCamShakeDur, _fallAttackCamShakeAmount));
                    break;
                }

                _state.State = PlayerFSM.eState.Attack; // ���ݻ��·� ��ȯ
                _rbody.MovePosition(_rbody.position + Vector3.down * _fallAttackSpeed * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    #region �ִϸ��̼� Delegate�� �Լ���
    // Attack �ִϸ��̼� �������� �޾Ƴ��� Delegate, �޺��� �� �̾���� ������ ������ �����Ѵ�.
    public void SetComboInteger()
    {
        if (_atkBehaviour._isComboAtk)
        {
            _combo++;
            if (_combo > 2)
                _combo = 0;

            RotateToClickDir();
            _animator.SetInteger(_hashCombo, _combo);
            _weapon._sfx.PlayOneShotUsingDict("Slash Air");
            _atkBehaviour._isComboAtk = false;
            return;
        }

        EndComboAtk();
    }

    // �޺����ݴܰ��� �������޺��� ������ �����ӿ� �޾Ƴ��� Delegate, �����޺��� �ʱ�ȭ��Ų��.
    public void EndComboAtk()
    {
        _combo = 0;
        _atkBehaviour._isComboAtk = false;

        if (_state.State == PlayerFSM.eState.Hit || _state.State == PlayerFSM.eState.Dead)
        {
            _animator.SetInteger(_hashCombo, _combo);
            return;
        }

        //// ����1, 2, ��¡�߻� �ִϸ��̼� ���൵�߿� space�� �ԷµǸ� �ش� �ִϸ��̼��� �����������ӿ��� ȸ�Ƿ� �̵�
        //if (_smoothDodgeBehaviour._isDodgeInput)
        //{
        //    _animator.SetInteger(_hashCombo, _combo);
        //    //StartCoroutine(_mov.Dodge(_smoothDodgeBehaviour._h, _smoothDodgeBehaviour._v));
        //    return;
        //}

        _state.State = PlayerFSM.eState.Idle;
        _animator.SetInteger(_hashCombo, _combo);
        _attackStyle = eAttackStyle.NonCombat;
    }

    // ���ϰ��ݰ��� �ִϸ��̼� ��������Ʈ
    public void UnFreeze()
    {
        if (_unFreeze)
        {
            _animator.SetBool(_hashFallAttack, false);
            _state.State = PlayerFSM.eState.Idle;
            _unFreeze = false;
            _attackStyle = eAttackStyle.NonCombat;
            return;
        }
        _unFreeze = true;
    }

    // ���ݾִϸ��̼��� ���۰� ���� �޾Ƽ� collider�� Ű����¿�
    public void SetActiveWeaponColl()
    {
        // ���� boxcollider������Ʈ�� Ȱ��ȭ��, TrailRenderer�� emitting���� �����ϰ� ������Ų���� ���Խ�Ŵ
        bool isCollEnable = _weaponColl.enabled;
        bool isTrailEnable = _weaponTrail.emitting;

        _weaponTrail.emitting = !isTrailEnable;
        _weaponColl.enabled = !isCollEnable;

        if (!_weaponColl.enabled)
        {
            _weapon.DecreaseHitMonster();
        }
    }

    // �ִϸ��̼� delegate�� ���Ÿ�����
    public void LaunchProjectile()
    {
        _stat.DecreaseMP(10.0f);
        GameObject arrowObj = Instantiate(_longRangeProjectile, _firePos.position, transform.rotation) as GameObject;
        LaunchProjectile arrow = arrowObj.GetComponent<LaunchProjectile>();

        if (_onOffSkill != null)
        {
            _onOffSkill.UseOnOffSkill();
            arrow.UpgradeFire();
        }

        arrow.SetArrwInform(_stat._basicAtkDamage + 10);
    }

    #endregion �ִϸ��̼� Delegate�� �Լ���

    private void GenShockWave(eAttackStyle attackStyle)
    {
        RaycastHit hit;
        switch (attackStyle)
        {
            case eAttackStyle.FallAttack:
                Ray ray = new Ray(transform.position, -transform.up);
                if (Physics.Raycast(ray, out hit, 2.0f, 1 << LayerMask.NameToLayer("Ground")))
                {
                    Instantiate(_weapon._shockWave, new Vector3(transform.position.x, hit.transform.position.y, transform.position.z), transform.rotation);
                }
                break;
        }
    }

    public void ActDodgeAttack()
    {
        _animator.SetTrigger(_hashDodgeAttack);
        // _weapon._sfx.PlaySFXs("Slash Air");
        StartCoroutine(_weapon._sfx.PlaySFXsDelay("Slash Air", 0.2f));
        _attackStyle = eAttackStyle.DodgeAttack;
    }

    // ����, �뽬���ݿ� ���� �������������
    public float CalcDamage()
    {
        float damage = _stat._basicAtkDamage;
        switch (_attackStyle)
        {
            case eAttackStyle.Normal:
                damage += _weapon._atkPower * 1.0f;
                break;
            case eAttackStyle.DodgeAttack:
                damage += _weapon._atkPower * 3.0f;
                break;
            case eAttackStyle.FallAttack:
                damage += _weapon._atkPower * 4.0f;
                break;
        }

        return damage;
    }

    public void ActiveFireArrow(FireArrowSkill.eSkillActiveState activeState, IOnOffSwitchSkill skill)
    {
        bool isActive = (activeState == FireArrowSkill.eSkillActiveState.Active);
        if (isActive)
        {
            _onOffSkill = skill;
        }
        else
        {
            _onOffSkill = null;
        }
    }
}   
