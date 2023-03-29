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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            PatrolCamera();
        }
        else
        {
            // 카메라가 플레이어 따라다니는 중
            FollowPlayer();
        }
    }

    void PatrolCamera()
    {
        // 마우스가 있는곳에 물체가있고, 지정된 범위안에있으면 카메라가 정찰할 목적지로 설정
        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out _hit) &&
            Mathf.Abs((_target.position - new Vector3(_hit.point.x, _target.position.y, _hit.point.z)).magnitude) <= _range)
            _destination = new Vector3(_hit.point.x, transform.position.y, _hit.point.z);

        // 정찰지로 카메라를 이동시킴
        transform.position = Vector3.Lerp(transform.position, _destination, _speed * 1.05f);
    }

    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position, _speed);
    }
}
