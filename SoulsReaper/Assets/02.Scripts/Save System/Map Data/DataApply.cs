using System.Collections;
using UnityEngine;

public abstract class DataApply : MonoBehaviour
{
    /// <summary>
    /// �������ִ� �����͸� �����ϰ� JSon �� ����
    /// </summary>
    public abstract void EditData();
}

public interface IDataApply
{
    /// <summary>
    /// ������ �����͸� ���ӿ� �����Ű�� �޼���
    /// </summary>
    public IEnumerator ApplyData();
}