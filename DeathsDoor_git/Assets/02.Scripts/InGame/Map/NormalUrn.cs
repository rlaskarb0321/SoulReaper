using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUrn : MonoBehaviour
{
    [Header("=== Normal Urn ===")]
    [SerializeField] private GameObject _shatter;
    [SerializeField] private float _force;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _restoreDelay;

    private bool _isExploded;
    private int _recoverCount;
    private WaitForSeconds _ws;
    private Rigidbody[] _rbodys;
    private PlayerData _playerData;

    private void Awake()
    {
        _rbodys = _shatter.GetComponentsInChildren<Rigidbody>();
    }

    private void Start()
    {
        _ws = new WaitForSeconds(_restoreDelay);
    }

    public void Explosion(Vector3 dir)
    {
        if (_isExploded)
            return;

        _isExploded = true;
        for (int i = 0; i < _rbodys.Length; i++)
        {
            Transform originPos = _rbodys[i].transform;

            _rbodys[i].isKinematic = false;
            _rbodys[i].AddExplosionForce(_force, transform.position + dir, 10.0f);
            StartCoroutine(Restoration(_rbodys[i], originPos));
        }
    }

    // 깨지고 난 후, 파편들을 원래 위치와 각도로 복구
    private IEnumerator Restoration(Rigidbody shatter, Transform pos)
    {
        yield return _ws;

        shatter.isKinematic = true;
        StartCoroutine(RestorationPos(shatter, pos));
        StartCoroutine(RestorationRot(shatter, pos));
    }

    private IEnumerator RestorationPos(Rigidbody shatter, Transform pos)
    {
        Vector3 path = pos.position - transform.position;
        float movSpeed;

        while (Mathf.Abs(path.magnitude) > 0.1f)
        {
            movSpeed = Mathf.Lerp(-1.0f, path.magnitude, 1.0f);
            shatter.MovePosition(shatter.transform.position - path.normalized * movSpeed * Time.fixedDeltaTime);
            path = pos.position - transform.position;

            yield return new WaitForFixedUpdate();
        }

        shatter.transform.localPosition = Vector3.zero;
        CompleteRecover();
    }

    private IEnumerator RestorationRot(Rigidbody shatter, Transform pos)
    {
        Quaternion originalRotValue = Quaternion.identity; // 파편들이 다 모였을때의 각도
        Quaternion rotVariable = shatter.rotation; // 깨지고 나서의 각도
        Quaternion targetRot = rotVariable;
        float t = 0.5f;

        while (shatter.transform.rotation != originalRotValue)
        {
            targetRot =
                new Quaternion(Mathf.Lerp(targetRot.x, originalRotValue.x, t), Mathf.Lerp(targetRot.y, originalRotValue.y, t)
                , Mathf.Lerp(targetRot.z, originalRotValue.z, t), Mathf.Lerp(targetRot.w, originalRotValue.w, t));

            targetRot = targetRot.normalized;
            shatter.MoveRotation(targetRot);
            yield return new WaitForFixedUpdate();
        }
    }

    private void CompleteRecover()
    {
        _recoverCount++;
        if (_recoverCount == _rbodys.Length)
        {
            _isExploded = false;
            _recoverCount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerProjectile") &&
            other.gameObject.layer != LayerMask.NameToLayer("PlayerWeapon"))
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon") && !_isExploded)
        {
            if (_playerData == null)
                _playerData = other.GetComponentInParent<PlayerData>();

            _playerData.DecreaseMP(-10.0f);
        }

        Vector3 dir = transform.position - other.gameObject.transform.position;
        dir = dir.normalized;
        Explosion(dir);
    }
}
