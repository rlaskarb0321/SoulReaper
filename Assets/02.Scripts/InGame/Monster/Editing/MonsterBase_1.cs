 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase_1 : MonoBehaviour
{
    public enum eMonsterState { Idle, Move, Attack, Hit, Delay, Dead, }
    public enum eMonsterType { Wave, Patrol, }

    [Header("=== Stat ===")]
    public MonsterStat _stat;

    [Header("=== FSM ===")]
    public eMonsterState _state;

    [Header("=== Target ===")]
    public Transform _eyePos;
    public GameObject _target;

    [Header("=== Hit & Dead ===")]
    public Material[] _hitMats; // 0번 인덱스는 기본 mat, 1번 인덱스는 피격시 잠깐바뀔 mat
    public float _bodyBuryTime; // 시체처리연출의 시작까지 기다릴 값
    public Material _deadMat;

    [Header("=== Monster Type ===")]
    public WaveMonster _waveMonster;
    public SentryMonster_1 _sentryMonster;

    private NavMeshAgent _nav;

    protected void Awake()
    {
        if (_waveMonster == null && _sentryMonster == null)
        {
            Debug.LogError(gameObject.name + "오브젝트의 초병 또는 웨이브 등 몬스터 형식을 지정하지 않음");
            return;
        }

        if ((_waveMonster != null && _sentryMonster != null))
        {
            Debug.LogError(gameObject.name + "오브젝트의 초병 또는 웨이브 등 몬스터 형식은 하나만 지정해야 함");
            return;
        }

        if (_waveMonster != null)
        {
            _stat.traceDist = 150.0f;
        }

        _nav = GetComponent<NavMeshAgent>();
    }

    public virtual void SearchTarget()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _stat.traceDist, 1 << LayerMask.NameToLayer("PlayerTeam"));

        if (colls.Length >= 1)
        {
            // 습격 이벤트에 나오는 몬스터면 바로 타겟 지정
            if (_waveMonster != null)
            {
                _target = colls[0].gameObject;
                return;
            }

            // 습격 이벤트 몬스터가 아닌경우, 자신과 비슷한 높이에있는지, 타겟이 사물에 가려져있는지 여부 판단 후 타겟 지정
            Vector3 targetVector = colls[0].transform.position - _eyePos.position;
            float distance = targetVector.magnitude;
            Vector3 dir = new Vector3(targetVector.x, 0.0f, targetVector.z);
            RaycastHit hit;
            bool isHit = Physics.Raycast(_eyePos.position, dir, out hit, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("PlayerTeam"));

            if (!isHit)
            {
                // print("nothing");
                return;
            }

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
            {
                // print("player");
                _target = hit.transform.gameObject;
            }
        }
    }

    public virtual void Move(Vector3 pos, float movSpeed)
    {
        if (_nav.pathPending)
        {
            return;
        }

        _nav.isStopped = false;
        _nav.speed = movSpeed;
        _nav.SetDestination(pos);
    }

    public virtual void DecreaseHP(float amount) { }
    public virtual IEnumerator OnHitEvent() { yield return null; }
    public virtual void Dead() { }
    public virtual IEnumerator OnMonsterDead() { yield return null; }
}