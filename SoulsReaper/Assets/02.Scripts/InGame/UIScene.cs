using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIScene : MonoBehaviour
{
    // Instance
    public static UIScene _instance;

    [Header("=== 패널 & 알람 ===")]
    [SerializeField]
    private List<GameObject> _currOpenPanel;
    
    [SerializeField]
    private GameObject _pausePanel; // 일시정지
    public LetterPanel _letter; // 전서구

    [Space(10.0f)]
    [SerializeField]
    private TMP_Text _alarmText;

    [Header("=== 생명의 씨앗 UI ===")]
    public SeedUI _seedUI;

    [Header("=== 플레이어 ===")]
    public PlayerData _stat;

    [SerializeField]
    private PlayerDeath _playerDeath;

    [Header("=== Hp & Mp ===")]
    [SerializeField] 
    private Image _hpFill;

    [SerializeField]
    private Image _hpBorder;

    [SerializeField] 
    private TMP_Text _hpText;

    [Space(10.0f)]
    [SerializeField] 
    private Image _mpFill;

    [SerializeField]
    private Image _mpBorder;

    [SerializeField]
    private AudioClip _lowMPWarnSound;

    [SerializeField]
    private int _lowMPWarnRepeat;

    [SerializeField] 
    private TMP_Text _mpText;
    public enum ePercentageStat { HP, MP, }

    [Header("=== 영혼 획득 카운트 ===")]
    [SerializeField]
    private SoulCountUI _soulCount;

    [Header("=== 씬 바꾸기 ===")]
    [SerializeField] 
    private TMP_Text _mapName;

    [Header("=== 물체에 뜨게 할 UI ===")]
    public RectTransform _interactUI; // 인게임에서 상호작용 가능한 물체 가까이 갔을 때 뜨게 할 텍스트 UI
    public RectTransform _dialogBallon; // 보스가 띄울 말풍선 UI

    [SerializeField]
    private GameObject _gaugeObj;

    [SerializeField]
    private Image _gaugeFill;

    private Color _originColor;
    private string _uiTag; // 최근에 띄운 UI의 태그를 저장, 만일 저장한 값과 새로 들어온 UI의 태그가 다르다면 _context 를 새로 getcomponent 
    private TMP_Text _context;
    private Vector3 _originGaugePos;

    [Header("=== 버프 ===")]
    public BuffDataPackage _buffMgr;

    [Header("=== 스킬 ===")]
    public SkillList _skillMgr;

    [Header("=== 데이터 ===")]
    [SerializeField]
    private DataApply _apply;

    // Field
    private AudioSource _audio;

    private void Awake()
    {
        _instance = this;

        _audio = GetComponent<AudioSource>();
        _currOpenPanel = new List<GameObject>();
        _mapName.text = EditMapName();
    }

    private void Start()
    {
        _originGaugePos = _gaugeObj.transform.position;
        _originColor = _gaugeFill.color;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel();
        }

        // 테스트 용 소울 에너지 1000 증가
        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdateSoulCount(1000.0f);
        }

        // 테스트 용 소울 에너지 100 감소
        if (Input.GetKeyDown(KeyCode.V))
        {
            UpdateSoulCount(-100.0f);
        }
    }

    #region UI Panel

    /// <summary>
    /// UI패널을 키고 , 켜져있는 ui들 리스트에 넣음
    /// </summary>
    /// <param name="panel"></param>
    public void SetUIPanelActive(GameObject panel)
    {
        UIPanel uiPanel = panel.GetComponent<UIPanel>();

        panel.SetActive(!panel.activeSelf);
        _currOpenPanel.Add(panel);
        if (uiPanel != null)
        {
            uiPanel.PlaySetActiveSound(true, _audio);
        }
    }

    /// <summary>
    /// UI 패널을 끄고, ui 리스트에서 뺌
    /// </summary>
    /// <param name="panel"></param>
    public void SetUIPanelDeactive(GameObject panel)
    {
        if (!_currOpenPanel.Contains(panel))
            return;

        panel.SetActive(false);
        _currOpenPanel.Remove(panel);
    }

    public void PausePanel()
    {
        UIPanel uiPanel;

        // 켜져있는 UI가 있으면 끔
        if (_currOpenPanel.Count != 0)
        {
            int index = _currOpenPanel.Count - 1;
            uiPanel = _currOpenPanel[index].GetComponent<UIPanel>();
            if (uiPanel != null)
            {
                uiPanel.PlaySetActiveSound(false, _audio);
            }

            _currOpenPanel[index].SetActive(false);
            _currOpenPanel.RemoveAt(index);
            return;
        }

        // PausePanel 을 킴
        uiPanel = _pausePanel.GetComponent<UIPanel>();
        if (uiPanel != null)
            uiPanel.PlaySetActiveSound(true, _audio);

        SetUIPanelActive(_pausePanel);
    }
    #endregion UI Panel

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    /// <summary>
    /// 영혼 재화 보유량 값을 변동시킬 때 쓰는 메서드
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateSoulCount(float amount)
    {
        // UI 카운트 효과
        _soulCount.StartCount
            (
            CharacterDataPackage._cDataInstance._characterData._soulCount + amount,
            CharacterDataPackage._cDataInstance._characterData._soulCount
            );

        // 데이터 저장
        _stat._soulCount = (int)CharacterDataPackage._cDataInstance._characterData._soulCount + (int)amount;
        CharacterDataPackage._cDataInstance._characterData._soulCount = _stat._soulCount;
        DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");
    }

    /// <summary>
    /// 보유 씨앗량을 변동시킬 때 쓰는 메서드
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateSeedCount(int amount)
    {
        _seedUI.EditSeedText(amount);
        _stat._seedCount = amount;

        CharacterDataPackage._cDataInstance._characterData._alreadySeedGet =
            CharacterDataPackage._cDataInstance._characterData._alreadySeedGet == false ? true : true;
        CharacterDataPackage._cDataInstance._characterData._seedCount = _stat._seedCount;
        DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");
    }

    #region HP & MP
    /// <summary>
    /// 필요한 Mp량보다 보유한 Mp량이 적을때 호출됨, Mp 부족 UX 관련 메서드이다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator WarnLowMP()
    {
        Color originColor = Color.white;
        _audio.clip = _lowMPWarnSound;
        if (_audio.isPlaying)
            yield break;

        _audio.Play();
        for (int i = 0; i < _lowMPWarnRepeat; i++)
        {
            _mpBorder.color = Color.blue;
            yield return new WaitForSeconds(0.2f);

            _mpBorder.color = originColor;
            yield return new WaitForSeconds(0.2f);
        }
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
    #endregion HP & MP

    #region Buff
    public void UpdatePlayerDamage(float amount)
    {
        _stat._basicAtkDamage = amount;
    }

    public void UpdatePlayerMovSpeed(float amount)
    {
        _stat._move._movSpeed = amount;
    }

    #endregion Buff

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

    public void FloatInteractTextUI(RectTransform target, bool turnOn, Vector3 pos, string text)
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

        if (!_context.text.Equals(text) && !text.Equals(""))
        {
            _context.text = text;
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        target.transform.position = pos;
        target.gameObject.SetActive(true);
    }

    public void FloatAlarmTextUI(string content)
    {
        _alarmText.gameObject.SetActive(false);
        _alarmText.gameObject.SetActive(true);
        _alarmText.text = content;
    }

    #region 게이지 UI
    public void SetGaugeUI(bool activeValue) => _gaugeObj.SetActive(activeValue);

    public void SetGaugeFill(float curr, float total) => _gaugeFill.fillAmount = curr / total;

    public IEnumerator ChangeGaugeColor(string htmlString)
    {
        Color changeColor;
        ColorUtility.TryParseHtmlString(htmlString, out changeColor);
        _gaugeFill.color = changeColor;

        yield return new WaitForSeconds(Time.deltaTime * 4.0f);

        _gaugeFill.color = _originColor;
    }

    public IEnumerator ShakeGaugeUI(float amount, float dur)
    {
        Vector3 originPos = _originGaugePos;
        while (dur > 0.0f)
        {
            Vector3 shakePos = Random.insideUnitCircle * amount;

            _gaugeObj.transform.position += shakePos;
            dur -= Time.deltaTime;
            yield return null;
        }

        _gaugeObj.transform.position = originPos;
    }
    #endregion 게이지 UI

    public void DeadPlayer()
    {
        // 플레이어 죽었을때 패널띄우고 최근에 저장했던 위치로 피 마나 다 채우고 옮기기 (씬 다시 불러오기)
        _playerDeath.AnnouncePlayerDeath();
    }
}
