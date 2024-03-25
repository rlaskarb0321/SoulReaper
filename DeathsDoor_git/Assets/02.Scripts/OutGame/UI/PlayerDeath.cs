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

    [SerializeField]
    private SkinnedMeshRenderer _playerMesh;

    [SerializeField]
    private Material _playerFadeMat;

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

    // ��� �� ���� ����
    private void StartDeathProgress()
    {
        if (_vCamOption.m_CameraDistance > _magnifyCamValue)
        {
            MagnifyCam();
            return;
        }

        StartCoroutine(FadeOutPlayerMat());
        _anim.enabled = true;
        _isPlayerDead = false;
    }

    // ��� ���� �ִϸ��̼��� �������� ��������Ʈ�� �޾Ƴ��� �޼���
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

    private IEnumerator FadeOutPlayerMat()
    {
        Material fadeMat = Instantiate(_playerFadeMat);
        Color color = fadeMat.color;

        while (fadeMat.color.a >= 0.05f)
        {
            color.a -= Time.deltaTime * 0.5f;
            fadeMat.color = color;
            _playerMesh.material = fadeMat;
            yield return null;
        }
    }
}
