using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform _target;
    public float _range; // 카메라가 너무 멀리가지않도록 지정할 범위
    public float _speed;

    Vector3 _destination;
    RaycastHit _hit;
    Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out _hit))
            {
                _destination = new Vector3(_hit.point.x, transform.position.y, _hit.point.z);
            }
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position = Vector3.Lerp(transform.position, _destination, _speed * 1.5f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _target.position, _speed);
        }
    }
}
