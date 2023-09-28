using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LaunchData
{
    public Vector3 launchAngle;
    public Vector3 position;
    public float damage;
    public float speed;
}

public class EnemyProjectile_1 : VFXPool
{
    public GameObject _explodeEffect;
    public LaunchData _launchData;

    private bool _isLaunch;
    private Rigidbody _rbody;
    private Vector3 _originPos;
    private Vector3 _originScale;

    private void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _originPos = transform.localPosition;
        _originScale = transform.localScale;
    }

    private void Update()
    {
        if (!_isLaunch)
            return;

        _rbody.MovePosition(_rbody.position + transform.forward * _launchData.speed * Time.fixedDeltaTime);
    }

    public override void SetPoolData(LaunchData data)
    {
        _launchData = data;
        transform.forward = data.launchAngle;
        _rbody.position = data.position;

        _isLaunch = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            print("Player Trigger");
        }
    }
}
