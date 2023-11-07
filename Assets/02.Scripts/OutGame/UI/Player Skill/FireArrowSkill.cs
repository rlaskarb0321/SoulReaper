using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrowSkill : PlayerSkill, IOnOffSwitchSkill
{
    private enum eSkillActiveState { DeActive, Active, Count, }
    private eSkillActiveState _skillActiveState;

    public override void OnMouseDown()
    {

    }

    public override void OnMouseOver()
    {

    }

    public void SwitchActive()
    {
        // 스킬의 On Off 상태를 바꿈
        _skillActiveState++;
        print((eSkillActiveState)((int)_skillActiveState % (int)eSkillActiveState.Count));
    }

    public void UseOnOffSkill()
    {
        // 스킬이 On인 상태에서 차징을 끝까지 하고 마우스 놓았을때 비로소 불화살 스킬을 사용
    }

    public override void UseSkill()
    {
        SwitchActive();
    }
}
