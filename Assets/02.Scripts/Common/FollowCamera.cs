using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public enum eCameraState { Follow, Patrol, Charging }
    public Transform _target;
    public float _range; 
    public float _speed; 

    public eCameraState CamState { get { return _camState; } set { _camState = value; } }
    eCameraState _camState;
    Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        _camState = eCameraState.Follow;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            CamState = eCameraState.Patrol;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            CamState = eCameraState.Follow;
    }

    void FixedUpdate()
    {
        switch (_camState)
        {
            case eCameraState.Follow:
                FollowPlayer();
                break;
            case eCameraState.Patrol:
                PatrolCamera(_cam, _target, _range, _speed);
                break;
            case eCameraState.Charging:
                PatrolCamera(_cam, _target, _range * 0.5f, _speed * 1.2f);
                break;
        }
    }

    /// <summary>
    /// ī�޶� ���콺��ġ�� �Ű��ִ� �Լ�
    /// </summary>
    /// <param name="cam">�ű� ī�޶�(main Camera)</param>
    /// <param name="player">ī�޶� ����ٴ� ĳ����</param>
    /// <param name="range">ī�޶� ���콺��ġ�� ���� �� �ִ� ����</param>
    /// <param name="speed">ī�޶� �Ű����� �ӵ�</param>
    public void PatrolCamera(Camera cam, Transform player, float range, float speed)
    {
        Vector3 mousePos; // ���콺�� ��ġ�� ����
        Vector3 dir; // player�� pos���� mousePos�� ���ϴ� ����
        float dirLength;
        RaycastHit hit;

        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            mousePos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            dir = mousePos - new Vector3(player.position.x, transform.position.y, player.position.z);
            dirLength = dir.magnitude;

            if (dirLength < range)
            {
                // �������ȿ� �ִٸ� Ư���Ѱ� ���� ī�޶� ���콺��ġ�� �̵���Ŵ

                Vector3 destination = player.position + mousePos;
                destination.y = transform.position.y;
                transform.position = Vector3.Lerp(transform.position, destination, speed * 1.05f);
            }
            else
            {
                // �������ۿ� �ִٸ� �÷��̾�~���콺��ġ���� ���⸸�ް� ī�޶� ���������� �����̰� ��

                dir = dir.normalized;
                Vector3 destination = new Vector3(player.position.x + (dir.x * range)
                    , transform.position.y
                    , player.position.z + (dir.z * range));

                transform.position = Vector3.Lerp(transform.position, destination, speed * 1.05f);
            }
        }
    }

    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position, _speed);
    }
}
