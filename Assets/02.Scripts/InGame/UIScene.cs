using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScene : MonoBehaviour
{
    // Instance
    public static UIScene _instance;

    [Header("=== UI ===")]
    [SerializeField]
    private List<GameObject> _currOpenPanel;

    [SerializeField]
    private GameObject _pausePanel; // �Ͻ�����
    public GameObject _letterPanel; // ������


    [Header("=== Seed UI ===")]
    public SeedUI _seedUI;


    [Header("=== Player ===")]
    [SerializeField] private PlayerData _stat;


    [Header("=== Hp & Mp ===")]
    [SerializeField] 
    private Image _hpFill;

    [SerializeField] 
    private TMP_Text _hpText;

    [SerializeField] 
    private Image _mpFill;

    [SerializeField] 
    private TMP_Text _mpText;
    public enum ePercentageStat { Hp, Mp, }


    [Header("=== Scene Change ===")]
    [SerializeField] 
    private TMP_Text _mapName;


    [Header("=== Interact ===")]
    public GameObject _interactUI; // �ΰ��ӿ��� ��ȣ�ۿ� ������ ��ü ������ ���� �� �߰� �� �ؽ�Ʈ UI
    public TMP_Text _objName;
    private RectTransform _rect;


    private void Awake()
    {
        _instance = this;

        EditMapName();
        _currOpenPanel = new List<GameObject>();
        _rect = _interactUI.GetComponent<RectTransform>();
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
                _stat._currHP = currValue;
                _stat._maxHP = maxValue;
                _hpText.text = $"{currValue} / {maxValue}";
                _hpFill.fillAmount = currValue / maxValue;
                break;

            case ePercentageStat.Mp:
                _stat._currMP = currValue;
                _stat._maxMP = maxValue;
                _mpText.text = $"{currValue} / {maxValue}";
                _mpFill.fillAmount = currValue / maxValue;
                break;
        }
    }

    private void EditMapName()
    {

    }

    public void FloatInteractUI(bool turnOn, Vector3 pos, string text)
    {
        if (!turnOn)
        {
            _interactUI.SetActive(false);
            return;
        }

        if (!_objName.text.Equals(text))
        {
            _objName.text = text;
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
        _interactUI.transform.position = pos;
        _interactUI.SetActive(true);
    }
}
