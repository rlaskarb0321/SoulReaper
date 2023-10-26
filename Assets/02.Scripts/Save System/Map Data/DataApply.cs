using System.Collections;
using UnityEngine;

public abstract class DataApply : MonoBehaviour
{
    /// <summary>
    /// 가지고있는 데이터를 수정하고 JSon 에 저장
    /// </summary>
    public abstract void EditMapData();
}

public interface IDataApply
{
    /// <summary>
    /// 가져온 데이터를 게임에 적용시키는 메서드
    /// </summary>
    public IEnumerator ApplyData();
}