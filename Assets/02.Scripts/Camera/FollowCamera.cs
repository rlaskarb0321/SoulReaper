using System.Linq;
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
    public GameObject _raySearchTarget;
    public eCameraState CamState { get { return _camState; } set { _camState = value; } }
    eCameraState _camState;
    Camera _cam;
    Outline _playerOutline;

    private void Awake()
    {
        _playerOutline = _target.GetComponent<Outline>();
    }

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
        ControlTargetOutLine();

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

    public IEnumerator ShakingCamera(float shakeDur, float shakeAmount)
    {
        Vector3 originCamPos = transform.localPosition;
        while (shakeDur > 0.0f)
        {
            Vector3 randomPos = originCamPos + Random.insideUnitSphere * shakeAmount;
            transform.localPosition = randomPos;
            shakeDur -= Time.deltaTime;
            yield return null;
        }
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
        RaycastHit[] hits;
        RaycastHit hit;
        Ray ray;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray, float.MaxValue, 1 << LayerMask.NameToLayer("Ground"));
        hit = hits.Where(obj => obj.transform.tag != "Wall").FirstOrDefault();

        mousePos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        dir = mousePos - new Vector3(player.position.x, transform.position.y, player.position.z);
        dirLength = dir.magnitude;
        dir = dir.normalized;

        // 카메라를 해당방향쪽으로 이동시킴
        Vector3 destination = new Vector3(player.position.x + (dir.x * range)
            , transform.position.y
            , player.position.z + (dir.z * range));

        transform.position = Vector3.Lerp(transform.position, destination, speed * 1.05f);
        #region 23.05.19 물체의 모서리와 모서리 근처로 카메라를 보낼 때 위치값이 크게 차이나는 현상 수정
        //if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 1 << LayerMask.NameToLayer("Ground")))
        //{
        //    mousePos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        //    dir = mousePos - new Vector3(player.position.x, transform.position.y, player.position.z);
        //    dirLength = dir.magnitude;

        //    if (dirLength < range)
        //    {
        //        // 허용범위안에 있다면 특별한거 없이 카메라를 마우스위치로 이동시킴
        //        Vector3 destination = player.position + mousePos;
        //        destination.y = transform.position.y;
        //        transform.position = Vector3.Lerp(transform.position, destination, speed * 1.05f);
        //    }
        //    else
        //    {
        //        // 허용범위밖에 있다면 플레이어~마우스위치로의 방향만받고 카메라를 범위내에서 움직이게 함
        //        dir = dir.normalized;
        //        Vector3 destination = new Vector3(player.position.x + (dir.x * range)
        //            , transform.position.y
        //            , player.position.z + (dir.z * range));

        //        transform.position = Vector3.Lerp(transform.position, destination, speed * 1.05f);
        //    }
        //}
        #endregion 23.05.19 물체의 모서리와 모서리 근처로 카메라를 보낼 때 위치값이 크게 차이나는 현상 수정
    }

    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position, _speed);
    }

    void ControlTargetOutLine()
    {
        Ray silhouetteRay;
        RaycastHit[] silRayHits;
        RaycastHit hitObj;

        // silhouetteRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        silhouetteRay = new Ray(Camera.main.transform.position, (_raySearchTarget.transform.position - Camera.main.transform.position).normalized);
        silRayHits = Physics.SphereCastAll(silhouetteRay, 1.8f, byte.MaxValue);

        hitObj = silRayHits.Last();
        hitObj = silRayHits.OrderBy(distance => (Camera.main.transform.position - distance.transform.position).magnitude).First();

        if (hitObj.transform.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            _playerOutline.enabled = false;
        }
        else
        {
            _playerOutline.enabled = true;
        }
    }
}
