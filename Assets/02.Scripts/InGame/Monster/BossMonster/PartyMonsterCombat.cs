using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PartyMonsterSkill
{
    // �ش� ��ų�� ����� �� ������ ����
    public bool _canUse;

    // ��ų���� �����ϱ� ���� �̸�
    public string _id;

    // �ش� ��ų�� ���� Ư���� ��Ȳ
    public eSkillUseCondition _skillCondition;

    // �ش� ��ų�� ����� ���� ���׷��̵� Ȥ�� �ٿ�׷��̵�Ǵ����� ����
    public eSkillUpgrade _skillUpgrade;

    // �ش� ��ų�� �켱����
    public int _priority;

    // �ش� ��ų�� ��Ÿ�� ��
    public float _coolTime;

    // ���� ��ٿ� ���� �ð�
    public float _currCoolTime; 

    // ��������Ʈ �޾Ƴ���, ��������Ʈ ȣ���ϴ°͵� �����


    /// <summary>
    /// �ش� ��ų�� ������ �޾��ִ� enum
    /// </summary>
    public enum eSkillUseCondition 
    { 
        None,       // �ش� ��ų�� ��� ���ɿ� ������ ����
        Phase2,     // �ش� ��ų�� phase2 �� ���� ��� ����
        Long,       // �ش� ��ų�� �÷��̾ �ſ� �ָ� ���� �� ��� ��������
        Behind,     // �ش� ��ų�� �÷��̾ �ڽ��� �ڿ� ���� �� ��� ��������
    }

    /// <summary>
    /// �ش� ��ų�� ���׷��̵� ���θ� �˷��ִ� enum
    /// </summary>
    public enum eSkillUpgrade
    { 
        None,           // �ش� ��ų�� ������ ��ȯ �� ���׷��̵� Ȥ�� �ٿ�׷��̵� ���� ����
        Phase2_Up,      // �ش� ��ų�� ������ 2�� ���׷��̵� ��
        Phase2_Down,    // �ش� ��ų�� ������ 2�� �ٿ�׷��̵� ��
    }
}

public class PartyMonsterCombat : MonoBehaviour
{
    public PartyMonsterSkill[] _normalStateSkills;
    public PartyMonsterSkill[] _tiredStateSkills;
    public bool _isBossTired;

    private void Awake()
    {
        InitSkill();
    }

    private void InitSkill()
    {
        for (int i = 0; i < _normalStateSkills.Length; i++)
            EditSkillCondition(_normalStateSkills[i], _normalStateSkills[i]._skillCondition, _normalStateSkills[i]._skillUpgrade);
    }

    private void EditSkillCondition
        (PartyMonsterSkill skill, PartyMonsterSkill.eSkillUseCondition useCondition, PartyMonsterSkill.eSkillUpgrade upgradeCondition)
    {
        switch (useCondition)
        {
            // ������ �׳� �н�
            case PartyMonsterSkill.eSkillUseCondition.None:
                break;
            // ������ 2�� ��� �����ϵ��� ���� �ֱ�
            case PartyMonsterSkill.eSkillUseCondition.Phase2:
                break;
            // �ָ��ִ��� üũ �� ��� �����ϵ��� ���� �ֱ�
            case PartyMonsterSkill.eSkillUseCondition.Long:
                break;
            // �÷��̾ �ڿ��ִ��� üũ �� ��� �����ϵ��� ���� �ֱ�
            case PartyMonsterSkill.eSkillUseCondition.Behind:
                break;
        }

        switch (upgradeCondition)
        {
            case PartyMonsterSkill.eSkillUpgrade.None:
                break;
            case PartyMonsterSkill.eSkillUpgrade.Phase2_Up:
                break;
            case PartyMonsterSkill.eSkillUpgrade.Phase2_Down:
                break;
        }
    }
}