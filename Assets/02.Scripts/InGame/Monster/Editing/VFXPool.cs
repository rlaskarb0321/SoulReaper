using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPool : MonoBehaviour
{
    #region Pooling 오브젝트의 데이터를 초기화 시켜주는 오버라이딩 함수들
    public virtual void SetPoolData() { }
    public virtual void SetPoolData(LaunchData data) { }
    #endregion Pooling 오브젝트의 데이터를 초기화 시켜주는 오버라이딩 함수들
}
