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

    // �ش� ��ų�� �켱����
    public int _priority;

    // �ش� ��ų�� ��Ÿ�� ��
    public float _coolTime;

    // ���� ��ٿ� ���� �ð�
    public float _currCoolTime;

    // ������ �ϱ� ���� ��ǥ�� �����ϰ� ��ų �Ÿ���, 0 �����̸� �������� �ʾƵ� �Ǵ� ��ų�̶� ��
    public float _attackRange;

    // ��ų�� ��� ���� ���θ� üũ�ϴ� ��������Ʈ
    public EditCanUseDelegate _editCanUseDelegate;

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
    public float _tiredDuration;
    public GameObject _bull;
    public GameObject _sweating;

    // Field
    private BossMonsterSkill _selectedSkill;
    private PartyMonster _monsterBase;
    private GameObject _target;
    private PartyBossPattern _pattern;
    private float _originActDelay;
    private float _originTiredDur;
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
        Run,
        Rest,
        Count,
    }

    private void Awake()
    {
        _monsterBase = GetComponent<PartyMonster>();
        _pattern = GetComponent<PartyBossPattern>();
        _bossWave = GetComponent<BossWave>();
    }

    private void Start()
    {
        _originActDelay = _monsterBase._stat.actDelay;
        _originTiredDur = _tiredDuration;
        _target = _bossWave._monsterBase._target;

        CheckSkill();
    }

    //private void Update()
    //{
    //    if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead)
    //        return;

    //    if (!_isBossTired)
    //    {
    //        switch (_monsterBase._state)
    //        {
    //            // ������ ���� ��ų�� ���� ������
    //            case MonsterBase_1.eMonsterState.Idle:
    //                float dist = Vector3.Distance(_monsterBase._target.transform.position, transform.position);
    //                if (_selectedSkill == null)
    //                    _selectedSkill = SelectSkill(_normalStateSkills);
    //                if (_selectedSkill._attackRange > 0.0f && dist > _selectedSkill._attackRange)
    //                {
    //                    Trace();
    //                    return;
    //                }

    //                DoSkill(_selectedSkill);
    //                break;

    //            case MonsterBase_1.eMonsterState.Attack:
    //                _monsterBase.AimingTarget(_target.transform.position, 2.0f);
    //                break;

    //            // ������ ���� �� �ణ�� ������ ������
    //            case MonsterBase_1.eMonsterState.Delay:
    //                if (_monsterBase._stat.actDelay <= 0.0f)
    //                {
    //                    _monsterBase._stat.actDelay = _originActDelay;
    //                    _monsterBase._state = MonsterBase_1.eMonsterState.Idle;
    //                    _selectedSkill = null;
    //                    return;
    //                }
                     
    //                _monsterBase._stat.actDelay -= Time.deltaTime;
    //                break;
    //        }
    //    }
    //    else
    //    {
    //        Recover();
    //        switch (_monsterBase._state)
    //        {
    //            case MonsterBase_1.eMonsterState.Idle:
    //                if (_selectedSkill == null)
    //                    _selectedSkill = SelectSkill(_tiredStateSkills);

    //                DoSkill(_selectedSkill);
    //                break;

    //            case MonsterBase_1.eMonsterState.Delay:
    //                if (_monsterBase._stat.actDelay <= 0.0f)
    //                {
    //                    _monsterBase._stat.actDelay = _originActDelay;
    //                    _monsterBase._state = MonsterBase_1.eMonsterState.Idle;
    //                    _selectedSkill = null;
    //                    return;
    //                }

    //                _monsterBase._stat.actDelay -= Time.deltaTime;
    //                break;
    //        }
    //    }
    //}

    public void CombatPartyMonster()
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
            Recover();
            switch (_monsterBase._state)
            {
                case MonsterBase_1.eMonsterState.Idle:
                    if (_selectedSkill == null)
                        _selectedSkill = SelectSkill(_tiredStateSkills);

                    DoSkill(_selectedSkill);
                    break;

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
        // �� �� �ִ� ��ų�� ���� ����
        if (skill == null)
            return;

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
                    ExitAttackState(1.0f);
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

            case ePartyBossSkill.Run:
                _pattern.Run();
                break;

            case ePartyBossSkill.Rest:
                _pattern.Rest();
                break;

            #endregion ������ ��ģ ������ �� ��밡���� ��ų��
        }
    }

    /// <summary>
    /// ��ų ����ϱ����� �Ÿ��� �������ϴ� �޼���
    /// </summary>
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
            EditSkillCondition(_normalStateSkills[i], _normalStateSkills[i]._eSkillCondition);
    }

    /// <summary>
    /// ��ų���� ��� ���� ���θ� ��Ž���ϰ� �ϴ� ��������Ʈ�� ȣ��
    /// </summary>
    private void EditSkillCondition
        (BossMonsterSkill skill, BossMonsterSkill.eSkillUseCondition useCondition)
    {
        switch (useCondition)
        {
            // ������ 2�� ��� �����ϵ��� ���� �ֱ�
            case BossMonsterSkill.eSkillUseCondition.Phase2:
                skill._editCanUseDelegate -= isPhaseTwo;
                skill._editCanUseDelegate += isPhaseTwo;

                skill._canUse = skill._editCanUseDelegate();
                break;

            case BossMonsterSkill.eSkillUseCondition.Phase3:
                skill._editCanUseDelegate -= isPhaseThree;
                skill._editCanUseDelegate += isPhaseThree;

                skill._canUse = skill._editCanUseDelegate();
                break;

            // �ָ��ִ��� üũ �� ��� �����ϵ��� ���� �ֱ�
            case BossMonsterSkill.eSkillUseCondition.Long:
                skill._editCanUseDelegate -= isPlayerFar;
                skill._editCanUseDelegate += isPlayerFar;

                skill._canUse = skill._editCanUseDelegate();
                break;

            // �÷��̾ �ڿ��ִ��� üũ �� ��� �����ϵ��� ���� �ֱ�
            case BossMonsterSkill.eSkillUseCondition.Behind:
                skill._editCanUseDelegate -= isPlayerBehind;
                skill._editCanUseDelegate += isPlayerBehind;

                skill._canUse = skill._editCanUseDelegate();
                break;
        }
    }

    /// <summary>
    /// �̴� ���� ��ȯ�Ϸ� �� ��ġ�� �����
    /// </summary>
    public void Tired()
    {
        _sweating.gameObject.SetActive(true);
        _isBossTired = true;
    }

    private void Recover()
    {
        if (!_isBossTired)
            return;

        if (_tiredDuration <= 0.0f)
        {
            _tiredDuration = _originTiredDur;
            _isBossTired = false;
            _sweating.gameObject.SetActive(false);
        }
        else
        {
            _tiredDuration -= Time.deltaTime;
        }
    }

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