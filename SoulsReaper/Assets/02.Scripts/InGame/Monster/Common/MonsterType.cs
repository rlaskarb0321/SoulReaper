using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterType : MonoBehaviour
{
    /// <summary>
    /// �ش� ��ũ��Ʈ�� ��ӹ޴� Ŭ������ ���͵��� ���� ã�� ���
    /// </summary>
    public virtual GameObject SearchTarget(float searchRange) { return null; }

    /// <summary>
    /// ���� �Ѿư���
    /// </summary>
    public virtual void Trace() { }

    /// <summary>
    /// ���� Ÿ�Ժ��� ������ �޾��� ��, Ư�� �̺�Ʈ�� �߻��� ��� �ۼ��ϴ� �޼���
    /// </summary>
    public virtual void ReactDamaged() { }
}
