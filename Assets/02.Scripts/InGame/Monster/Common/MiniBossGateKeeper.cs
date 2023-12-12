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

    [SerializeField]
    private GameObject[] _battleICons;

    private enum eBattleICon { Castle, Battle_1, Battle_2, Count }
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
            for (int i = (int)eBattleICon.Castle; i < (int)eBattleICon.Count; i++)
            {
                _battleICons[i].SetActive(false);
            }

            _castleGate.enabled = true;
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

        for (int i = (int)eBattleICon.Battle_1; i < (int)eBattleICon.Count; i++)
        {
            _battleICons[i].SetActive(true);
        }

        _interactable.SetActiveInteractUI(false);
        _monsterBase._target = SearchTarget(150.0f);
        _bullInteract.SetActive(false);
        _isAttacked = true;
        _monsterBase._state = MonsterBase_1.eMonsterState.Trace;
    }

    public void OpenDoor()
    {
        _castleGate.enabled = true;
        _battleICons[(int)eBattleICon.Castle].SetActive(false);

        _interactable.SetActiveInteractUI(false);
        _bullInteract.SetActive(false);
    }
}
