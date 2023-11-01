public class DamageBuffProvider : BuffProvider
{
    public override PlayerBuff GenerateBuffInstance()
    {
        DamageBuff buff = new DamageBuff(ConstData.DAMAGE_BUFF_DURATION, ConstData.DAMAGE_BUFF_AMOUNT);
        return buff;
    }
}
