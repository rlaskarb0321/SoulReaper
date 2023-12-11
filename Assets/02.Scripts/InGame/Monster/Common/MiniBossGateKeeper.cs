using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossGateKeeper : MonsterType
{
    [Header("=== ¹®Áö±â ===")]
    [SerializeField]
    private BoxCollider _castleGate;

    [SerializeField]
    private GameObject _bullInteract;

    private MonsterBase_1 _monsterBase;
    private IInteractable _interactable;
    private Bull_1 _bull;
    private AudioSource _audio;
    private bool _isAttacked;
    private readonly int _hashRun = Animator.StringToHash("Run");

    private void Awake()
    {
        _monsterBase = GetComponent<MonsterBase_1>();
        _interactable = GetComponentInChildren<IInteractable>();
        _audio = GetComponent<AudioSource>();
        _bull = GetComponent<Bull_1>();
        _monsterBase._stat.traceDist = 150.0f;
    }

    private void Start()
    {
        if (!_audio.isPlaying && _audio.enabled)
        {
            _audio.clip = _bull._bossSound[(int)Bull_1.eBossSound.Idle];
            _audio.Play();
        }
    }

    private void Update()
    {
        if (!_isAttacked)
            return;

        if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead && !_castleGate.enabled)
        {
            _castleGate.enabled = true;
            _interactable.SetActiveInteractUI(false);
            return;
        }

        switch (_monsterBase._state)
        {
            case MonsterBase_1.eMonsterState.Idle:
                if (!_audio.isPlaying && _audio.enabled)
                {
                    _audio.clip = _bull._bossSound[(int)Bull_1.eBossSound.Idle];
                    _audio.Play();
                }
                break;

            case MonsterBase_1.eMonsterState.Trace:
                Trace();
                break;
        }
    }

    public override GameObject SearchTarget(float searchRange)
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, searchRange, 1 << LayerMask.NameToLayer("PlayerTeam"));
        return colls[0].gameObject;
    }

    public override void Trace()
    {
        if (!_monsterBase._nav.enabled)
            return;

        float distance = Vector3.Distance(transform.position, _monsterBase._target.transform.position);

        _monsterBase._animator.SetBool(_hashRun, true);
        if (distance <= _monsterBase._stat.attakDist)
        {
            _monsterBase._state = MonsterBase_1.eMonsterState.Attack;
            _monsterBase._animator.SetBool(_hashRun, false);
        }
        else
        {
            _monsterBase.Move(_monsterBase._target.transform.position, _monsterBase._stat.movSpeed);
        }
    }

    public override void ReactDamaged()
    {
        if (_monsterBase._state != MonsterBase_1.eMonsterState.Idle)
            return;

        _monsterBase._target = SearchTarget(150.0f);
        _bullInteract.SetActive(false);
        _isAttacked = true;
        _monsterBase._state = MonsterBase_1.eMonsterState.Trace;
    }
}
