using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 누군가가 소환할때 쓰이는 오오라에 사용되는 스크립트
/// </summary>
public class MonsterSummon : MonoBehaviour
{
    public GameObject _summonMonster;

    private IDisolveEffect _dissolve;
    private ISummonType _summonType;

    private void OnEnable()
    {
        _summonMonster.gameObject.SetActive(true);
        _summonType.InitUnitData();
    }

    private void Awake()
    {
        _dissolve = _summonMonster.GetComponent<IDisolveEffect>();
        _summonType = _summonMonster.GetComponent<ISummonType>();
    }

    public void SetMonsterAnimOn()
    {
        StartCoroutine(_dissolve.DissolveAppear());
    }

    public void SetMonsterAIOn()
    {
        _dissolve.CompleteDissloveAppear();
        gameObject.SetActive(false);
    }
}
