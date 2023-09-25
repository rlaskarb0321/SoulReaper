 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase_1 : MonoBehaviour
{
    public enum eMonsterState 
    { 
        Idle,   // ���� �� �������� ��ų��
        Move,
        Attack,
        Hit,
        Delay,  // ���� �� ������ �ð��� ������
        Dead,
    }

    public enum eMonsterType { Wave, Patrol, }

    [Header("=== Stat ===")]
    public MonsterStat _stat;
    public float _currHp;

    [Header("=== FSM ===")]
    public eMonsterState _state;

    [Header("=== Target ===")]
    public Transform _eyePos;
    public GameObject _target;

    [Header("=== Hit & Dead ===")]
    public Material[] _hitMats; // 0�� �ε����� �⺻ mat, 1�� �ε����� �ǰݽ� ���ٲ� mat
    public float _bodyBuryTime; // ��üó�������� ���۱��� ��ٸ� ��
    public Material _deadMat;

    [Header("=== Monster Type ===")]
    public WaveMonster _waveMonster;
    public SentryMonster_1 _sentryMonster;

    private NavMeshAgent _nav;
    private SkinnedMeshRenderer _mesh;

    protected void Awake()
    {
        if (_waveMonster == null && _sentryMonster == null)
        {
            Debug.LogError(gameObject.name + "������Ʈ�� �ʺ� �Ǵ� ���̺� �� ���� ������ �������� ����");
            return;
        }

        if ((_waveMonster != null && _sentryMonster != null))
        {
            Debug.LogError(gameObject.name + "������Ʈ�� �ʺ� �Ǵ� ���̺� �� ���� ������ �ϳ��� �����ؾ� ��");
            return;
        }

        if (_waveMonster != null)
        {
            _stat.traceDist = 150.0f;
        }

        _nav = GetComponent<NavMeshAgent>();
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    protected void Start()
    {
        _currHp = _stat.health;
    }

    public virtual void SearchTarget()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _stat.traceDist, 1 << LayerMask.NameToLayer("PlayerTeam"));

        if (colls.Length >= 1)
        {
            // ���� �̺�Ʈ�� ������ ���͸� �ٷ� Ÿ�� ����
            if (_waveMonster != null)
            {
                _target = colls[0].gameObject;
                return;
            }

            // ���� �̺�Ʈ ���Ͱ� �ƴѰ��, �ڽŰ� ����� ���̿��ִ���, Ÿ���� �繰�� �������ִ��� ���� �Ǵ� �� Ÿ�� ����
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

    /// <summary>
    /// ��ǥ ��ġ�� movSpeed ���� ���� �ӵ��� �̵���
    /// </summary>
    /// <param name="pos">�̵� ��ǥ ��ġ</param>
    /// <param name="movSpeed">��ǥ�� ���ϴ� �̵� �ӵ�</param>
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

    /// <summary>
    /// ������ currHp �� amount ��ŭ ����
    /// </summary>
    /// <param name="amount">hp�� ���� ��</param>
    public virtual void DecreaseHP(float amount)
    {
        StartCoroutine(OnHitEvent());

        _currHp -= amount;
        if (_currHp <= 0.0f)
        {
            _currHp = 0.0f;
            Dead();
            if (_waveMonster != null)
            {
                _waveMonster.AlertDead();
            }
        }
    }

    /// <summary>
    /// ���Ͱ� �ǰݴ����� ��, �ǰ� ����� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator OnHitEvent()
    {
        Material newMat;

        newMat = _hitMats[1];
        _mesh.material = newMat;
        yield return new WaitForSeconds(Time.deltaTime * 4.0f);

        newMat = _hitMats[0];
        _mesh.material = newMat;
    }

    /// <summary>
    /// ���Ͱ� ���� �ǰ��� currHp = 0 �� �Ǿ��� �� ȣ��� �Լ�
    /// </summary>
    public virtual void Dead()
    {
        GetComponent<BoxCollider>().enabled = false;

        _nav.velocity = Vector3.zero;
        _nav.isStopped = true;
        _nav.baseOffset = 0.0f;

        StartCoroutine(OnMonsterDead());
    }

    /// <summary>
    /// ������ ��� �� ������ ���� �Լ�
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator OnMonsterDead()
    {
        yield return new WaitForSeconds(_bodyBuryTime);

        Material newMat = Instantiate(_deadMat);
        Color color = newMat.color;

        while (newMat.color.a >= 0.05f)
        {
            color.a -= Time.deltaTime;
            newMat.color = color;
            _mesh.material = newMat;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}