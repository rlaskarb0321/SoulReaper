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

    public void AnnouncePlayerDeath()
    {
        CharacterData.CData data = CharacterDataPackage._cDataInstance._characterData;

        _isPlayerDead = true;
        data._isPlayerDead = _isPlayerDead;
        CharacterDataPackage._cDataInstance._characterData = data;
        DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");
    }

    // 사망 후 연출 시작
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

    // 사망 연출 애니메이션의 마지막에 델리게이트로 달아놓을 메서드
    public void LoadBeforeDeathData()
    {
        LoadingScene.LoadScene(CharacterDataPackage._cDataInstance._characterData._mapName);
    }

    private void MagnifyCam()
    {
        if (_vCamOption.m_CameraDistance <= _magnifyCamValue)
            return;

        _vCamOption.m_CameraDistance -= Time.deltaTime * 2.0f;
    }
}
