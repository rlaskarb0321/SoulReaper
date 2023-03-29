using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform _target;
    public float _range; // ī�޶� �ʹ� �ָ������ʵ��� ������ ����
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
            // ī�޶� �÷��̾� ����ٴϴ� ��
            FollowPlayer();
        }
    }

    void PatrolCamera()
    {
        // ���콺�� �ִ°��� ��ü���ְ�, ������ �����ȿ������� ī�޶� ������ �������� ����
        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out _hit) &&
            Mathf.Abs((_target.position - new Vector3(_hit.point.x, _target.position.y, _hit.point.z)).magnitude) <= _range)
            _destination = new Vector3(_hit.point.x, transform.position.y, _hit.point.z);

        // �������� ī�޶� �̵���Ŵ
        transform.position = Vector3.Lerp(transform.position, _destination, _speed * 1.05f);
    }

    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position, _speed);
    }
}
