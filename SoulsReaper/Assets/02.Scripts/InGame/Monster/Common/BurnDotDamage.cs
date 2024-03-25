using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Burn", menuName = "CustomScriptableObject/DeBuff/Burn")]
public class BurnDotDamage : ScriptableObject
{
    public float _debuffDur;
    public float _dotDamamge;
    public float _dotInterval;
    public int _maxStackCount; // ������ ��ӽ��̴µ�, ������ ������ ����� �ִ� ������ ����
}
