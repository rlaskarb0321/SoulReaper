using UnityEngine;

public class DamageBuffProvider : BuffProvider
{
    [SerializeField]
    private PlayerBuff _buff;

    public override PlayerBuff GenerateBuffInstance()
    {
        return _buff;
    }
}
