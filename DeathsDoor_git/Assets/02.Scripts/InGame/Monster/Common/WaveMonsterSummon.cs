using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 웨이브 형식으로 몬스터를 소환할 때 쓰는 스크립트
/// </summary>
public class WaveMonsterSummon : MonoBehaviour
{
    public GameObject _monster;

    private IDisolveEffect _dissolve;

    private void Awake()
    {
        _dissolve = _monster.GetComponent<IDisolveEffect>();
    }

    public void SetMonsterAnimOn()
    {
        StartCoroutine(_dissolve.DissolveAppear());
    }

    public void SetMonsterAIOn()
    {
        _dissolve.CompleteSummon();
        gameObject.SetActive(false);
    }
}
