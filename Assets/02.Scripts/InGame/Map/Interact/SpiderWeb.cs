using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWeb : Flammable
{
    private MeshRenderer[] _mesh;
    private Material[] _mats;
    private WaitForSeconds _ws;

    private void Awake()
    {
        _mesh = GetComponentsInChildren<MeshRenderer>();
        _ws = new WaitForSeconds(1.0f);

        _mats = new Material[_mesh.Length];
        for (int i = 0; i < _mats.Length; i++)
        {
            _mats[i] = Instantiate(_mesh[i].material);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void IgniteSelf()
    {
        base.IgniteSelf();

        for (int i = 0; i < _mats.Length; i++)
        {
            StartCoroutine(FadeDownWeb(_mats[i], _mesh[i]));
        }

        gameObject.GetComponent<BoxCollider>().enabled = false;
        _apply.EditData();
    }

    private IEnumerator FadeDownWeb(Material mat, MeshRenderer mesh)
    {
        yield return _ws;

        Color color = mat.color;
        while (mat.color.a > 0.05f)
        {
            color.a -= Time.deltaTime * 0.5f;
            mat.color = color;
            mesh.material = mat;
            yield return null;
        }

        mesh.gameObject.SetActive(false);
        if (_fireEffect.activeSelf)
            _fireEffect.gameObject.SetActive(false);
    }
}
