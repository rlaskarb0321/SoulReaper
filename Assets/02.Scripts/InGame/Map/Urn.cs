using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Urn : MonoBehaviour
{
    private enum eUrnType { Normal, VictimSealed, }

    [SerializeField] private GameObject _shatters;
    [SerializeField] private float _force;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private eUrnType _urnType;
    [SerializeField] private float _restoreDelay;

    [Header("=== Victim Sealed Urn ===")]
    [SerializeField] private Material _fadeMat;
    [SerializeField] private GameObject _key;
    [SerializeField] private float _keyRotateSpeed;

    private WaitForSeconds _ws;
    private Rigidbody[] _rbodys;

    private void Awake()
    {
        _rbodys = _shatters.GetComponentsInChildren<Rigidbody>();
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

        if (_urnType == eUrnType.VictimSealed)
        {
            _key.transform.Rotate(Vector3.up * _keyRotateSpeed * Time.deltaTime); 
        }
    }

    public void Explosion()
    {
        switch (_urnType)
        {
            case eUrnType.Normal:
                for (int i = 0; i < _rbodys.Length; i++)
                {
                    Transform originPos = _rbodys[i].transform;

                    _rbodys[i].isKinematic = false;
                    _rbodys[i].AddExplosionForce(_force, transform.position + _offset, 10.0f);
                    StartCoroutine(Restoration(_rbodys[i], originPos));
                }
                break;

            case eUrnType.VictimSealed:
                for (int i = 0; i < _rbodys.Length; i++)
                {
                    Transform originPos = _rbodys[i].transform;

                    _rbodys[i].isKinematic = false;
                    _rbodys[i].AddExplosionForce(_force, transform.position + _offset, 10.0f);
                    StartCoroutine(FadeOutShatters(_rbodys[i].gameObject.GetComponent<MeshRenderer>(), _rbodys[i]));
                }

                _key.gameObject.SetActive(false);
                break;
        }

    }

    #region Normal Urn 는 부서진 파편들을 회복함
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

        while (Mathf.Abs(path.magnitude) > 0.0f)
        {
            movSpeed = Mathf.Lerp(-1.0f, path.magnitude, 1.0f);
            shatter.MovePosition(shatter.transform.position - path.normalized * movSpeed * Time.fixedDeltaTime);
            path = pos.position - transform.position;

            yield return new WaitForFixedUpdate();
        }
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
    #endregion Normal Urn 는 부서진 파편들을 회복함

    // Vicim Sealed Urn 은 부서진 후 파편들을 없앰
    private IEnumerator FadeOutShatters(MeshRenderer mesh, Rigidbody rbody)
    {
        yield return new WaitForSeconds(4.0f);

        Material newMat = Instantiate(_fadeMat);
        Color color = newMat.color;

        rbody.isKinematic = true;
        while (newMat.color.a > 0.05f)
        {
            color.a -= Time.deltaTime;
            newMat.color = color;
            mesh.material = newMat;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
