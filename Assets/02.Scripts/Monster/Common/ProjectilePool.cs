using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePool : MonoBehaviour
{
    public IObjectPool<EnemyProjectile> _pool;

    LongRangeMonster _rangerMonster;

    void Awake()
    {
        _rangerMonster = GetComponent<LongRangeMonster>();
        _pool = new ObjectPool<EnemyProjectile>(CreateProjectile, OnGetProjectile, OnReleaseProjectile, DestroyProjectile, maxSize: 3);
    }

    EnemyProjectile CreateProjectile()
    {
        EnemyProjectile projectile = Instantiate(_rangerMonster._projectile, _rangerMonster._projectileSpawnPos.position, transform.rotation).GetComponent<EnemyProjectile>();
        projectile.SetManagedPool(_pool);

        return projectile;
    }

    void OnGetProjectile(EnemyProjectile projectile)
    {
        projectile.gameObject.SetActive(true);

        projectile._isReleased = false;
        projectile.transform.position = _rangerMonster._projectileSpawnPos.position;
        projectile.transform.rotation = transform.rotation;
    }

    void OnReleaseProjectile(EnemyProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    void DestroyProjectile(EnemyProjectile projectile)
    {
        Destroy(projectile.gameObject);
    }
}
