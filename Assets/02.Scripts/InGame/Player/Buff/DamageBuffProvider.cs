using UnityEngine;

public class DamageBuffProvider : BuffProvider
{
    [SerializeField]
    private DamageBuff _buff;

    private void Awake()
    {
        _buff = new DamageBuff(ConstData.DAMAGE_BUFF_DURATION, ConstData.DAMAGE_BUFF_AMOUNT);
    }

    public override PlayerBuff GenerateBuffInstance()
    {
        return _buff;
    }
}
