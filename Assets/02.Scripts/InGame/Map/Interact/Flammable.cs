using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    public enum eFireState { Normal, Fire }

    [Header("=== Map ===")]
    [SerializeField] private QuestRoom _roomMgr;

    [Header("=== Fire State ===")]
    public eFireState _fireState;
    [SerializeField] protected GameObject _fireEffect;

    [Header("=== Data ===")]
    public DataApply _apply;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerProjectile"))
            return;

        LaunchProjectile projectile = other.GetComponent<LaunchProjectile>();

        if (projectile == null)
            return;

        // 트리거된 화살의 상태와 본인의 상태에 따른 결과값 진행
        switch (projectile._arrowState)
        {
            case eArrowState.Normal:
                if (_fireState == eFireState.Fire) projectile.UpgradeFire();
                else projectile.Boom();
                break;

            case eArrowState.Fire:
                if (_fireState == eFireState.Normal) IgniteSelf(); 
                else projectile.Boom(); 
                break;
        }
    }

    public void Ignite()
    {
        _fireEffect.SetActive(true);
        _fireState = eFireState.Fire;
    }

    // 본인을 점화시키는 함수
    public virtual void IgniteSelf()
    {
        Ignite();

        if (_roomMgr != null)
            _roomMgr.SolveQuest();
        if (_apply != null)
            _apply.EditData();
    }
}
