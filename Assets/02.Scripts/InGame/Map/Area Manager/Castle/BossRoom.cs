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
        // ���� �׿��� �� ȣ��, ������ ����
        print("hi");
    }

    private void OnTriggerEnter(Collider other)
    {
        // ������ ���� ������ �����ش�, �����ڸ�忡�� ���������ʰ� �ٷ�

        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
        {
            return;
        }

        _boss.SetActive(true);
    }
}
