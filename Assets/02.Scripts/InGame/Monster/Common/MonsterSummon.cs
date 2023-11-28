using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 누군가가 소환할때 쓰이는 오오라에 사용되는 스크립트
/// </summary>
public class MonsterSummon : MonoBehaviour
{
    public GameObject _summonMonster;

    private IDisolveEffect _dissolve; // SummonMonster 객체에 있는 IDisolveEffect 인터페이스를 할당
    private ISummonType _summonType; // SummonMonster 객체에 있는 ISummonType 인터페이스를 할당

    private void OnEnable()
    {
        _summonMonster.gameObject.SetActive(true);
        _summonType.InitUnitData();
    }

    private void Awake()
    {
        _dissolve = _summonMonster.GetComponent<IDisolveEffect>();
        _summonType = _summonMonster.GetComponent<ISummonType>();
        // _summonMonster.GetComponent<NormalSummonType>()._aura = this.gameObject;
    }

    public void SetMonsterAnimOn()
    {
        StartCoroutine(_dissolve.DissolveAppear());
    }

    public void SetMonsterAIOn()
    {
        _dissolve.CompleteDissloveAppear();
    }
}
