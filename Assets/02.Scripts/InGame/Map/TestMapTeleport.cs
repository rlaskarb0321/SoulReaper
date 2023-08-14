using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMapTeleport : MonoBehaviour
{
    [Header("=== From ===")]
    [SerializeField] private GameObject _enterPos;
    [SerializeField] private GameObject _exitPos;

    [Header("=== To ===")]
    [SerializeField] private GameObject _nextPos;

    [Header("=== Target ===")]
    [SerializeField] private GameObject _playerBody;
    [SerializeField] private GameObject _cameraArm;

    [Header("=== Fade Panel ===")]
    [SerializeField] private GameObject _fadePanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            _fadePanel.SetActive(false);
            _fadePanel.SetActive(true);

            _playerBody.transform.position = _nextPos.transform.position;
            _cameraArm.transform.position = _playerBody.transform.position;
        }
    }
}
