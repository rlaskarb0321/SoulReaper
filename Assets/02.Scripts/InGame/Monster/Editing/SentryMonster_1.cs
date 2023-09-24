using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SentryMonster_1 : MonoBehaviour
{
    public Vector3 SetRandomPoint(Vector3 center, Vector3 destination, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas))
            {
                destination = hit.position;
                return destination;
            }
        }

        return Vector3.zero;
    }
}
