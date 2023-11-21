using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate bool EditCanUseDelegate();

[Serializable]
public class BossMonsterSkill
{
    // �ش� ��ų�� ����� �� ������ ����
    public bool _canUse;

    // ��ų���� �����ϱ� ���� �̸�
    public string _id;
    
    // �ش� ��ų�� ���� Ư���� ��Ȳ
    public eSkillUseCondition _eSkillCondition;

    // �ش� ��ų�� ����� ���� ���׷��̵� Ȥ�� �ٿ�׷��̵�Ǵ����� ����
    public eSkillUpgrade _eSkillUpgrade;

    // �ش� ��ų�� �켱����
    public int _priority;

    // �ش� ��ų�� ��Ÿ�� ��
    public float _coolTime;

    // ���� ��ٿ� ���� �ð�
    public float _currCoolTime;

    // ������ �ϱ� ���� ��ǥ�� �����ϰ� ��ų �Ÿ���, 0 �����̸� �������� �ʾƵ� �Ǵ� ��ų�̶� ��
    public float _attackRange;

    // ��ų�� ��� ���� ���θ� üũ�ϴ� ��������Ʈ
    public static EditCanUseDelegate _editCanUseDelegate;

    /// <summary>
    /// �ش� ��ų�� ������ �޾��ִ� enum
    /// </summary>
    public enum eSkillUseCondition 
    { 
        None,           // �ش� ��ų�� ��� ���ɿ� ������ ����
        Phase2,         // �ش� ��ų�� phase2 �� ���� ��� ����
        Phase3,         // �ش� ��ų�� phase3 �� ���� ��� ����
        Long,           // �ش� ��ų�� �÷��̾ �ſ� �ָ� ���� �� ��� ��������
        Behind,         // �ش� ��ų�� �÷��̾ �ڽ��� �ڿ� ���� �� ��� ��������
    }

    /// <summary>
    /// �ش� ��ų�� ���׷��̵� ���θ� �˷��ִ� enum
    /// </summary>
    public enum eSkillUpgrade
    { 
        None,           // �ش� ��ų�� ������ ��ȯ �� ���׷��̵� Ȥ�� �ٿ�׷��̵� ���� ����
        Phase2_Up,      // �ش� ��ų�� ������ 2�� ���׷��̵� ��
        Phase2_Down,    // �ش� ��ų�� ������ 2�� �ٿ�׷��̵� ��
        Phase3_Up,      // �ش� ��ų�� ������ 3�� ���׷��̵� ��                                              
        Phase3_Down,    // �ش� ��ų�� ������ 3�� �ٿ�׷��̵� ��
    }

    /// <summary>
    /// �ش� ��ų�� ��ٿ� ������
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoolDown()
    {
        while (_currCoolTime >= 0.0f)
        {
            _currCoolTime -= Time.deltaTime;
            yield return null;
        }

        _currCoolTime = _coolTime;
    }
}

public class PartyMonsterCombat : MonoBehaviour
{
    public BossMonsterSkill[] _normalStateSkills;
    public BossMonsterSkill[] _tiredStateSkills;
    public bool _isBossTired;
    public GameObject _bull;

    // Field
    private BossMonsterSkill _selectedSkill;
    private PartyMonster _monsterBase;
    private GameObject _target;
    private PartyBossPattern _pattern;
    private float _originActDelay;
    private BossWave _bossWave;
    private enum ePartyBossSkill 
    {
        // Normal State
        Summon_Mini_Boss,
        Drop_Kick,
        Summon_Normal_Monster,
        Blink,
        Sliding,
        Jump,
        Fist,
        Push,

        // Tired State
        Rest,
        Run,
        Count,
    }

    private void Awake()
    {
        _monsterBase = GetComponent<PartyMonster>();
        _pattern = GetComponent<PartyBossPattern>();
        _bossWave = GetComponent<BossWave>();
        _target = _monsterBase._target;

        CheckSkill();
    }

    private void Start()
    {
        _originActDelay = _monsterBase._stat.actDelay;
    }

    private void Update()
    {
        if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead)
            return;

