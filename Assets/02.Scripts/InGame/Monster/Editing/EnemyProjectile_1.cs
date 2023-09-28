using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LaunchData
{
    public float damage;
    public float eleveationAngle;
    public float speed;
}

public class EnemyProjectile_1 : VFXPool
{
    public GameObject _explodeEffect;
    public Vector3 _launchDir;

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

    }

    public override void SetPoolData(LaunchData data)
    {

        Launch();
    }

    private void Launch()
    {
        _launchDir = new Vector3(0.0f, 0.0f, 0.0f);
        print("น฿ป็!!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            print("Player Trigger");
        }
    }
}
