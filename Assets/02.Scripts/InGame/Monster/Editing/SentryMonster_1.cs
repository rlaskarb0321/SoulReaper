using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SentryMonster_1 : MonoBehaviour
{
    public MonsterBase_1 _monsterBase;

    [SerializeField] private Vector3 _movPos;
    [SerializeField] private bool _isSetPatrolPos;

    private void Update()
    {
        if (_monsterBase._target == null)
        {
            _monsterBase.SearchTarget();
        }
        else
        {
            _movPos = _monsterBase._target.transform.position;
        }

        if (!_isSetPatrolPos)
        {
            _movPos = SetRandomPoint(transform.position, _movPos, _monsterBase._stat.traceDist * 0.5f);
            _isSetPatrolPos = true;
            return;
        }
    }

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