        if (!_isBossTired)
        {
            switch (_monsterBase._state)
            {
                // ������ ���� ��ų�� ���� ������
                case MonsterBase_1.eMonsterState.Idle:
                    float dist = Vector3.Distance(_monsterBase._target.transform.position, transform.position);
                    if (_selectedSkill == null)
                        _selectedSkill = SelectSkill(_normalStateSkills);
                    if (_selectedSkill._attackRange > 0.0f && dist > _selectedSkill._attackRange)
                    {
                        Trace();
                        return;
                    }

                    DoSkill(_selectedSkill);
                    break;

                case MonsterBase_1.eMonsterState.Trace:
                    break;

                case MonsterBase_1.eMonsterState.Attack:
                    _monsterBase.AimingTarget(_target.transform.position, 2.0f);
                    break;

                // ������ ���� �� �ణ�� ������ ������
                case MonsterBase_1.eMonsterState.Delay:
                    if (_monsterBase._stat.actDelay <= 0.0f)
                    {
                        _monsterBase._stat.actDelay = _originActDelay;
                        _monsterBase._state = MonsterBase_1.eMonsterState.Idle;
                        _selectedSkill = null;
                        return;
                    }
                     
                    _monsterBase._stat.actDelay -= Time.deltaTime;
                    break;
            }
        }
        else
        {
            switch (_monsterBase._state)
            {
                case MonsterBase_1.eMonsterState.Idle:
                    if (_selectedSkill == null)
                        _selectedSkill = SelectSkill(_normalStateSkills);

                    DoSkill(_selectedSkill);
                    break;

                case MonsterBase_1.eMonsterState.Delay:
                    _monsterBase._state = MonsterBase_1.eMonsterState.Idle;
                    break;
            }
        }
    }

    /// <summary>
    /// ��ų����Ʈ���� ��� �����ϰ� �켱������ ���� ��ų�� ��
    /// </summary>
    private BossMonsterSkill SelectSkill(BossMonsterSkill[] skillPack)
    {
        // �켱������ �̹� �ν����� â�� skillPack ���� �� �����ؼ� �������
        for (int i = 0; i < skillPack.Length; i++)
        {
            if (!skillPack[i]._canUse)
            {
                //print(skillPack[i] + " ��� �Ұ���");
                continue;
            }
            if (skillPack[i]._currCoolTime != skillPack[i]._coolTime)
            {
                //print(skillPack[i] + " ��Ÿ���� �� ��������");
                continue;
            }

            //print(skillPack[i] + " �� ����");
            return skillPack[i];
        }

        return null;
    }

    /// <summary>
    /// ����� ��ų�� ID �� �ް�, ������ �����ϰ� �ϴ� �Լ�
    /// </summary>
    /// <param name="skillID"></param>
    private void DoSkill(BossMonsterSkill skill)
    {
        //// �� �� �ִ� ��ų�� ���� ����
        //if (skillID == null || skillID == "")
        //{
        //}

        string skillID = skill._id;
        int skillIndex = int.Parse(skillID.Split('_')[0]);
        ePartyBossSkill bossSkill = (ePartyBossSkill)skillIndex;

        _monsterBase._animator.SetBool(_monsterBase._hashMove, false);
        _monsterBase._nav.enabled = false;
        _monsterBase._state = MonsterBase_1.eMonsterState.Attack;
        
        StartCoroutine(skill.CoolDown());
        switch (bossSkill)
        {
            #region ������ ��ġ�� ���� ������ �� ��밡���� ��ų��

            case ePartyBossSkill.Summon_Mini_Boss:
                if (_bull.activeSelf)
                {
                    _monsterBase._state = MonsterBase_1.eMonsterState.Delay;
                    CheckSkill();
                    return;
                }

                _pattern.SummonMiniBoss();
                break;

            case ePartyBossSkill.Drop_Kick:
                _pattern.DropKick();
                break;

            case ePartyBossSkill.Summon_Normal_Monster:
                _pattern.SummonNormalMonster();
                break;

            case ePartyBossSkill.Blink:
                _pattern.Blink();
                break;

            case ePartyBossSkill.Sliding:
                _pattern.Sliding();
                break;

            case ePartyBossSkill.Jump:
                _pattern.Jump();
                break;

            case ePartyBossSkill.Fist:
                _pattern.Fist();
                break;

            case ePartyBossSkill.Push:
                _pattern.Push();
                break;

            #endregion ������ ��ġ�� ���� ������ �� ��밡���� ��ų��

            #region ������ ��ģ ������ �� ��밡���� ��ų��

            case ePartyBossSkill.Rest:
                _pattern.Rest();
                break;

            case ePartyBossSkill.Run:
                _pattern.Run();
                break;

            #endregion ������ ��ģ ������ �� ��밡���� ��ų��
        }
    }

    private void Trace()
    {
        _monsterBase._nav.enabled = true;
        _bossWave.Trace();
    }


    /// <summary>
    /// ��ų�� ������ ��, �ִϸ��̼� �������� ��Ͻ�ų �޼���, ������ ���·� ����� �ش�.
    /// </summary>
    public void ExitAttackState(float amount)
    {
        if (amount == 0.0f)
            amount = 1.0f;
        if (amount < 0.0f)
            amount = 0.0f;

        _monsterBase._stat.actDelay = amount;
        _monsterBase._state = MonsterBase_1.eMonsterState.Delay;
    }

    /// <summary>
    /// ��������Ʈ ȣ���ϴ� �޼��带 �ܺο��� �θ��� �����ϱ� ���� ������ �޼���
    /// </summary>
    public void CheckSkill()
    {
        for (int i = 0; i < _normalStateSkills.Length; i++)
            EditSkillCondition(_normalStateSkills[i], _normalStateSkills[i]._eSkillCondition, _normalStateSkills[i]._eSkillUpgrade);
    }

    /// <summary>
    /// ��ų���� ��� ���� ���θ� ��Ž���ϰ� �ϴ� ��������Ʈ�� ȣ��
    /// </summary>
    private void EditSkillCondition
        (BossMonsterSkill skill, BossMonsterSkill.eSkillUseCondition useCondition, BossMonsterSkill.eSkillUpgrade upgradeCondition)
    {
        switch (useCondition)
        {
            // ������ 2�� ��� �����ϵ��� ���� �ֱ�
            case BossMonsterSkill.eSkillUseCondition.Phase2:
                BossMonsterSkill._editCanUseDelegate -= isPhaseTwo;
                BossMonsterSkill._editCanUseDelegate += isPhaseTwo;

                skill._canUse = BossMonsterSkill._editCanUseDelegate();
                break;

            case BossMonsterSkill.eSkillUseCondition.Phase3:
                BossMonsterSkill._editCanUseDelegate -= isPhaseThree;
                BossMonsterSkill._editCanUseDelegate += isPhaseThree;

                skill._canUse = BossMonsterSkill._editCanUseDelegate();
                break;

            // �ָ��ִ��� üũ �� ��� �����ϵ��� ���� �ֱ�
            case BossMonsterSkill.eSkillUseCondition.Long:
                BossMonsterSkill._editCanUseDelegate -= isPlayerFar;
                BossMonsterSkill._editCanUseDelegate += isPlayerFar;

                skill._canUse = BossMonsterSkill._editCanUseDelegate();
                break;

            // �÷��̾ �ڿ��ִ��� üũ �� ��� �����ϵ��� ���� �ֱ�
            case BossMonsterSkill.eSkillUseCondition.Behind:
                BossMonsterSkill._editCanUseDelegate -= isPlayerBehind;
                BossMonsterSkill._editCanUseDelegate += isPlayerBehind;

                skill._canUse = BossMonsterSkill._editCanUseDelegate();
                break;
        }

        switch (upgradeCondition)
        { 
            case BossMonsterSkill.eSkillUpgrade.Phase2_Up:
                break;
            case BossMonsterSkill.eSkillUpgrade.Phase2_Down:
                break;
        }

    }

    /// <summary>
    /// �̴� ���� ��ȯ�Ϸ� �� ��ġ�� �����
    /// </summary>
    public void Tired() => _isBossTired = true;

    #region ��ų�� CanUSe �� �����ϱ� ���� ��ų�� Delegate �� �޾Ƴ��� �޼����

    public bool isPlayerBehind()
    {
        // �ڿ��ְ� �����̿� �ִ���
        float angle = Vector3.SignedAngle(transform.forward, _target.transform.position - transform.position, transform.up);
        float distance = Vector3.Distance(transform.position, _target.transform.position);

        return Mathf.Abs(angle) >= 100.0f && distance <= 2.5f;
    }

    public bool isPlayerFar()
    {
        return Vector3.Distance(_target.transform.position, transform.position) >= 3.0f;
    }

    public bool isPhaseTwo()
    {
        return _monsterBase.Phase == PartyMonster.ePhase.Phase_2;
    }

    public bool isPhaseThree()
    {
        return _monsterBase.Phase == PartyMonster.ePhase.Phase_3;
    }

    #endregion ��ų�� CanUSe �� �����ϱ� ���� ��ų�� Delegate �� �޾Ƴ��� �޼����
}