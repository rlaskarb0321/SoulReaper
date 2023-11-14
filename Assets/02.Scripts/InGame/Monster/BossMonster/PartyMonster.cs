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

    private PartyMonsterCombat _monsterCombat;

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
