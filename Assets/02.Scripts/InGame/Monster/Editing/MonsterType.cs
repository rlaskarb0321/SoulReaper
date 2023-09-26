using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterType : MonoBehaviour
{
    /// <summary>
    /// 해당 스크립트를 상속받는 클래스별 몬스터들의 적을 찾는 방식
    /// </summary>
    public virtual GameObject SearchTarget() { return null; }

    public virtual void Trace() { }
}
