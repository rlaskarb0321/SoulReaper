using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapTeleport : MonoBehaviour
{
    [Header("=== From ===")]
    [SerializeField] private GameObject _enterPos;
    [SerializeField] private GameObject _exitPos;

    [Header("=== To ===")]
    [SerializeField] private GameObject _nextPos;

    [Header("=== Target ===")]
    [SerializeField] private GameObject _playerBody;
    [SerializeField] private GameObject _cameraArm;
    [SerializeField] private CinemachineVirtualCamera _cam;
    private Vector3 _originDamp;

    [Header("=== Fade Panel ===")]
    [SerializeField] private GameObject _fadePanel;
    private CinemachineFramingTransposer _vcamOption;

    private PlayerMove_1 _playerMove;

    private void Awake()
    {
        //_vcamOption = _cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        //_originDamp = new Vector3(_vcamOption.m_XDamping, _vcamOption.m_YDamping, _vcamOption.m_ZDamping);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            if (_playerMove == null)
                _playerMove = other.GetComponent<PlayerMove_1>();
            if (_playerMove._state.State == PlayerFSM.eState.Ladder)
            {
                _playerMove.ClimbDown();
            }

            _playerMove.TeleportPlayer(_nextPos.transform);

            //_vcamOption.m_XDamping = 0.0f;
            //_vcamOption.m_YDamping = 0.0f;
            //_vcamOption.m_ZDamping = 0.0f;

            //_fadePanel.SetActive(false);
            //_fadePanel.SetActive(true);
            //_playerBody.transform.position = _nextPos.transform.position;
            //_cameraArm.transform.position = _playerBody.transform.position;

            //StartCoroutine(RestoreCamDampValue());
        }
    }

    private IEnumerator RestoreCamDampValue()
    {
        yield return new WaitForSeconds(0.2f);

        _vcamOption.m_XDamping = _originDamp.x;
        _vcamOption.m_YDamping = _originDamp.y;
        _vcamOption.m_ZDamping = _originDamp.z;
    }
}
