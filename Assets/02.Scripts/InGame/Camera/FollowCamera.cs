using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FollowCamera : MonoBehaviour
{
    public static FollowCamera _instance;

    public enum eCameraState { Follow, Patrol, Charging }
    public eCameraState CamState { get { return _camState; } set { _camState = value; } }

    [Header("=== Cam ===")]
    public Transform _target;
    public Transform _player;
    public CinemachineVirtualCamera _vCam;
    public float _shakeTimer;
    public float _range; 
    public float _speed;
    [SerializeField] private eCameraState _camState;

    [Header("=== OutLine ===")]
    public GameObject _raySearchTarget;
    public Outline _playerOutline;

    [SerializeField] private CinemachineBasicMultiChannelPerlin _perlin;

    private void Awake()
    {
        _instance = this;

        _perlin = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Start()
    {
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
                PatrolCamera(_target, _range, _speed);
                break;
            case eCameraState.Charging:
                PatrolCamera(_target, _range * 0.5f, _speed * 1.2f);
                break;
        }
    }

    public IEnumerator ShakeCamera(float intensity, float timer)
    {
        _perlin.m_AmplitudeGain = intensity;
        _shakeTimer = timer;

        while (_shakeTimer > 0.0f)
        {
            _shakeTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _perlin.m_AmplitudeGain = 0.0f;
        _shakeTimer = 0.0f;
    }

    /// <summary>
    /// 카메라를 마우스위치로 옮겨주는 함수
    /// </summary>
    /// <param name="cam">옮길 카메라(main Camera)</param>
    /// <param name="player">카메라가 따라다닐 캐릭터</param>
    /// <param name="range">카메라가 마우스위치를 따라갈 수 있는 길이</param>
    /// <param name="speed">카메라가 옮겨지는 속도</param>
    void PatrolCamera(Transform lookTarget, float range, float speed)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2); // 스크린 중앙 좌표
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y); // 마우스 입력 좌표
        Vector3 dir = (mousePos - screenCenter).normalized;
        Vector3 destination 
            = new Vector3(_player.position.x + (dir.x * range), lookTarget.transform.position.y, _player.position.z + (dir.y * range));

        lookTarget.transform.position = Vector3.Lerp(lookTarget.transform.position, destination, speed * 1.05f);
    }

    void FollowPlayer()
    {
        _target.position = Vector3.Lerp(_target.position, _player.position, 0.1f);
    }

    void ControlTargetOutLine()
    {
        Ray silhouetteRay;
        RaycastHit[] silRayHits;
        RaycastHit hitObj;

        silhouetteRay = new Ray(_vCam.transform.position, (_raySearchTarget.transform.position - _vCam.transform.position).normalized);
        silRayHits = Physics.RaycastAll(silhouetteRay, (_raySearchTarget.transform.position - _vCam.transform.position).magnitude);
        silRayHits.OrderBy(hit => (_vCam.transform.position - hit.transform.position).magnitude);
        hitObj = silRayHits[0];

        if (hitObj.transform.gameObject.layer == LayerMask.NameToLayer("PlayerTeam") ||
            hitObj.transform.gameObject.layer == LayerMask.NameToLayer("Ignore OutLine"))
        {
            _playerOutline.enabled = false;
        }
        else
        {
            _playerOutline.enabled = true;
        }
    }
}
