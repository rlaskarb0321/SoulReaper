public class HPMPBuffProvider : BuffProvider
{
    public override PlayerBuff GenerateBuffInstance()
    {
        HPMPBuff buff = new HPMPBuff(ConstData.HPMP_BUFF_DURATION, ConstData.HP_BUFF_AMOUNT, ConstData.MP_BUFF_AMOUNT);
        return buff;
    }
}