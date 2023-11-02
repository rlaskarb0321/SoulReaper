using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DamageBuff", menuName = "CustomScriptableObject/Buff/DamageBuff")]
public class DamageBuff : PlayerBuff
{
    [Header("=== Damage Buff ===")]
    [SerializeField]
    private float _atkIncrease;

    //public DamageBuff(float buffDur, float atkIncrease)
    //{
    //    _buffName = ConstData.DAMAGE_BUFF_NAME;
    //    _atkIncrease = atkIncrease;
    //}

    public override void BuffPlayer()
    {
        CharacterData.CData data = CharacterDataPackage._cDataInstance._characterData;
        float upgradeDamage = data._basicAtkDamage + _atkIncrease;

        UIScene._instance.UpdatePlayerDamage(upgradeDamage);
    }

    public override void ResetBuff()
    {
        CharacterData.CData data = CharacterDataPackage._cDataInstance._characterData;
        float originDamage = data._basicAtkDamage - _atkIncrease;
        if (originDamage <= 0.0f)
            originDamage = 0.0f;

        UIScene._instance.UpdatePlayerDamage(originDamage);

        _remainBuffDur = _buffDur;
    }
}