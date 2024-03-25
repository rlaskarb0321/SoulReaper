using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffect : MonoBehaviour
{
    private Transform _followPos;
    private float _duration;
    private ParticleSystem _particle;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (_followPos == null)
            return;

        transform.position = _followPos.position;
    }

    public void InitBuffEffect(Transform tr, float duration)
    {
        _followPos = tr;
        _duration = duration;

        StartCoroutine(SetBuffDeactive());
    }

    private IEnumerator SetBuffDeactive()
    {
        yield return new WaitForSeconds(_duration - 0.2f);

        ParticleSystem.MainModule main = _particle.main;
        main.loop = false;
    }
}
