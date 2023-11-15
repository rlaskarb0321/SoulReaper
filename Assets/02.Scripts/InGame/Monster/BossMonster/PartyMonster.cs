using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMonster : MonsterBase_1
{
    [Header("------ Boss_Party Monster ------")]
    [Header("=== Mesh ===")]
    [SerializeField]
    private SkinnedMeshRenderer[] _meshRenderes;

    [Header("=== Phase ===")]
    [SerializeField]
    private float[] _phaseCondition;

    [SerializeField]
    private ePhase _ePhase;

    public enum ePhase { Phase_1, Phase_2, Phase_3, Count, }
    public ePhase Phase { get { return _ePhase; } set { _ePhase = value; } }

    // Field
    private PartyMonsterCombat _monsterCombat;
    private bool _needAiming;

    protected override void Awake()
    {
        _meshRenderes = GetComponentsInChildren<SkinnedMeshRenderer>();
        _monsterCombat = GetComponent<PartyMonsterCombat>();
    }

    public override IEnumerator OnHitEvent()
    {
        Material[] newMats = new Material[_meshRenderes.Length];

        for (int i = 0; i < _meshRenderes.Length; i++)
        {
            newMats[i] = _meshRenderes[i].material;
            _meshRenderes[i].material = _hitMats[1];
        }

        yield return new WaitForSeconds(Time.deltaTime * 6.0f);

        for (int i = 0; i < _meshRenderes.Length; i++)
            _meshRenderes[i].material = newMats[i];
    }

    public override void DecreaseHP(float amount)
    {
        base.DecreaseHP(amount);
        ChangePhase();
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

    private void ChangePhase()
    {
        if ((_currHp / _stat.health) <= _phaseCondition[(int)ePhase.Phase_2])
        {
            if (_ePhase == ePhase.Phase_3)
                return;

            _ePhase = ePhase.Phase_3;
            _monsterCombat.CheckSkill();
        }
        else if ((_currHp / _stat.health) <= _phaseCondition[(int)ePhase.Phase_1])
        {
            if (_ePhase == ePhase.Phase_2)
                return;

            _ePhase = ePhase.Phase_2;
            _monsterCombat.CheckSkill();
        }
        else
        {
            if (_ePhase == ePhase.Phase_1)
                return;

            _ePhase = ePhase.Phase_1;
        }
    }
}
