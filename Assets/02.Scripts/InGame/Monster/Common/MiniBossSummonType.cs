using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̴� ���� (Bull) ��ȯ��
/// </summary>
public class MiniBossSummonType : MonsterType, IDisolveEffect, ISummonType
{
    public Material _dissolveMat;
    public float _dissolveAmount;

    private MonsterBase_1 _monsterBase;
    private readonly int _hashRun = Animator.StringToHash("Run");

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

    private void Awake()
    {
        _monsterBase = GetComponent<MonsterBase_1>();
        _monsterBase._stat.traceDist = 150.0f;
        _monsterBase._target = SearchTarget();
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

        _monsterBase._animator.SetBool(_hashRun, true);
        if (distance <= _monsterBase._stat.attakDist)
        {
            _monsterBase._state = MonsterBase_1.eMonsterState.Attack;
            _monsterBase._animator.SetBool(_hashRun, false);
        }
        else
        {
            _monsterBase.Move(_monsterBase._target.transform.position, _monsterBase._stat.movSpeed);
        }
    }

    public void CompleteDissloveAppear()
    {
        _monsterBase._nav.enabled = true;
        _monsterBase.GetComponent<CapsuleCollider>().enabled = true;
        _monsterBase.GetComponentInChildren<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    public IEnumerator DissolveAppear()
    {
        Material newMat = Instantiate(_dissolveMat);
        float dissolveAmount = newMat.GetFloat("_DissolveAmount");

        _monsterBase._animator.enabled = true;
        while (dissolveAmount >= 0.0f)
        {
            dissolveAmount -= _dissolveAmount * Time.deltaTime;
            if (dissolveAmount <= 0.0f)
            {
                dissolveAmount = 0.0f;
                break;
            }

            newMat.SetFloat("_DissolveAmount", dissolveAmount);
            _monsterBase._mesh.material = newMat;
            yield return null;
        }
    }

    public void InitUnitData()
    {

    }
}
