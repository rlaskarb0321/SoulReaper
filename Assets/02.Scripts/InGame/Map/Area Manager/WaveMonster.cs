using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMonster : MonoBehaviour
{
    public RaidWave _waveMaster;

    // �ش� ���Ͱ� ���Ե� ���̺� �����ϴ� Obj���� ������ �׾����� �˸�
    public void AlertDead()
    {
        _waveMaster.DecreaseMonsterCount();
    }
}
