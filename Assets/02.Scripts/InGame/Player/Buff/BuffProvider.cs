using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffProvider : MonoBehaviour
{
    /// <summary>
    /// 버프 Scriptable Object 를 전달
    /// </summary>
    /// <returns></returns>
    public abstract PlayerBuff GenerateBuffInstance();
}
