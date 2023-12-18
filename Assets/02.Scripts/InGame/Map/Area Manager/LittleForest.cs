using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LittleForest : MonoBehaviour
{
    private BuffProvider _buffProvider;

    private void Awake()
    {
        _buffProvider = GetComponent<BuffProvider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
            return;

        BackToReversePos(other);
        DebuffPlayer();
    }

    private void BackToReversePos(Collider other)
    {
        IMapOut mapOutObject = other.GetComponent<IMapOut>();
        if (mapOutObject == null)
            return;

        Collider[] reversePoses = null;
        int searchCount = 1;
        while (true)
        {
            float searchRadius = 100.0f * searchCount;
            reversePoses = Physics.OverlapSphere(other.transform.position, searchRadius, 1 << LayerMask.NameToLayer("ReversePos"));
            if (reversePoses.Length < 1)
            {
                searchCount++;
                continue;
            }

            reversePoses = reversePoses.OrderBy(item => Vector3.Distance(item.transform.position, other.transform.position)).ToArray();
            mapOutObject.RestorePos(reversePoses[0].transform.position);
            break;
        }
    }

    private void DebuffPlayer()
    {
        PlayerBuff buff = _buffProvider.GenerateBuffInstance();
        UIScene._instance._buffMgr.BuffPlayer(buff);
    }
}
