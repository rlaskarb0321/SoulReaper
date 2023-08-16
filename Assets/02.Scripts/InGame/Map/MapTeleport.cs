using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("=== Fade Panel ===")]
    [SerializeField] private GameObject _fadePanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            Vector3 contactPoint = other.ClosestPoint(_enterPos.transform.localPosition);
            Vector3 contactLocalPos = _enterPos.transform.InverseTransformPoint(contactPoint);
            contactLocalPos.y = 0.0f;

            _fadePanel.SetActive(false);
            _fadePanel.SetActive(true);
            _playerBody.transform.position = _nextPos.transform.position + contactLocalPos;
            _cameraArm.transform.position = _playerBody.transform.position;
        }
    }
}
