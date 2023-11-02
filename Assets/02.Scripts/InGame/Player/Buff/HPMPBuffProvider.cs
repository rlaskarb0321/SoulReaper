using UnityEngine; 

public class HPMPBuffProvider : BuffProvider
{
    [SerializeField]
    private HPMPBuff _buff;

    private void Awake()
    {
        _buff = new HPMPBuff(ConstData.HPMP_BUFF_DURATION, ConstData.HP_BUFF_AMOUNT, ConstData.MP_BUFF_AMOUNT);
    }

    public override PlayerBuff GenerateBuffInstance()
    {
        return _buff;
    }
}