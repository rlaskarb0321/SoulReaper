using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float _attackAdvancedDist;

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
