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

    /// <summary>
    /// ��� �Ϸ�� ������Ʈ�� Ǯ�� ��ȯ��Ű�� �Լ�
    /// </summary>
    public void ReturnObject();
}

public class VFXPool : MonoBehaviour
{
    public GameObject _objectPoolManager;
    public IObjectPooling _pooling;

    #region Pooling ������Ʈ�� �����͸� �ʱ�ȭ �����ִ� �������̵� �Լ���
    public virtual void SetPoolData() { }
    public virtual void SetPoolData(LaunchData data) { }
    #endregion Pooling ������Ʈ�� �����͸� �ʱ�ȭ �����ִ� �������̵� �Լ���
}
