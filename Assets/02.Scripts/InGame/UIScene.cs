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
    private GameObject _pausePanel; // 일시정지
    public GameObject _letterPanel; // 전서구

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

    [Header("=== Interact ===")]
    public GameObject _interactUI; // 인게임에서 상호작용 가능한 물체 가까이 갔을 때 뜨게 할 텍스트 UI
    public TMP_Text _objName;
    private RectTransform _rect;

    [Header("=== Data ===")]
    [SerializeField]
    private DataApply _apply;

    [Header("=== Buff ===")]
    public BuffDataPackage _buffMgr;

    private void Awake()
    {
        _instance = this;

        _currOpenPanel = new List<GameObject>();
        _rect = _interactUI.GetComponent<RectTransform>();
        _mapName.text = EditMapName();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel();
        }

        // 테스트 용 소울 에너지 20 증가
        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdateSoulCount(20.0f);
        }

        // 테스트 용 소울 에너지 20 감소
        if (Input.GetKeyDown(KeyCode.V))
        {
            UpdateSoulCount(-20.0f);
        }
    }

    #region UI Panel
    // UI패널을 키고, 켜져있는 ui들 리스트에 넣음
    public void SetUIPanelActive(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
        _currOpenPanel.Add(panel);
    }

    public void PausePanel()
    {
        // 켜져있는 UI가 있으면 끔
        if (_currOpenPanel.Count != 0)
        {
            int index = _currOpenPanel.Count - 1;

            _currOpenPanel[index].SetActive(false);
            _currOpenPanel.RemoveAt(index);
            return;
        }

        // PausePanel 을 킴
        SetUIPanelActive(_pausePanel);
    }
    #endregion UI Panel

    public void UpdateSoulCount(float amount)
    {
        print("Edit Soul Data");

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

    // 씬 옮기면서 보여줄 맵 네임값을 수정
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
                return "성";

            case "LittleForest_Map":
                return "작은 숲";
        }

        return "";
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

    public void DeadPlayer()
    {
        // 플레이어 죽었을때 패널띄우고 최근에 저장했던 위치로 피 마나 다 채우고 옮기기 (씬 다시 불러오기)
        _playerDeath.AnnouncePlayerDeath();
    }
}
