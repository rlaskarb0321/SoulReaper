using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPooling
{
    /// <summary>
    /// 오브젝트풀에서 요소를 꺼내는 함수
    /// </summary>
    public void PullOutObject();

    /// <summary>
    /// 오브젝트풀의 기본 사이즈보다 더 오브젝트가 필요할 때 사이즈를 늘려주는 함수
    /// </summary>
    public void AddObject();
}

public class VFXPool : MonoBehaviour
{
    #region Pooling 오브젝트의 데이터를 초기화 시켜주는 오버라이딩 함수들
    public virtual void SetPoolData() { }
    public virtual void SetPoolData(LaunchData data) { }
    #endregion Pooling 오브젝트의 데이터를 초기화 시켜주는 오버라이딩 함수들
}
