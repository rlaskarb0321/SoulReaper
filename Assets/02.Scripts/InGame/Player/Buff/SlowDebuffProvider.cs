using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDebuffProvider : BuffProvider
{
    [SerializeField]
    private SlowDebuff _buff;

    public GameObject _buffEffect;

    public override PlayerBuff GenerateBuffInstance()
    {
        return _buff;
    }
}
