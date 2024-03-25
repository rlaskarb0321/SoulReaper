using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이거 인터페이스로 수정하고, DamageBuffPro.. 랑 HPMPBuffPro.. 에게 상속시키자
/// </summary>
public abstract class BuffProvider : MonoBehaviour
{
    /// <summary>
    /// 버프 Scriptable Object 를 전달
    /// </summary>
    /// <returns></returns>
    public abstract PlayerBuff GenerateBuffInstance();
}
