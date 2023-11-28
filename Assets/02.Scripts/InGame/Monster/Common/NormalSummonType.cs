using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISummonType
{
    /// <summary>
    /// 소환할때 관련값들 초기화하는 함수
    /// </summary>
    public void InitUnitData();
}

/// <summary>
/// 일반몬스터 소환형
/// </summary>
public class NormalSummonType : MonsterType, IDisolveEffect, ISummonType
{
    public Material _dissolveMat;
    public float _dissolveAmount;
    public GameObject _aura;

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
    /// 소환 몬스터가 죽었을때 anim event로 등록
    /// </summary>
    public void DeadSummonObj()
    {
        _aura.gameObject.SetActive(false);
    }

    public void InitUnitData()
    {
        // 여기에 일반몬스터를 재소환할떄 기본값으로 초기화하는 코드 작성
        Material newMat = Instantiate(_dissolveMat);
        float dissolveAmount = newMat.GetFloat("_DissolveAmount");
        newMat.SetFloat("_DissolveAmount", 1.0f);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _monsterBase._animator.enabled = false;
        _monsterBase._nav.enabled = false;
        _monsterBase.GetComponent<CapsuleCollider>().enabled = false;
        _monsterBase._currHp = _monsterBase._stat.health;
        _monsterBase._state = MonsterBase_1.eMonsterState.Trace;
        _monsterBase.GetComponentInChildren<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
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

    public void CompleteDissloveAppear()
    {
        _monsterBase._nav.enabled = true;
        _monsterBase.GetComponent<CapsuleCollider>().enabled = true;
        _monsterBase.GetComponentInChildren<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}
