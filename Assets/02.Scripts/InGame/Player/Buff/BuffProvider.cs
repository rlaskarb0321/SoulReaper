using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffProvider : MonoBehaviour
{
    /// <summary>
    /// ���� Scriptable Object �� ����
    /// </summary>
    /// <returns></returns>
    public abstract PlayerBuff GenerateBuffInstance();
}
