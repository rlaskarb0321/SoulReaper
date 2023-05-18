using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCombat : MonoBehaviour
{
    [Header("Follow Cam")]
    public GameObject _followCamObj;

    [Header("Attack")]
    public float _attackAdvancedDist; // 공격시 전진거리
    public float _needChargingTime; // 원거리 공격 사용하는데 필요한 차징 시간
    [HideInInspector] public float _curLongRangeChargingTime; // 현재 원거리 공격 충전시간
    public float _fallAttackSpeed; // 낙하공격시 떨어지는 속도

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
    Camera _cam;
    Animator _animator;
    Rigidbody _rbody;

    // Weapon
    BoxCollider _weaponColl;
    TrailRenderer _weaponTrail;
    MeleeWeaponMgr _weapon;

    int _combo;
    bool _unFreeze;

    [Header("Component")]
    AttackComboBehaviour _atkBehaviour;
    SmoothDodgeBehaviour _smoothDodgeBehaviour;
    PlayerMove _mov;
    PlayerFSM _state;
    FollowCamera _followCam;

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
        _mov = GetComponent<PlayerMove>();
        _smoothDodgeBehaviour = _animator.GetBehaviour<SmoothDodgeBehaviour>();

        // Weapon
        _weaponColl = _meleeWeaponObj.GetComponentInChildren<BoxCollider>();
        _weaponTrail = _meleeWeaponObj.GetComponentInChildren<TrailRenderer>();
        _weapon = _meleeWeaponObj.GetComponentInChildren<MeleeWeaponMgr>();
    }

    void Start()
    {
        _cam = Camera.main;
        _player = this.transform;
        _combo = 0;
        _attackStyle = eAttackStyle.NonCombat;

        InitChargingGauge();
    }

    void Update()
    {
        if (_state.State == PlayerFSM.eState.Hit)
            return;
        if (_state.State == PlayerFSM.eState.Dead)
            return;

        // 근or원거리공격으로 모션전환관련
        if (Input.GetMouseButtonDown(0) && 
            (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move))
        {
            // 근거리 공격
            _state.State = PlayerFSM.eState.Attack;
            RotateToClickDir();
            _animator.SetInteger(_hashCombo, ++_combo);

            // 근거리 일반공격 관련
            _attackStyle = eAttackStyle.Normal;
        }

        // 차징모션으로 전환 관련
        else if (Input.GetMouseButton(1) && 
            (_state.State == PlayerFSM.eState.Idle || _state.State == PlayerFSM.eState.Move
            || _state.State == PlayerFSM.eState.Charging))
        {
            // 원거리공격
            if (_curLongRangeChargingTime < _needChargingTime)
            {
                _curLongRangeChargingTime += Time.deltaTime;
                _animator.SetFloat(_hashChargingValue, _curLongRangeChargingTime);
            }

            _state.State = PlayerFSM.eState.Charging;
            RotateToClickDir();
            _followCam.CamState = FollowCamera.eCameraState.Charging;
        }

        // 차징타임에따라 원거리공격을 하거나 취소를 결정
        else if (Input.GetMouseButtonUp(1))
        {
            // 원거리공격 발사or취소
            if (_curLongRangeChargingTime > _needChargingTime)
            {
                _animator.SetTrigger(_hashChargingBurst);
            }

            InitChargingGauge();
            _followCam.CamState = FollowCamera.eCameraState.Follow;
            _state.State = PlayerFSM.eState.Idle;
        }

        #region 04.26 공격에 적중한 적과 관련된 요소들에대한 작업
        //if (_hitEnemiesList.Count > 0)
        //{
        //    _hitEnemiesList = _hitEnemiesList.Distinct().ToList();

        //    for (int i = _hitEnemiesList.Count - 1; i >= 0; i--)
        //    {
        //        if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
        //        {
        //            Debug.Log(_hitEnemiesList[i].name + "체력 깎");
        //            _hitEnemiesList.Remove(_hitEnemiesList[i]);

        //            //Monster monster = _hitEnemiesList[i].GetComponent<Monster>();
        //            //monster.DecreaseHp(CalcDamage());
        //            //_hitEnemiesList.Remove(monster.gameObject);
        //        }
        //        else if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
        //        {
        //            Debug.Log(_hitEnemiesList[i].name + "반사");
        //            _hitEnemiesList.Remove(_hitEnemiesList[i]);
        //        }
        //    }
        //    _hitEnemiesList.Clear();
        //}
        #endregion
    }

    /// <summary>
    /// 클릭한 마우스 방향으로 캐릭터를 회전시킴
    /// </summary>
    public void RotateToClickDir()
    {
        RaycastHit hit;
        Vector3 clickVector;
        Vector3 dir;

        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            clickVector = new Vector3(hit.point.x, _player.position.y, hit.point.z);
            dir = clickVector - _player.position;
            transform.forward = dir;
        }
    }

    public void InitChargingGauge()
    {
        _curLongRangeChargingTime = 0.0f;
        _animator.SetFloat(_hashChargingValue, _curLongRangeChargingTime);
    }

    // Attack 애니메이션 마지막에 달아놓는 Delegate, 콤보를 더 이어나갈지 공격을 끝낼지 결정한다.
    public void SetComboInteger()
    {
        if (_atkBehaviour._isComboAtk)
        {
            _combo++;
            if (_combo > 2)
                _combo = 0;

            RotateToClickDir();
            _animator.SetInteger(_hashCombo, _combo);
            _atkBehaviour._isComboAtk = false;
            return;
        }

        EndComboAtk();
    }

    // 콤보공격단계중 마지막콤보의 마지막 프레임에 달아놓는 Delegate, 어택콤보를 초기화시킨다.
    public void EndComboAtk()
    {
        _combo = 0;

        if (_state.State == PlayerFSM.eState.Hit || _state.State == PlayerFSM.eState.Dead)
        {
            _animator.SetInteger(_hashCombo, _combo);
            return;
        }

        // 공격1, 2, 차징발사 애니메이션 실행도중에 space가 입력되면 마지막프레임에서 회피로 이동
        if (_smoothDodgeBehaviour._isDodgeInput)
        {
            StartCoroutine(_mov.Dodge(_mov._h, _mov._v));
            _animator.SetInteger(_hashCombo, _combo);
            return;
        }

        _state.State = PlayerFSM.eState.Idle;
        _animator.SetInteger(_hashCombo, _combo);
        _attackStyle = eAttackStyle.NonCombat;
    }

    public IEnumerator ActFallAttack(Rigidbody rbody, Animator animator)
    {
        RaycastHit hit;
        Vector3 landingPoint;
        float originAnimSpeed;

        animator.SetBool(_hashFallAttack, true);
        _attackStyle = eAttackStyle.FallAttack;
        rbody.isKinematic = true;
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
                    GenShockWave();
                    // StartCoroutine(_followCam.ShakingCamera(_fallAttackCamShakeDur, _fallAttackCamShakeAmount));
                    break;
                }

                _state.State = PlayerFSM.eState.Attack; // 공격상태로 전환
                _rbody.MovePosition(_rbody.position + Vector3.down * _fallAttackSpeed * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    #region 애니메이션 Delegate용 함수들

    // 낙하공격관련 애니메이션 델리게이트
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

    // 공격애니메이션의 시작과 끝에 달아서 collider를 키고끄는용
    public void SetActiveWeaponColl()
    {
        // 현재 boxcollider컴포넌트의 활성화값, TrailRenderer의 emitting값을 저장하고 반전시킨값을 대입시킴
        bool collEnable = _weaponColl.enabled;
        bool trailEnable = _weaponTrail.emitting;

        _weaponTrail.emitting = !trailEnable;
        _weaponColl.enabled = !collEnable;
    }

    // 애니메이션 delegate로 원거리공격
    public void LaunchProjectile()
    {
        Instantiate(_longRangeProjectile, _firePos.position, transform.rotation);
    }
    #endregion 애니메이션 Delegate용 함수들

    private void GenShockWave()
    {
        print("쾅");
    }

    public void ActDodgeAttack()
    {
        _animator.SetTrigger(_hashDodgeAttack);
        _attackStyle = eAttackStyle.DodgeAttack;
    }

    // 낙하, 대쉬공격에 따른 데미지배율계산
    public float CalcDamage()
    {
        float damage = 0.0f;
        switch (_attackStyle)
        {
            case eAttackStyle.Normal:
                damage = _weapon._atkPower * 1.0f;
                break;
            case eAttackStyle.DodgeAttack:
                damage = _weapon._atkPower * 3.0f;
                break;
            case eAttackStyle.FallAttack:
                damage = _weapon._atkPower * 4.0f;
                break;
        }

        return damage;
    }
}   
