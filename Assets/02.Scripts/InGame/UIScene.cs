using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    [SerializeField]
    private PlayerData _stat;

    [SerializeField]
    private PlayerDeath _playerDeath;

    [Header("=== Hp & Mp ===")]
    [SerializeField] 
    private Image _hpFill;

    [SerializeField] 
    private TMP_Text _hpText;

    [SerializeField] 
    private Image _mpFill;

    [SerializeField] 
    private TMP_Text _mpText;
    public enum ePercentageStat { HP, MP, }

    [Header("=== Soul Count ===")]
    [SerializeField]
    private SoulCountUI _soulCount;

    [Header("=== Scene Change ===")]
    [SerializeField] 
    private TMP_Text _mapName;

    [Header("=== Float UI ===")]
    public RectTransform _interactUI; // �ΰ��ӿ��� ��ȣ�ۿ� ������ ��ü ������ ���� �� �߰� �� �ؽ�Ʈ UI
    public RectTransform _dialogBallon; // ������ ��� ��ǳ�� UI
    private string _uiTag; // �ֱٿ� ��� UI�� �±׸� ����, ���� ������ ���� ���� ���� UI�� �±װ� �ٸ��ٸ� _context �� ���� getcomponent 
    private TMP_Text _context;

    [Header("=== Buff ===")]
    public BuffDataPackage _buffMgr;

    [Header("=== Skill ===")]
    public SkillList _skillMgr;

    [Header("=== Data ===")]
    [SerializeField]
    private DataApply _apply;

    private void Awake()
    {
        _instance = this;

        _currOpenPanel = new List<GameObject>();
        _mapName.text = EditMapName();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel();
        }

        // �׽�Ʈ �� �ҿ� ������ 20 ����
        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdateSoulCount(20.0f);
        }

        // �׽�Ʈ �� �ҿ� ������ 20 ����
        if (Input.GetKeyDown(KeyCode.V))
        {
            UpdateSoulCount(-20.0f);
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

    public void UpdateSoulCount(float amount)
    {
        _soulCount.StartCount
            (
            CharacterDataPackage._cDataInstance._characterData._soulCount + amount,
            CharacterDataPackage._cDataInstance._characterData._soulCount
            );

        _stat._soulCount = (int)CharacterDataPackage._cDataInstance._characterData._soulCount + (int)amount;
        CharacterDataPackage._cDataInstance._characterData._soulCount = _stat._soulCount;
        DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");
    }

    public void UpdateHPMP(ePercentageStat stat, float currValue, float maxValue, bool isDataEdit = true)
    {
        if (currValue > maxValue)
            currValue = maxValue;

        switch (stat)
        {
            case ePercentageStat.HP:
                _stat._currHP = currValue;
                _stat._maxHP = maxValue;
                _hpText.text = $"{currValue} / {maxValue}";
                _hpFill.fillAmount = currValue / maxValue;
                break;

            case ePercentageStat.MP:
                _stat._currMP = currValue;
                _stat._maxMP = maxValue;
                _mpText.text = $"{currValue} / {maxValue}";
                _mpFill.fillAmount = currValue / maxValue;
                break;
        }

        if (isDataEdit)
        {
            _apply.EditData();
        }
    }

    public void UpdatePlayerDamage(float amount)
    {
        _stat._basicAtkDamage = amount;
    }

    // �� �ű�鼭 ������ �� ���Ӱ��� ����
    private string EditMapName()
    {
        string mapName;
        int sceneCount = SceneManager.sceneCount;
        List<string> sceneList = new List<string>();
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = SceneManager.GetSceneAt(i).name;
            if (!sceneName.Contains("_Map"))
                continue;

            sceneList.Add(sceneName);
        }

        mapName = sceneList[0];
        switch (mapName)
        {
            case "Castle_Map":
                return "��";

            case "LittleForest_Map":
                return "���� ��";
        }

        return "";
    }

    public void FloatGameObjectUI(RectTransform target, bool turnOn, Vector3 pos, string text)
    {
        if (!turnOn)
        {
            target.gameObject.SetActive(false);
            return;
        }

        if (_uiTag != target.tag)
        {
            _context = target.GetComponentInChildren<TMP_Text>();
            _uiTag = target.tag;
        }

        if (!_context.text.Equals(text))
        {
            _context.text = text;
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        target.transform.position = pos;
        target.gameObject.SetActive(true);
    }

    public void DeadPlayer()
    {
        // �÷��̾� �׾����� �гζ��� �ֱٿ� �����ߴ� ��ġ�� �� ���� �� ä��� �ű�� (�� �ٽ� �ҷ�����)
        _playerDeath.AnnouncePlayerDeath();
    }
}
