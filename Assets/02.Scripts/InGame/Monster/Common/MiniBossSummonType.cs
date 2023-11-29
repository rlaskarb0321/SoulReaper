using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 미니 보스 (Bull) 소환형
/// </summary>
public class MiniBossSummonType : MonsterType, IDisolveEffect, ISummonType
{
    public Material _dissolveMat;
    public float _dissolveAmount;

    private MonsterBase_1 _monsterBase;
    private Vector3 _originPos;
    private Quaternion _originRot;
    private readonly int _hashRun = Animator.StringToHash("Run");

    private void Awake()
    {
        _monsterBase = GetComponent<MonsterBase_1>();
        _monsterBase._stat.traceDist = 150.0f;
        _monsterBase._target = SearchTarget();
    }

    private void Start()
    {
        _originPos = Vector3.zero;
        _originRot = new Quaternion(0.0f, 180.0f, 0.0f, 0.0f);
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
        _monsterBase.GetComponent<AudioSource>().enabled = true;
    }

    public IEnumerator DissolveAppear()
    {
        Material newMat = Instantiate(_dissolveMat);
        float dissolveAmount = newMat.GetFloat("_DissolveAmount");
        newMat.SetFloat("_DissolveAmount", 1.0f);

        _monsterBase._mesh.material = newMat; 
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
        // 여기에 미니보스(Bull)을 재소환할떄 기본값으로 초기화하는 코드 작성
        Material newMat = Instantiate(_dissolveMat);
        float dissolveAmount = newMat.GetFloat("_DissolveAmount");
        newMat.SetFloat("_DissolveAmount", 1.0f);

        transform.localPosition = _originPos;
        transform.localRotation = _originRot;
        _monsterBase._animator.enabled = false;
        _monsterBase._nav.enabled = false;
        _monsterBase.GetComponent<CapsuleCollider>().enabled = false;
        _monsterBase._currHp = _monsterBase._stat.health;
        _monsterBase._state = MonsterBase_1.eMonsterState.Trace;
        _monsterBase.GetComponentInChildren<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _monsterBase._mesh.material = newMat;
    }
}
