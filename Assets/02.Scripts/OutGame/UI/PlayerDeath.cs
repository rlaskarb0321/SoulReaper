using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _cam;

    [SerializeField]
    private float _magnifyCamValue;

    // Field
    private CinemachineFramingTransposer _vCamOption;
    private bool _isPlayerDead;
    private Animator _anim;

    private void Awake()
    {
        _vCamOption = _cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isPlayerDead)
            StartDeathProgress();
    }

    public void AnnouncePlayerDeath() => _isPlayerDead = true; 

    // ��� �� ���� ����
    private void StartDeathProgress()
    {
        if (_vCamOption.m_CameraDistance > _magnifyCamValue)
        {
            MagnifyCam();
            return;
        }

        _anim.enabled = true;
        _isPlayerDead = false;
    }

    // ��� ���� �ִϸ��̼��� �������� ��������Ʈ�� �޾Ƴ��� �޼���
    public void LoadBeforeDeathData()
    {
        LoadingScene.LoadScene(CharacterDataPackage._characterData._characterData._mapName);
    }

    private void MagnifyCam()
    {
        if (_vCamOption.m_CameraDistance <= _magnifyCamValue)
            return;

        _vCamOption.m_CameraDistance -= Time.deltaTime * 2.0f;
    }
}
