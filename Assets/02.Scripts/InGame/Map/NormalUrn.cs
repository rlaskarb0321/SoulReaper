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

    private WaitForSeconds _ws;
    private Rigidbody[] _rbodys;

    private void Awake()
    {
        _rbodys = _shatter.GetComponentsInChildren<Rigidbody>();
    }

    private void Start()
    {
        _ws = new WaitForSeconds(_restoreDelay);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Explosion();
        }
    }

    public void Explosion()
    {
        for (int i = 0; i < _rbodys.Length; i++)
        {
            Transform originPos = _rbodys[i].transform;

            _rbodys[i].isKinematic = false;
            _rbodys[i].AddExplosionForce(_force, transform.position + _offset, 10.0f);
            StartCoroutine(Restoration(_rbodys[i], originPos));
        }
    }

    // ������ �� ��, ������� ���� ��ġ�� ������ ����
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
    }

    private IEnumerator RestorationRot(Rigidbody shatter, Transform pos)
    {
        Quaternion originalRotValue = Quaternion.identity; // ������� �� �������� ����
        Quaternion rotVariable = shatter.rotation; // ������ ������ ����
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
}
