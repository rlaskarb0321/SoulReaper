using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalMonster : Monster
{
    [Header("Normal Level Monster")]
    public int _numOfAttacks;
    public GameObject _hitBoxCollObj;

    float _rotSpeed;
    Animator _animator;
    MonsterAI _monsterAI;
    WaitForSeconds _ws;

    readonly int _hashDoAttack = Animator.StringToHash("DoAttack");

    void Awake()
    {
        _monsterAI = GetComponent<MonsterAI>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _ws = new WaitForSeconds(_monsterAI._nextActDelay);
        _rotSpeed = _monsterAI._navAgent.angularSpeed;
    }

    void Update()
    {
        if (_state == Monster.eMonsterState.Dead)
            return;

        
    }

    public override IEnumerator DoAttack()
    {
        if (_state == eMonsterState.Dead)
            yield break;

        RigidbodyConstraints originConstraints = _monsterAI._rbody.constraints;

        _monsterAI._navAgent.enabled = false;
        _monsterAI._rbody.constraints = RigidbodyConstraints.FreezePositionY | originConstraints;
        _state = eMonsterState.Acting;
        yield return StartCoroutine(LookTarget());

        _animator.SetTrigger(_hashDoAttack);
        yield return _ws;

        _state = eMonsterState.Trace;
        _monsterAI._navAgent.enabled = true;
        _monsterAI._rbody.constraints = originConstraints;
    }

    // ���ݹ����ȿ��� �÷��̾ ���� �ٶ�
    public override IEnumerator LookTarget()
    {
        Vector3 myPos = transform.position;
        Vector3 targetPos = _monsterAI._target.position;
        float angle;

        targetPos.y = 0.0f;
        myPos.y = 0.0f;
        angle = Quaternion.FromToRotation(transform.forward, targetPos - myPos).eulerAngles.y;

        // ������ transform.forward �������� �÷��̾ ���ʰ����������� �������� �����ϱ� ����
        if (angle - 180.0f > 0.0f)
            angle = -(360 - angle);

        while (Mathf.Abs(Quaternion.FromToRotation(transform.forward, targetPos - myPos).eulerAngles.y) >= 1.0f)
        {
            transform.eulerAngles = new Vector3(0.0f, 
                Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + angle, 2.0f * Time.deltaTime), 0.0f);
            yield return null;
        }
    }
}
