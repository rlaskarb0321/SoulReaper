using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SoulCountUI : MonoBehaviour
{
    [Header("=== Hierarchy ===")]
    [SerializeField]
    private TMP_Text _totalSoul;

    [SerializeField]
    private TMP_Text[] _getSoulAlarmArr;

    [Header("=== Float Value ===")]
    [SerializeField]
    private float _fadeOutTime;

    [SerializeField]
    private float _countWaitTime;

    // Field
    private WaitForSeconds _ws;
    private Animator[] _alarmAnim;

    private void Awake()
    {
        _ws = new WaitForSeconds(_countWaitTime);
        _alarmAnim = new Animator[_getSoulAlarmArr.Length];
        for (int i = 0; i < _alarmAnim.Length; i++)
        {
            _alarmAnim[i] = _getSoulAlarmArr[i].GetComponent<Animator>();
        }
    }

    public void StartCount(float target, float current)
    {
        char calcSign = (target < current) ? '-' : '+';

        for (int i = 0; i < _getSoulAlarmArr.Length; i++)
        {
            if (_getSoulAlarmArr[i].gameObject.activeSelf)
                continue;

            _getSoulAlarmArr[i].text = $"{calcSign} {Mathf.Abs(target - current)}";
            _getSoulAlarmArr[i].gameObject.SetActive(true);
            _getSoulAlarmArr[i].transform.SetAsFirstSibling();

            StartCoroutine(CountTotalSoul(target, current, i));
            break;
        }
    }

    private IEnumerator CountTotalSoul(float target, float current, int index)
    {
        yield return _ws;

        float framePerFadeOutTime = _fadeOutTime / Time.deltaTime; // FadeOutTime 은 몇 프레임?
        float amount = (target - current) / framePerFadeOutTime; // 한 프레임당 증감시켜야 되는 양
        StartCoroutine(CountGetSoul(target - current, index));

        while (current < target)
        {
            current += amount;
            _totalSoul.text = $"X {(int)current}";
            yield return null;
        }

        current = target;
        _totalSoul.text = $"X {(int)current}";
    }

    private IEnumerator CountGetSoul(float current, int index)
    {
        _alarmAnim[index].enabled = true;

        float framePerFadeOutTime = _fadeOutTime / Time.deltaTime;
        float amount = current / framePerFadeOutTime;

        while (current > 0.0f)
        {
            current -= amount;
            _getSoulAlarmArr[index].text = $"+ {(int)current}";
            yield return null;
        }

        current = 0.0f;
        _getSoulAlarmArr[index].text = $"+ {(int)current}";
        _alarmAnim[index].enabled = false;
        _getSoulAlarmArr[index].gameObject.SetActive(false);
    }
}
