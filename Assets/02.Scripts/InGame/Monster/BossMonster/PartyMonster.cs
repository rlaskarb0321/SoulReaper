using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMonster : MonsterBase_1
{
    [Header("=== Boss_Party Monster ===")]
    [SerializeField]
    private SkinnedMeshRenderer[] _meshRenderes;

    protected override void Awake()
    {
        _meshRenderes = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public override IEnumerator OnHitEvent()
    {
        Material newMat;

        newMat = _hitMats[1];
        for (int i = 0; i < _meshRenderes.Length; i++)
            _meshRenderes[i].material = newMat;

        yield return new WaitForSeconds(Time.deltaTime * 6.0f);

        newMat = _hitMats[0];
        for (int i = 0; i < _meshRenderes.Length; i++)
            _meshRenderes[i].material = newMat;
    }
}
