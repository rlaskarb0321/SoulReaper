using UnityEngine;

public abstract class DataApply : MonoBehaviour
{
    public abstract void EditMapData();
}

public interface IDataApply
{
    /// <summary>
    /// ������ �����͸� ���ӿ� �����Ű�� �޼���
    /// </summary>
    public void ApplyData();
}