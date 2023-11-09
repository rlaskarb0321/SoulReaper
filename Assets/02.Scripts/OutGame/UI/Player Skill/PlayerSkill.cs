using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnOffSwitchSkill
{
    /// <summary>
    /// 온, 오프 전환 가능한 스킬일 때 스킬을 온, 오프로 전환
    /// </summary>
    public void SwitchActive(bool isAudioPlay);

    /// <summary>
    /// 온, 오프 스킬을 진짜로 사용하게 하는 함수
    /// </summary>
    public void UseOnOffSkill();
}

public abstract class PlayerSkill : MonoBehaviour
{
    /// <summary>
    /// 스킬을 사용할 때 호출되는 함수
    /// </summary>
    public abstract void UseSkill();

    /// <summary>
    /// 유저가 스킬 위에 마우스를 올렸을 때 호출되는 함수
    /// </summary>
    public abstract void OnMouseOver();

    /// <summary>
    /// 유저가 스킬을 직접 눌렀을 때 호출되는 함수
    /// </summary>
    public abstract void OnMouseDown();
}
