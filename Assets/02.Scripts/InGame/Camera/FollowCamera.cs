using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
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
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2); // 스크린 중앙 좌표
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y); // 마우스 입력 좌표
        Vector3 dir = (mousePos - screenCenter).normalized;
        Vector3 destination = new Vector3(player.position.x + (dir.x * range), transform.position.y, player.position.z + (dir.y * range));

        transform.position = Vector3.Lerp(transform.position, destination, speed * 1.05f);
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

        silhouetteRay = new Ray(Camera.main.transform.position, (_raySearchTarget.transform.position - Camera.main.transform.position).normalized);
        //silRayHits = Physics.SphereCastAll(silhouetteRay, 0.26f, byte.MaxValue);
        //Debug.DrawRay(Camera.main.transform.position, (_raySearchTarget.transform.position - Camera.main.transform.position), Color.red);
        silRayHits = Physics.RaycastAll(silhouetteRay, (_raySearchTarget.transform.position - Camera.main.transform.position).magnitude);
        hitObj = silRayHits.OrderBy(hit => (Camera.main.transform.position - hit.transform.position).magnitude).First();

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
