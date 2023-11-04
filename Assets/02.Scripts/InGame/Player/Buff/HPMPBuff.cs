using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "HPMPBuff", menuName = "CustomScriptableObject/Buff/HPMPBuff")]
public class HPMPBuff : PlayerBuff
{
    [Header("=== HPMP Buff ===")]
    [SerializeField]
    private float _hpIncrease;

    [SerializeField]
    private float _mpIncrease;

    private float _originCurrHP;
    private float _originMaxHP;
    private float _originCurrMP;
    private float _originMaxMP;

    public override void BuffPlayer()
    {
        CharacterData.CData data = CharacterDataPackage._cDataInstance._characterData;

        _originCurrHP = data._currHP;
        _originMaxHP = data._maxHP;
        _originCurrMP = data._currMP;
        _originMaxMP = data._maxMP;

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.HP, _originCurrHP + _hpIncrease, _originMaxHP + _hpIncrease, false);
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.MP, _originCurrMP + _mpIncrease, _originMaxMP + _mpIncrease, false);
    }

    public override void ResetBuff()
    {
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.HP, _originCurrHP, _originMaxHP, false);
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.MP, _originCurrMP, _originMaxMP, false);

        _remainBuffDur = _buffDur;
    }
}
