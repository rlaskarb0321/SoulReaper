using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnOffSwitchSkill
{
    /// <summary>
    /// ��, ���� ��ȯ ������ ��ų�� �� ��ų�� ��, ������ ��ȯ
    /// </summary>
    public void SwitchActive(bool isAudioPlay);

    /// <summary>
    /// ��, ���� ��ų�� ��¥�� ����ϰ� �ϴ� �Լ�
    /// </summary>
    public void UseOnOffSkill();
}

public abstract class PlayerSkill : MonoBehaviour
{
    /// <summary>
    /// ��ų�� ����� �� ȣ��Ǵ� �Լ�
    /// </summary>
    public abstract void UseSkill();

    /// <summary>
    /// ������ ��ų ���� ���콺�� �÷��� �� ȣ��Ǵ� �Լ�
    /// </summary>
    public abstract void OnMouseOver();

    /// <summary>
    /// ������ ��ų�� ���� ������ �� ȣ��Ǵ� �Լ�
    /// </summary>
    public abstract void OnMouseDown();
}
