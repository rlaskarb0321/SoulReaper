using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISummonType
{
    /// <summary>
    /// ��ȯ�Ҷ� ���ð��� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    public void InitUnitData();

    public void CompleteSummon();
}

/// <summary>
/// �Ϲݸ��� ��ȯ��
/// </summary>
public class NormalSummonType : MonsterType, ISummonType
{
    public Material _originMat;

    private MonsterBase_1 _monsterBase;

    private void Awake()
    {
        _monsterBase = GetComponent<MonsterBase_1>();
        _monsterBase._stat.traceDist = 150.0f;
        _monsterBase._target = SearchTarget();
    }

    private void Update()
    {
        if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead)
            return;

        switch (_monsterBase._state)
        {
            case MonsterBase_1.eMonsterState.Trace:
                Trace();
                break;
        }
    }

    /// <summary>
    /// ��ȯ ���Ͱ� �׾����� anim event�� ���
    /// </summary>
    public void DeadSummonObj()
    {
        this.gameObject.SetActive(false);
    }

    public void InitUnitData()
    {
        // ���⿡ �Ϲݸ��͸� ���ȯ�ҋ� �⺻������ �ʱ�ȭ�ϴ� �ڵ� �ۼ�
        Material newMat = Instantiate(_originMat);

        gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        _monsterBase._animator.enabled = false;
        _monsterBase._nav.enabled = false;
        _monsterBase.GetComponent<CapsuleCollider>().enabled = false;
        _monsterBase.GetComponent<AudioSource>().enabled = false;

        _monsterBase._currHp = _monsterBase._stat.health;
        _monsterBase._state = MonsterBase_1.eMonsterState.Trace;
        _monsterBase._mesh.material = newMat;
    }

    public override GameObject SearchTarget()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _monsterBase._stat.traceDist, 1 << LayerMask.NameToLayer("PlayerTeam"));

        return colls[0].gameObject;
    }

    public override void Trace()
    {
        if (!_monsterBase._nav.enabled)
            return;

        float distance = Vector3.Distance(transform.position, _monsterBase._target.transform.position);
        if (distance <= _monsterBase._stat.attakDist)
        {
            _monsterBase._state = MonsterBase_1.eMonsterState.Attack;
            return;
        }

        _monsterBase.Move(_monsterBase._target.transform.position, _monsterBase._stat.movSpeed);
    }

    public void CompleteSummon()
    {
        _monsterBase._animator.enabled = true;
        _monsterBase._nav.enabled = true;
        _monsterBase.GetComponent<CapsuleCollider>().enabled = true;
        _monsterBase.GetComponent<AudioSource>().enabled = true;
    }
}
