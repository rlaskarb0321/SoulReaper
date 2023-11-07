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
        // ��ų�� On Off ���¸� �ٲ�
        _skillActiveState++;
        print((eSkillActiveState)((int)_skillActiveState % (int)eSkillActiveState.Count));
    }

    public void UseOnOffSkill()
    {
        // ��ų�� On�� ���¿��� ��¡�� ������ �ϰ� ���콺 �������� ��μ� ��ȭ�� ��ų�� ���
    }

    public override void UseSkill()
    {
        SwitchActive();
    }
}
