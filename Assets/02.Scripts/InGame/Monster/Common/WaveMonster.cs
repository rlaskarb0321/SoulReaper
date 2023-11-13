using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMonster : MonsterType
{
    [Header("=== Wave ===")]
    public RaidWave _waveMaster;
    public Material _dissolveMat;
    public float _dissolveAmount;

    [Header("=== MonsterBase ===")]
    public MonsterBase_1 _monsterBase;

    protected bool _isAlert;

    protected virtual void Awake()
    {
        _monsterBase._stat.traceDist = 150.0f;
        _monsterBase._target = SearchTarget();
    }

    protected virtual void Update()
    {
        if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead)
        {
            if (_isAlert)
                return;

            AlertDead();
        }

        switch (_monsterBase._state)
        {
            case MonsterBase_1.eMonsterState.Trace:
                Trace();
                break;
        }
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

    // 해당 몬스터가 포함된 웨이브 관리하는 Obj에게 본인이 죽었음을 알림
    public void AlertDead()
    {
        _isAlert = true;
        _waveMaster.DecreaseMonsterCount();
    }

    /// <summary>
    /// Dissolve 를 이용해서 등장하기
    /// </summary>
    /// <returns></returns>
    public IEnumerator DissolveAppear()
    {
        Material newMat = Instantiate(_dissolveMat);
        float dissolveAmount = newMat.GetFloat("_DissolveAmount");

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
}
