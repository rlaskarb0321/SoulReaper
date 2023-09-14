using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimSealedUrn : MonoBehaviour
{
    [Header("=== Map ===")]
    [SerializeField] private QuestRoom _roomMgr;

    [Header("=== Victim Sealed Urn ===")]
    [SerializeField] private GameObject _shatter;
    [SerializeField] private float _force;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Material _shatterFadeMat;

    [Header("=== Victim ===")]
    [SerializeField] private GameObject _victim;
    [SerializeField] private Material _victimFadeMat;

    [Header("=== Sealed Key ===")]
    [SerializeField] private GameObject _key;
    [SerializeField] private MeshRenderer _lock;
    [SerializeField] Material _unlockMat;
    [SerializeField] private float _keyRotateSpeed;

    private Rigidbody[] _rbodys;

    private void Awake()
    {
        _rbodys = _shatter.GetComponentsInChildren<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Explosion();
        }

        _key.transform.Rotate(Vector3.up * _keyRotateSpeed * Time.deltaTime);
    }

    public void Explosion()
    {
        // ����ȿ��
        for (int i = 0; i < _rbodys.Length; i++)
        {
            Transform originPos = _rbodys[i].transform;

            _rbodys[i].isKinematic = false;
            _rbodys[i].AddExplosionForce(_force, transform.position + _offset, 10.0f);
            StartCoroutine(FadeOutShatter(_rbodys[i].gameObject.GetComponent<MeshRenderer>(), _shatterFadeMat, _rbodys[i]));
        }

        // �� Ŭ���� ���� ä���, ����(�ڹ���) Ǯ�� ����
        _roomMgr.SolveQuest();
        _lock.material = _unlockMat;
        gameObject.GetComponent<Animator>().enabled = true;
        _key.gameObject.SetActive(false);
    }

    // �μ��� �� ������� ���̵�ƿ� �� ����
    private IEnumerator FadeOutShatter(MeshRenderer mesh, Material mat, Rigidbody rbody = null)
    {
        yield return new WaitForSeconds(5.0f);

        Material newMat = Instantiate(mat);
        Color color = newMat.color;

        if (rbody != null)
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

    private IEnumerator FadeOutVictim(SkinnedMeshRenderer mesh, Material mat)
    {
        yield return new WaitForSeconds(0.5f);

        Material newMat = Instantiate(mat);
        Color color = newMat.color;

        while (newMat.color.a > 0.05f)
        {
            color.a -= Time.deltaTime;
            newMat.color = color;
            mesh.material = newMat;
            yield return null;
        }

        _victim.gameObject.SetActive(false);
    }

    public void CallFadeOutVictim()
    {
        StartCoroutine(FadeOutVictim(_victim.GetComponentInChildren<SkinnedMeshRenderer>(), _victimFadeMat));
    }
}
