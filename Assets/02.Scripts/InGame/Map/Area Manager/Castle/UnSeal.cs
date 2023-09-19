using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class UnSeal : MonoBehaviour
{
    [SerializeField] private PlayableDirector _playableDirector;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
            return;

        ProductionMgr.StartProduction(_playableDirector);
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }
}
