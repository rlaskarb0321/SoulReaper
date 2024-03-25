using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterType : MonoBehaviour
{
    /// <summary>
    /// 해당 스크립트를 상속받는 클래스별 몬스터들의 적을 찾는 방식
    /// </summary>
    public virtual GameObject SearchTarget(float searchRange) { return null; }

    /// <summary>
    /// 적을 쫓아가기
    /// </summary>
    public virtual void Trace() { }

    /// <summary>
    /// 몬스터 타입별로 데미지 받았을 때, 특정 이벤트가 발생할 경우 작성하는 메서드
    /// </summary>
    public virtual void ReactDamaged() { }
}
