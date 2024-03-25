using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoParticleDisable : MonoBehaviour
{
    private ParticleSystem[] _particle;
    private WaitForSeconds _ws;

    private void Awake()
    {
        _particle = GetComponentsInChildren<ParticleSystem>();
        _ws = new WaitForSeconds(_particle[0].main.duration);
    }

    private void OnEnable()
    {
        for (int i = 0; i < _particle.Length; i++)
        {
            _particle[i].gameObject.SetActive(true);
            _particle[i].Play();
        }
        StartCoroutine(DisableParticle());
    }

    private IEnumerator DisableParticle()
    {
        yield return _ws;
        gameObject.SetActive(false);
    }
}
