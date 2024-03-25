using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̰� �������̽��� �����ϰ�, DamageBuffPro.. �� HPMPBuffPro.. ���� ��ӽ�Ű��
/// </summary>
public abstract class BuffProvider : MonoBehaviour
{
    /// <summary>
    /// ���� Scriptable Object �� ����
    /// </summary>
    /// <returns></returns>
    public abstract PlayerBuff GenerateBuffInstance();
}
