using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bull : MonsterBase
{
    [Header("=== Miniboss Bull ===")]
    public Transform _target;
    public float _attackRange;

    private readonly int _hashIsDead = Animator.StringToHash("isDead");

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        //SearchTarget();
    }

    public override void Attack()
    {

    }

    public override void DecreaseHp(float amount, Vector3 hitPos)
    {
        StartCoroutine(OnHitEvent());

        _currHp -= amount;
        if (_currHp <= 0.0f)
        {
            _currHp = 0.0f;
            Dead();
        }
    }

    public override void Idle()
    {

    }

    public override void Move(Vector3 pos, float movSpeed)
    {

    }

    public override IEnumerator OnHitEvent()
    {
        Material newMat;

        newMat = _hitMats[1];
        _mesh.material = newMat;
        yield return new WaitForSeconds(Time.deltaTime * 3.0f);

        newMat = _hitMats[0];
        _mesh.material = newMat;
    }

    protected override void Dead()
    {
        _animator.SetBool(_hashIsDead, true);
        StartCoroutine(OnMonsterDie());
    }

    protected override IEnumerator OnMonsterDie()
    {
        yield return new WaitForSeconds(_bodyBuryTime);

        Material newMat = _mesh.material;
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

    private void SearchTarget()
    {
        Vector3 targetPos = new Vector3(_target.position.x, transform.position.y, _target.position.z);
        Vector3 myPos = transform.position;
        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= _attackRange * 2.0f)
        {
            print("플레이어 탐지");
        }
    }
}
