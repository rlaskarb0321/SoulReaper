using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public enum eCameraState { Follow, Patrol, Charging }
    public Transform _target;
    public float _range; // ī�޶� �ʹ� �ָ������ʵ��� ������ ����
    public float _speed; // ī�޶� ���� �� �ӵ�

    public eCameraState CamState { get { return _camState; } set { _camState = value; } }
    eCameraState _camState;
    Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
        _camState = eCameraState.Follow;
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            CamState = eCameraState.Patrol;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            CamState = eCameraState.Follow;

        switch (_camState)
        {
            case eCameraState.Follow:
                FollowPlayer();
                break;
            case eCameraState.Patrol:
                PatrolCamera(_cam, _target, _range, _speed);
                break;
            case eCameraState.Charging:
                PatrolCamera(_cam, _target, _range * 0.5f, _speed);
                break;
        }
    }

    // �������� ���콺�� �����ϸ� ���������� ���⸸ �ٲ�� �����ؾ���
    public void PatrolCamera(Camera cam, Transform target, float range, float speed)
    {
        Vector3 destination;
        RaycastHit hit;

        // ���콺�� �ִ°��� ��ü���ְ�, ������ �����ȿ������� ī�޶� ������ �������� ����
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            // �÷��̾�� ���콺���������� �Ÿ��� range�� ���迡���� destination�� �޶�����.
            if (Mathf.Abs((target.position - new Vector3(hit.point.x, target.position.y, hit.point.z)).magnitude) <= range)
                destination = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            else
            {
                Vector3 direction = (new Vector3(hit.point.x, transform.position.y, hit.point.z) - target.position).normalized;
                destination = direction * range;
                destination.y = transform.position.y;
            }

            // �������� ī�޶� �̵���Ŵ
            transform.position = Vector3.Lerp(transform.position, destination, speed * 1.05f);
        }
    }

    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position, _speed);
    }
}
