using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brazier : MonoBehaviour
{
    [Header("=== Map ===")]
    [SerializeField] private A2RoomMgr _roomMgr;

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

        switch (projectile._arrowState)
        {
            case LaunchProjectile.ArrowState.Normal:
                if (_brazierState == eBrazier.Fire)
                    projectile.UpgradeFire();
                else
                    projectile.Boom();
                break;

            case LaunchProjectile.ArrowState.Fire:
                if (_brazierState == eBrazier.Normal)
                {
                    IgniteBrazier();
                    _roomMgr.ProceedingPuzzle();
                }
                else
                {
                    projectile.Boom();
                }
                break;
        }
    }

    private void IgniteBrazier()
    {
        _fireEffect.SetActive(true);
        _brazierState = eBrazier.Fire;
    }
}
