using System.Collections;
using UnityEngine;

public abstract class PlayerBuff
{
    protected string _buffName;
    protected float _buffDur;
    protected float _remainBuffDur;
    protected WaitForSeconds _ws;

    public string BuffName { get { return _buffName; } }
    public float RemainBuffDur { get { return _remainBuffDur; } }

    public PlayerBuff(string buffName, float buffDur)
    {
        _buffName = buffName;
        _buffDur = buffDur;

        _remainBuffDur = _buffDur;
        _ws = new WaitForSeconds(1.0f);
    }

    /// <summary>
    /// �÷��̾��� �پ��� ������ ���� �����ִ� �Լ�
    /// </summary>
    public abstract void BuffPlayer();

    /// <summary>
    /// �÷��̾�� ������ ������ ����Ǳ� ������ ������ �Լ�
    /// </summary>
    public abstract void ResetBuff();

    /// <summary>
    /// ���� ����� ������ ���ӽð��� ��� �Լ�
    /// </summary>
    /// <returns></returns>
    public IEnumerator DecreaseBuffDur()
    {
        while (_remainBuffDur > 0.0f)
        {
            yield return _ws;

            _remainBuffDur -= 1.0f;
            Debug.Log(_buffName + "�� ���ӽð� : " + _remainBuffDur + " ����");
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

        // ���⿡ ���� �����͸� ����, ĳ���� ������ �ƴ�

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
    }
}

public class DamageBuff : PlayerBuff
{
    private float _atkIncrease;

    public DamageBuff(string buffName, float buffDur, float atkIncrease) : base(buffName, buffDur)
    {
        _atkIncrease = atkIncrease;
    }

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
    }
}