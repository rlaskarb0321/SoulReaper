using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTest : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _cam;

    [SerializeField]
    private float _magnifyCamValue;

    // Field
    private CinemachineFramingTransposer _vCamOption;
    private float _originCamDistance;
    private bool _isPlayerDead;
    private Animator _anim;

    private void Awake()
    {
        _vCamOption = _cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        _anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _originCamDistance = _vCamOption.m_CameraDistance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            _isPlayerDead = true;
        }

        if (_isPlayerDead)
        {
            AnnouncePlayerDeath();
        }
    }

    public void AnnouncePlayerDeath()
    {
        if (_vCamOption.m_CameraDistance > _magnifyCamValue)
        {
            MagnifyCam();
            return;
        }

        _anim.enabled = true;
        _isPlayerDead = false;
    }

    private void MagnifyCam()
    {
        if (_vCamOption.m_CameraDistance <= _magnifyCamValue)
            return;

        _vCamOption.m_CameraDistance -= Time.deltaTime * 2.5f;
    }
}
