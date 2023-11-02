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

    //public HPMPBuff(float buffDur, float hpIncrease, float mpIncrease) : base(buffDur) 
    //{
    //    _buffName = ConstData.HPMP_BUFF_NAME;

    //    _hpIncrease = hpIncrease;
    //    _mpIncrease = mpIncrease;
    //}

    public override void BuffPlayer()
    {
        CharacterData.CData data = CharacterDataPackage._cDataInstance._characterData;

        float upgradeCurrHP = data._currHP + _hpIncrease;
        float upgradeMaxHP = data._maxHP + _hpIncrease;
        float upgradeCurrMP = data._currMP + _mpIncrease;
        float upgradeMaxMP = data._maxMP + _mpIncrease;

        data._maxHP = upgradeMaxHP;
        data._currHP = upgradeCurrHP;

        data._maxMP = upgradeMaxMP;
        data._currMP = upgradeCurrMP;

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.HP, upgradeCurrHP, upgradeMaxHP, false);
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.MP, upgradeCurrMP, upgradeMaxMP, false);

        // 여기에 버프 데이터를 적용, 캐릭터 데이터 아님

    }

    public override void ResetBuff()
    {
        CharacterData.CData data = CharacterDataPackage._cDataInstance._characterData;

        float originMaxHP = data._maxHP - _hpIncrease;
        float originMaxMP = data._maxMP - _mpIncrease;

        data._maxHP = originMaxHP;
        data._maxMP = originMaxMP;

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.HP, data._currHP, originMaxHP, false);
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.MP, data._currMP, originMaxMP, false);

        _remainBuffDur = _buffDur;
    }
}
