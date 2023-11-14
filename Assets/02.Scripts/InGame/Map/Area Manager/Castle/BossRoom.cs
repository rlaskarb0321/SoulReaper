using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : QuestRoom
{
    [Header("=== Boss ===")]
    [SerializeField]
    private GameObject _boss;

    public override void SolveQuest()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
        {
            return;
        }

        _boss.SetActive(true);
    }
}
