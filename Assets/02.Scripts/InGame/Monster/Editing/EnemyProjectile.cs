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

public class EnemyProjectile : VFXPool
{
    public ParticleSystem _explodeEffect;
    public GameObject _missileObj;
    public float _lifeTime;
    public bool _isReflected;
    [HideInInspector] public LaunchData _launchData;

    private bool _isLaunch;
    private float _originLifeTime;
    private SphereCollider _coll;
    private Rigidbody _rbody;

    private void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _coll = GetComponent<SphereCollider>();

        _originLifeTime = _lifeTime;
    }

    private void Update()
    {
        if (_lifeTime <= 0.0f)
        {
            StartCoroutine(Explosion());
            return;
        }
        
        _lifeTime -= Time.deltaTime;
    }

    private void FixedUpdate()
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
        _lifeTime = _originLifeTime;

        yield return new WaitForSeconds(_explodeEffect.main.duration);

        gameObject.SetActive(false);
    }

    public override void SetPoolData(LaunchData data)
    {
        _launchData = data;
        transform.forward = data.launchAngle;
        transform.position = data.position;

        _isReflected = false;
        _coll.enabled = true;
        _missileObj.SetActive(true);
        _explodeEffect.gameObject.SetActive(false);
        _isLaunch = true;
    }

    public void ReflectProjectile(Vector3 hitDir)
    {
        float hitMovSpeed = _launchData.speed * 4.0f;

        _lifeTime = _originLifeTime;
        _isReflected = true;
        transform.forward = hitDir;
        _launchData.speed = hitMovSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 반사시켰을 때
        if (_isReflected)
        {
            // 몬스터와 충돌
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                MonsterBase_1 monster = other.GetComponent<MonsterBase_1>();
                monster.DecreaseHP(_launchData.damage);
                StartCoroutine(Explosion());
            }
            // 땅과 충돌
            else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                StartCoroutine(Explosion());
            }
        }

        // 플레이어와 충돌
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            PlayerStat player = other.GetComponent<PlayerStat>();
            player.DecreaseHP(transform.forward, _launchData.damage);
            StartCoroutine(Explosion());
        }
        // 땅과 충돌
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StartCoroutine(Explosion());
        }
    }
}
