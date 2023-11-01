using System.Collections;
using UnityEngine;

public abstract class PlayerBuff
{
    protected string _buffName;
    protected float _buffDur;
    protected float _remainBuffDur;
    protected WaitForSeconds _ws;

    public PlayerBuff(string buffName, float buffDur)
    {
        _buffName = buffName;
        _buffDur = buffDur;

        _remainBuffDur = _buffDur;
        _ws = new WaitForSeconds(1.0f);
    }

    public abstract void BuffPlayer();

    public abstract void ResetBuff();

    public IEnumerator DecreaseBuffDur()
    {
        while (_remainBuffDur > 0.0f)
        {
            yield return _ws;

            _remainBuffDur -= 1.0f;
            Debug.Log(_buffName + "의 지속시간 : " + _remainBuffDur + " 남음");
        }

        if (_remainBuffDur <= 0.0f)
            ResetBuff();
    }
}

public class HPMPBuff : PlayerBuff
{
    private float _hpIncrease;
    private float _mpIncrease;

    public HPMPBuff(string buffName, float buffDur, float hpIncrease, float mpIncrease) : base(buffName, buffDur) 
    {
        _hpIncrease = hpIncrease;
        _mpIncrease = mpIncrease;
    }

    public override void BuffPlayer()
    {
        CharacterData.CData data = CharacterDataPackage._characterData._characterData;

        float upgradeCurrHP = data._currHP + _hpIncrease;
        float upgradeMaxHP = data._maxHP + _hpIncrease;
        float upgradeCurrMP = data._currMP + _mpIncrease;
        float upgradeMaxMP = data._maxMP + _mpIncrease;

        data._maxHP = upgradeMaxHP;
        data._currHP = upgradeCurrHP;

        data._maxMP = upgradeMaxMP;
        data._currMP = upgradeCurrMP;

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.HP, upgradeCurrHP, upgradeMaxHP);
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.MP, upgradeCurrMP, upgradeMaxMP);

        // 여기에 버프 데이터를 적용

    }

    public override void ResetBuff()
    {
        CharacterData.CData data = CharacterDataPackage._characterData._characterData;

        float originMaxHP = data._maxHP - _hpIncrease;
        float originMaxMP = data._maxMP - _mpIncrease;

        data._maxHP = originMaxHP;
        data._maxMP = originMaxMP;

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.HP, data._currHP, originMaxHP);
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.MP, data._currMP, originMaxMP);
    }
}

//public class AttackDamageBuff : PlayerBuff
//{
//    private float _atkIncrease;

//    public AttackDamageBuff(string buffName, float buffDur, float atkIncrease) : base(buffName, buffDur)
//    {
//        _atkIncrease = atkIncrease;
//    }

//    public override void BuffPlayer(PlayerData data)
//    {

//    }
//}