using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : QuestRoom
{
    [Header("=== Boss ===")]
    [SerializeField]
    private GameObject _boss;

    public bool _isDevelopMode;

    public override void SolveQuest()
    {
        // 보스 죽였을 때 호출, 게임의 엔딩
        print("hi");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 보스전 시작 연출을 보여준다, 개발자모드에선 보여주지않고 바로

        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
        {
            return;
        }

        _boss.SetActive(true);
    }
}
