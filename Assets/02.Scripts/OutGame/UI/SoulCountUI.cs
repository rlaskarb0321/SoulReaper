using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SoulCountUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _totalSoul;

    [SerializeField]
    private TMP_Text _getSoulCount;

    [SerializeField]
    private float _fadeOutTime;

    [SerializeField]
    private float _countWaitTime;

    // Field
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public IEnumerator CountTotalSoul(float target, float current)
    {
        float framePerFadeOutTime = _fadeOutTime / Time.deltaTime; // FadeOutTime 은 몇 프레임?
        float amount = Mathf.Abs(target - current) / framePerFadeOutTime; // 한 프레임당 증감시켜야 되는 양

        _getSoulCount.text = $"+ {(int)target - current}";
        yield return new WaitForSeconds(_countWaitTime);

        StartCoroutine(CountGetSoul(target - current));
        while (current < target)
        {
            current += amount;
            _totalSoul.text = $"X {(int)current}";
            yield return null;
        }

        current = target;
        _totalSoul.text = $"X {(int)current}";
    }

    public IEnumerator CountGetSoul(float current)
    {
        float framePerFadeOutTime = _fadeOutTime / Time.deltaTime;
        float amount = current / framePerFadeOutTime;

        while (current > 0.0f)
        {
            current -= amount;
            _getSoulCount.text = $"+ {(int)current}";
            yield return null;
        }

        current = 0.0f;
        _getSoulCount.text = $"+ {(int)current}";
    }
}
