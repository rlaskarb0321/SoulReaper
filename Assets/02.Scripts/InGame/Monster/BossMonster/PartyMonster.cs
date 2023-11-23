using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMonster : MonsterBase_1, IDotDebuff
{
    [Header("------ Boss_Party Monster ------")]
    [Header("=== Mesh ===")]
    [SerializeField]
    private SkinnedMeshRenderer[] _meshRenderes;

    [SerializeField]
    private Material[] _originMats;

    [Header("=== Phase ===")]
    [SerializeField]
    private float[] _phaseCondition;

    [SerializeField]
    private ePhase _ePhase;

    [SerializeField]
    private int _burnStack;

    public enum ePhase { Phase_1, Phase_2, Phase_3, Count, }
    public ePhase Phase { get { return _ePhase; } set { _ePhase = value; } }

    // Field
    private readonly int _hashPhase = Animator.StringToHash("Phase Count");
    private PartyMonsterCombat _monsterCombat;
    private PartyBossPattern _pattern;
    private bool _needAiming;
    private Outline _outline;
    private Color _originOutlineColor;

    protected override void Awake()
    {
        base.Awake();

        _meshRenderes = GetComponentsInChildren<SkinnedMeshRenderer>();
        _monsterCombat = GetComponent<PartyMonsterCombat>();
        _pattern = GetComponent<PartyBossPattern>();
        _outline = GetComponent<Outline>();
    }

    protected override void Start()
    {
        base.Start();

        _originOutlineColor = _outline.OutlineColor;
    }

    public override IEnumerator OnHitEvent(eArrowState state = eArrowState.Normal)
    {
        Material[] originMats = new Material[_meshRenderes.Length];
        float duration = 5.0f + (3.0f * (int)state);

        for (int i = 0; i < _meshRenderes.Length; i++)
        {
            originMats[i] = _originMats[0];
            _meshRenderes[i].material = _hitMats[(int)state];
        }
        yield return new WaitForSeconds(Time.deltaTime * duration);

        for (int i = 0; i < _meshRenderes.Length; i++)
        {
            _meshRenderes[i].material = originMats[i];
        }
    }

    public override void DecreaseHP(float amount, BurnDotDamage burn = null)
    {
        if (_pattern._isSummonStart)
            _pattern.HitDuringSummon(_burnStack > 0, amount + (35.0f * _burnStack));
        if (burn != null)
        {
            StartCoroutine(DotDamaged(burn));
            ControlDebuffStack(1);
        }
        if (_currHp <= 0.0f)
            return;

        _currHp -= amount;
        ChangeOutLine();
        ChangePhase();
        if (_currHp <= 0.0f)
        {
            _currHp = 0.0f;
            Dead();
        }
    }

    public override void SwitchNeedAiming(int value) => _needAiming = value == 1 ? true : false;

    public override void AimingTarget(Vector3 target, float rotMulti)
    {
        if (_needAiming)
        {
            if (_nav != null)
            {
                _nav.updatePosition = false;
            }

            Vector3 dir = target - transform.position;
            dir = Vector3.ProjectOnPlane(dir, Vector3.up);
            transform.rotation =
                Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _stat.rotSpeed * rotMulti * Time.deltaTime);
        }
        else
        {
            if (_nav != null)
            {
                _nav.updatePosition = true;
            }
        }
    }

    private void ChangeOutLine()
    {
        _outline.OutlineColor = Color.Lerp(_originOutlineColor, Color.red, 1 - (_currHp / _stat.health));
    }

    private void ChangePhase()
    {
        if ((_currHp / _stat.health) <= _phaseCondition[(int)ePhase.Phase_2])
        {
            if (_ePhase == ePhase.Phase_3)
                return;

            _ePhase = ePhase.Phase_3;
            _animator.SetInteger(_hashPhase, (int)_ePhase + 1);
            _monsterCombat.CheckSkill();
        }
        else if ((_currHp / _stat.health) <= _phaseCondition[(int)ePhase.Phase_1])
        {
            if (_ePhase == ePhase.Phase_2)
                return;

            _ePhase = ePhase.Phase_2;
            _animator.SetInteger(_hashPhase, (int)_ePhase + 1);
            _monsterCombat.CheckSkill();
        }
        else
        {
            if (_ePhase == ePhase.Phase_1)
                return;

            _ePhase = ePhase.Phase_1;
            _animator.SetInteger(_hashPhase, (int)_ePhase + 1);
        }
    }

    public IEnumerator DotDamaged(BurnDotDamage dotDamage)
    {
        float duration = dotDamage._debuffDur;
        WaitForSeconds ws = new WaitForSeconds(dotDamage._dotInterval);

        while (duration > 0.0f)
        {
            yield return ws;

            DecreaseHP(dotDamage._dotDamamge + (0.5f * _burnStack));
            StartCoroutine(OnHitEvent(eArrowState.Fire));
            duration -= dotDamage._dotInterval;
        }
        ControlDebuffStack(-1);
    }

    public void ControlDebuffStack(int count)
    {
        _burnStack += count;
    }
}
