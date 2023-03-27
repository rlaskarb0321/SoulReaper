using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private PlayerControlloer _playerController;
    private Animator _animator;
    private Rigidbody _rbody;
    private readonly int _hashMove = Animator.StringToHash("isMove");
    private readonly int _hashYVelocity = Animator.StringToHash("yVelocity");

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rbody = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerControlloer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Input.anyKey)
            _animator.SetBool(_hashMove, false);
        else
            _animator.SetBool(_hashMove, _playerController._h != Vector3.zero || _playerController._v != Vector3.zero);

        _animator.SetFloat(_hashYVelocity, _rbody.velocity.y);
    }
}
