using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // 인스턴스화 고려중

    public enum eCameraState { Follow, Patrol, Charging }
    public Transform _target;
    public float _range; 
    public float _speed;

    [Header("Shake")]
    public float _shakeAmount;
    public float _shakeDur;

    public eCameraState CamState { get { return _camState; } set { _camState = value; } }
    eCameraState _camState;
    Camera _cam;
    float _originShakeDur;

    void Start()
    {
        _cam = Camera.main;
        _camState = eCameraState.Follow;
        _originShakeDur = _shakeDur;
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

    public IEnumerator ShakingCamera()
    {
        Vector3 originCamPos = transform.localPosition;
        while (_shakeDur > 0.0f)
        {
            Vector3 randomPos = originCamPos + Random.insideUnitSphere * _shakeAmount;
            transform.localPosition = randomPos;
            _shakeDur -= Time.deltaTime;
            yield return null;
        }

        _shakeDur = _originShakeDur;
    }

    /// <summary>
    /// 카메라를 마우스위치로 옮겨주는 함수
    /// </summary>
    /// <param name="cam">옮길 카메라(main Camera)</param>
    /// <param name="player">카메라가 따라다닐 캐릭터</param>
    /// <param name="range">카메라가 마우스위치를 따라갈 수 있는 길이</param>
    /// <param name="speed">카메라가 옮겨지는 속도</param>
    void PatrolCamera(Camera cam, Transform player, float range, float speed)
    {
        Vector3 mousePos; // 마우스의 위치값 저장
        Vector3 dir; // player의 pos에서 mousePos로 향하는 벡터
        float dirLength;
        RaycastHit hit;

        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            mousePos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            dir = mousePos - new Vector3(player.position.x, transform.position.y, player.position.z);
            dirLength = dir.magnitude;

            if (dirLength < range)
            {
                // 허용범위안에 있다면 특별한거 없이 카메라를 마우스위치로 이동시킴

                Vector3 destination = player.position + mousePos;
                destination.y = transform.position.y;
                transform.position = Vector3.Lerp(transform.position, destination, speed * 1.05f);
            }
            else
            {
                // 허용범위밖에 있다면 플레이어~마우스위치로의 방향만받고 카메라를 범위내에서 움직이게 함

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
