using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlloer : MonoBehaviour
{
    public float _movSpeed;
    public float _rotSpeed;
    public float _dodgeSpeed;

    private Rigidbody _rbody;
    private Vector3 _dir;
    [HideInInspector] public Vector3 _h;
    [HideInInspector] public Vector3 _v;

    private void Start()
    {
        _rbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Input.anyKey)
        {
            MovePlayer();
            RotatePlayer();
        }
        else
        {
            _h = Vector3.zero;
            _v = Vector3.zero;
            _dir = Vector3.zero;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            // Dodge();
        }
    }


    void MovePlayer()
    {
        _h = Input.GetAxisRaw("Horizontal") * Vector3.right * Time.deltaTime;
        _v = Input.GetAxisRaw("Vertical") * Vector3.forward * Time.deltaTime;
        _dir = (_v + _h).normalized;

        transform.position += _dir * _movSpeed * Time.deltaTime;
    }

    void RotatePlayer()
    {
        if (_dir == Vector3.zero)
            return;

        Quaternion newRot = Quaternion.LookRotation(_dir);
        _rbody.rotation = Quaternion.Slerp(_rbody.rotation, newRot, _rotSpeed * Time.deltaTime);
    }

    void Dodge()
    {
        transform.position += transform.forward * _dodgeSpeed;
    }
}
