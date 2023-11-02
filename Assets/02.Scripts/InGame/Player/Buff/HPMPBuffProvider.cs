using UnityEngine; 

public class HPMPBuffProvider : BuffProvider
{
    [SerializeField]
    private HPMPBuff _buff;

    public override PlayerBuff GenerateBuffInstance()
    {
        return _buff;
    }
}