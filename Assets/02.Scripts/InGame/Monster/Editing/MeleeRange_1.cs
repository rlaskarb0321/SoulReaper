using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRange_1 : MonsterBase_1
{
    private void Update()
    {
        if (_target == null)
        {
            SearchTarget();
        }

        // Move();
    }

    public override void SearchTarget()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _stat.traceDist, 1 << LayerMask.NameToLayer("PlayerTeam")); 

        if (colls.Length >= 1)
        {
            if (_waveMonster != null)
            {
                _target = colls[0].gameObject;
                return;
            }

            Vector3 targetVector = colls[0].transform.position - _eyePos.position;
            float distance = targetVector.magnitude;
            Vector3 dir = new Vector3(targetVector.x, 0.0f, targetVector.z);
            RaycastHit hit;
            bool isHit = Physics.Raycast(_eyePos.position, dir, out hit, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("PlayerTeam"));

            if (!isHit)
                return;

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
            {
                _target = hit.transform.gameObject;
            }
        }
    }

    public override void Move(Vector3 pos, float movSpeed)
    {

    }
}
