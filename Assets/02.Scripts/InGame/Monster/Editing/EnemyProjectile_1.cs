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

    public LaunchData(Vector3 launchAngle, Vector3 position, float damage, float speed)
    {
        this.launchAngle = launchAngle;
        this.position = position;
        this.damage = damage;
        this.speed = speed;
    }
}

public class EnemyProjectile_1 : VFXPool
{
    public ParticleSystem _explodeEffect;
    public GameObject _missileObj;
    [HideInInspector] public LaunchData _launchData;

    private bool _isLaunch;
    private SphereCollider _coll;
    private Rigidbody _rbody;
    private Vector3 _originPos;
    private Vector3 _originScale;

    private void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _coll = GetComponent<SphereCollider>();
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

    private IEnumerator Explosion()
    {
        // Data Off
        _isLaunch = false;
        _missileObj.SetActive(false);
        _explodeEffect.transform.position = transform.position;
        _explodeEffect.gameObject.SetActive(true);
        _coll.enabled = false;

        yield return new WaitForSeconds(_explodeEffect.main.duration);

        gameObject.SetActive(false);
    }

    public override void SetPoolData(LaunchData data)
    {
        _launchData = data;
        transform.forward = data.launchAngle;
        transform.position = data.position;

        _coll.enabled = true;
        _missileObj.SetActive(true);
        _explodeEffect.gameObject.SetActive(false);
        _isLaunch = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            PlayerStat player = other.GetComponent<PlayerStat>();
            player.DecreaseHP(transform.forward, _launchData.damage);
            StartCoroutine(Explosion());
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StartCoroutine(Explosion());
        }
    }
}
