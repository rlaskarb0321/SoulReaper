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
    private float _countTime;

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

        float framePerFadeOutTime = _countTime / Time.deltaTime; // FadeOutTime 은 몇 프레임?
        float dist = target - current;
        float amount = Mathf.Abs(dist) / framePerFadeOutTime; // 한 프레임당 증감시켜야 되는 양
        StartCoroutine(CountGetSoul(dist, index));

        if (dist < 0.0f)
        {
            while (target < current)
            {
                current -= amount;
                _totalSoul.text = $"X {(int)current}";
                yield return null;
            }
        }
        else
        {
            while (current < target)
            {
                current += amount;
                _totalSoul.text = $"X {(int)current}";
                yield return null;
            }
        }


        current = target;
        _totalSoul.text = $"X {(int)current}";
    }

    private IEnumerator CountGetSoul(float value, int index)
    {
        _alarmAnim[index].enabled = true;

        char calcSign = value < 0 ? '-' : '+';
        float framePerFadeOutTime = _countTime / Time.deltaTime;
        float amount = Mathf.Abs(value) / framePerFadeOutTime;

        value = Mathf.Abs(value);
        while (value > 0.0f)
        {
            value -= amount;
            _getSoulAlarmArr[index].text = $"{calcSign} {(int)value}";
            yield return null;
        }

        value = 0.0f;
        _getSoulAlarmArr[index].text = $"{calcSign} {(int)value}";
        _alarmAnim[index].enabled = false;
        _getSoulAlarmArr[index].gameObject.SetActive(false);
    }
}
