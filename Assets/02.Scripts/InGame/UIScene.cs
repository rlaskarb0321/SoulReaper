using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScene : MonoBehaviour
{
    [Header("=== Panel ===")]
    [SerializeField] private GameObject _pausePanel;

    [Header("=== Player ===")]
    [SerializeField] private PlayerStat _stat;

    [Header("=== Hp & Mp ===")]
    [SerializeField] private Image _hpFill;
    [SerializeField] private TMP_Text _hpText;
    public enum ePercentageStat { Hp, Mp, }
    [SerializeField] private Image _mpFill;
    [SerializeField] private TMP_Text _mpText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetUIActive(_pausePanel);
        }
    }

    public void SetUIActive(GameObject uiObj)
    {
        bool isOn = uiObj.activeSelf;
        uiObj.SetActive(!isOn);
    }

    public void UpdateHPMP(ePercentageStat stat, float currValue, float maxValue)
    {
        switch (stat)
        {
            case ePercentageStat.Hp:
                _hpText.text = $"{currValue} / {maxValue}";
                _hpFill.fillAmount = currValue / maxValue;
                break;
            case ePercentageStat.Mp:
                _mpText.text = $"{currValue} / {maxValue}";
                _mpFill.fillAmount = currValue / maxValue;
                break;
        }
    }
}
