using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack")]
    public float _attackAdvancedDist;
    public GameObject _weapon;
    public Transform _weaponCombatPos; // 공격할때 무기의 위치값
    public Transform _weaponNonCombatPos; // 공격상태가 아닐때 무기의 위치값

    private Transform _player;
    private Camera _cam;
    private Animator _animator;
    private PlayerAttackBehaviour _atkBehaviour;
    private int _combo;

    readonly int _hashCombo = Animator.StringToHash("AttackCombo");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _atkBehaviour = _animator.GetBehaviour<PlayerAttackBehaviour>();
        _cam = Camera.main;
        _player = transform;

        _combo = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 근거리 공격
            RotateToClickDir();
            _combo = _animator.GetInteger(_hashCombo);
            _animator.SetInteger(_hashCombo, ++_combo);
        }
        else if (Input.GetMouseButton(1))
        {
            // 원거리마법
            RotateToClickDir();
        }
    }

    void RotateToClickDir()
    {
        RaycastHit hit;
        Vector3 clickVector;
        Vector3 dir;

        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            clickVector = new Vector3(hit.point.x, _player.position.y, hit.point.z);
            dir = clickVector - _player.position;
            transform.forward = dir;
        }
    }

    // Attack 애니메이션 마지막에 달아놓는 Delegate
    // 콤보를 더 이어나갈지, 공격을 끝낼지 결정한다.
    public void SetComboInteger()
    {
        if (_atkBehaviour._isComboAtk)
        {
            if (++_combo > 2)
                _combo = 1;

            _animator.SetInteger(_hashCombo, _combo);
        }
        else
        {
            _animator.SetInteger(_hashCombo, 0);
        }
    }
}
