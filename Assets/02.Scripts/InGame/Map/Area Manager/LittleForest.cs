using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleForest : MonoBehaviour
{
    [Header("=== Fall Death ===")]
    [SerializeField] private PlayerFSM _player;
    [SerializeField] private GameObject _camLookAt;
    [SerializeField] private GameObject _fadePanel;
    [Range(1.0f, 10.0f)] [SerializeField] private float _recordTime;
    [SerializeField] private Vector3 _reversPos;

    private float _recordTimeOrigin;
    private bool _isFallDeath;

    private void Start()
    {
        _recordTimeOrigin = _recordTime;
        _reversPos = _player.transform.position;
    }

    private void Update()
    {
        CheckFallDeath();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
            return;
        if (_player == null)
            _player = other.GetComponent<PlayerFSM>();

        _player.transform.position = _reversPos;
        _isFallDeath = true;
    }

    private void CheckFallDeath()
    {
        if (_player.State == PlayerFSM.eState.Fall)
        {
            _recordTime = _recordTimeOrigin;
            return;
        }

        if (_isFallDeath || _recordTime < 0.0f)
        {
            _reversPos = _player.transform.position;
            _recordTime = _recordTimeOrigin;
            _isFallDeath = false;
        }

        _recordTime -= Time.deltaTime;
    }
}
