using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPooling
{
    /// <summary>
    /// ������ƮǮ���� ��Ҹ� ������ �Լ�
    /// </summary>
    public void PullOutObject();

    /// <summary>
    /// ������ƮǮ�� �⺻ ������� �� ������Ʈ�� �ʿ��� �� ����� �÷��ִ� �Լ�
    /// </summary>
    public void AddObject();
}

public class VFXPool : MonoBehaviour
{
    #region Pooling ������Ʈ�� �����͸� �ʱ�ȭ �����ִ� �������̵� �Լ���
    public virtual void SetPoolData() { }
    public virtual void SetPoolData(LaunchData data) { }
    #endregion Pooling ������Ʈ�� �����͸� �ʱ�ȭ �����ִ� �������̵� �Լ���
}
