using UnityEngine;

public abstract class DataApply : MonoBehaviour
{
    public abstract void EditMapData();
}

public interface IDataApply
{
    /// <summary>
    /// 가져온 데이터를 게임에 적용시키는 메서드
    /// </summary>
    public void ApplyData();
}