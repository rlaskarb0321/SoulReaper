using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public enum eCameraState { Follow, Patrol, Charging }
    public Transform _target;
    public float _range; // 카메라가 너무 멀리가지않도록 지정할 범위
    public float _speed; // 카메라가 따라갈 때 속도

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

    // 범위밖을 마우스가 조준하면 범위내에서 방향만 바뀌도록 수정해야함
    public void PatrolCamera(Camera cam, Transform target, float range, float speed)
    {
        Vector3 destination;
        RaycastHit hit;

        // 마우스가 있는곳에 물체가있고, 지정된 범위안에있으면 카메라가 정찰할 목적지로 설정
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            // 플레이어와 마우스찍은지점의 거리와 range의 관계에따라 destination이 달라진다.
            if (Mathf.Abs((target.position - new Vector3(hit.point.x, target.position.y, hit.point.z)).magnitude) <= range)
                destination = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            else
            {
                Vector3 direction = (new Vector3(hit.point.x, transform.position.y, hit.point.z) - target.position).normalized;
                destination = direction * range;
                destination.y = transform.position.y;
            }

            // 정찰지로 카메라를 이동시킴
            transform.position = Vector3.Lerp(transform.position, destination, speed * 1.05f);
        }
    }

    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position, _speed);
    }
}
