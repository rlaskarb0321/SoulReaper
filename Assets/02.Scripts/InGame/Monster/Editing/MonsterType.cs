using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterType : MonoBehaviour
{
    /// <summary>
    /// �ش� ��ũ��Ʈ�� ��ӹ޴� Ŭ������ ���͵��� ���� ã�� ���
    /// </summary>
    public virtual GameObject SearchTarget() { return null; }

    public virtual void Trace() { }
}
