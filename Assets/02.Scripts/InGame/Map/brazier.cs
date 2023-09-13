using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brazier : MonoBehaviour
{
    [Header("=== Map ===")]
    [SerializeField] private QuestRoom _roomMgr;

    public enum eBrazier { Normal, Fire }
    [Header("=== Brazier ===")]
    [SerializeField] public eBrazier _brazierState;
    [SerializeField] private GameObject _fireEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerProjectile"))
            return;

        LaunchProjectile projectile = other.GetComponent<LaunchProjectile>();

        if (projectile == null)
            return;

        // 트리거된 화살의 상태와 본인(화로)의 상태에 따른 결과값 진행
        switch (projectile._arrowState)
        {
            case LaunchProjectile.ArrowState.Normal:
                if (_brazierState == eBrazier.Fire) projectile.UpgradeFire();
                else projectile.Boom();
                break;

            case LaunchProjectile.ArrowState.Fire:
                if (_brazierState == eBrazier.Normal) IgniteBrazier(); 
                else projectile.Boom(); 
                break;
        }
    }

    // 본인(화로)를 점화시키는 함수
    private void IgniteBrazier()
    {
        _fireEffect.SetActive(true);
        _brazierState = eBrazier.Fire;
        _roomMgr.SolveQuest();
    }
}
