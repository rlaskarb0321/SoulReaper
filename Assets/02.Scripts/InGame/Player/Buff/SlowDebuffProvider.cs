using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDebuffProvider : BuffProvider
{
    [SerializeField]
    private SlowDebuff _buff;

    public override PlayerBuff GenerateBuffInstance()
    {
        return _buff;
    }
}
