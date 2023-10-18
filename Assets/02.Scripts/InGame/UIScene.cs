using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScene : MonoBehaviour
{
    [Header("=== UI ===")]
    [SerializeField] private List<GameObject> _currOpenPanel;
    [SerializeField] private GameObject _pausePanel; // �Ͻ�����
    public GameObject _letterPanel; // ������

    [Header("=== Seed UI ===")]
    public SeedUI _seedUI;

    [Header("=== Player ===")]
    [SerializeField] private PlayerData _stat;

    public enum ePercentageStat { Hp, Mp, }
    [Header("=== Hp & Mp ===")]
    [SerializeField] private Image _hpFill;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private Image _mpFill;
    [SerializeField] private TMP_Text _mpText;

    [Header("=== Scene Change ===")]
    [SerializeField] private TMP_Text _mapName;
    public SceneTeleport _sceneTeleport;

    [Header("=== Interact ===")]
    public GameObject _interactUI; // �ΰ��ӿ��� ��ȣ�ۿ� ������ ��ü ������ ���� �� �߰� �� �ؽ�Ʈ UI

    private void Awake()
    {
        EditMapName();
        _currOpenPanel = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel();
        }
    }


    #region UI Panel
    
    // UI�г��� Ű��, �����ִ� ui�� ����Ʈ�� ����
    public void SetUIPanelActive(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
        _currOpenPanel.Add(panel);
    }

    public void PausePanel()
    {
        // �����ִ� UI�� ������ ��
        if (_currOpenPanel.Count != 0)
        {
            int index = _currOpenPanel.Count - 1;

            _currOpenPanel[index].SetActive(false);
            _currOpenPanel.RemoveAt(index);
            return;
        }

        // PausePanel �� Ŵ
        SetUIPanelActive(_pausePanel);
    }

    #endregion UI Panel

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

    private void EditMapName()
    {

    }
}
