using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public GameObject _gameBtnGroup;
    public SettingDataEditor _settingPanel;
    public Button _firstSelecBtn;
    public bool _isDevelopMode;
    
    // Field
    private bool _isAlreadyInput;
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        string settingFilePath = DataManage.SavePath + "TestSettingData" + ".json";

        if (!File.Exists(settingFilePath))
        {
            print("세팅 데이터 없어서 새로 생성");
            SettingDataObj settingData = new SettingDataObj();
            DataManage.SaveSettingData(settingData, "TestSettingData");
        }
    }

    public void OnStartBtnClick()
    {
        if (_isAlreadyInput)
            return;

        _isAlreadyInput = true;
        AudioSource[] buttonAudio = _gameBtnGroup.transform.GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < buttonAudio.Length; i++)
        {
            buttonAudio[i].enabled = false;
        }

        StartCoroutine(EventOnStartBtnClick());
    }

    private IEnumerator EventOnStartBtnClick()
    {
        AudioClip audio = _firstSelecBtn.GetComponent<ButtonUI>()._onClickSound;

        _audio.PlayOneShot(audio, 1.0f * SettingData._sfxVolume);
        if (!_isDevelopMode)
            yield return new WaitForSeconds(4.0f);

        string mapFilePath = DataManage.SavePath + "TestMData" + ".json";
        string characterFilePath = DataManage.SavePath + "TestCData" + ".json";
        string buffFilePath = DataManage.SavePath + "TestBData" + ".json";

        if (!File.Exists(mapFilePath))
        {
            print("맵 데이터 없어서 새로 생성");
            MapData mapData = new MapData();
            DataManage.SaveMData(mapData, "TestMData");
        }

        if (!File.Exists(characterFilePath))
        {
            print("캐릭 데이터 없어서 새로 생성");
            CharacterData characterData = new CharacterData();
            DataManage.SaveCData(characterData, "TestCData");
        }

        if (!File.Exists(buffFilePath))
        {
            print("버프 데이터 없어서 새로 생성");
            BuffData buffData = new BuffData();
            DataManage.SaveBData(buffData, "TestBData");
        }

        // 데이터에 저장된 현재 플레이어의 위치(맵)에 맞는 씬을 불러옴
        CharacterData cDataPack = DataManage.LoadCData("TestCData");
        CharacterDataPackage._cDataInstance = cDataPack;
        LoadingScene.LoadScene(cDataPack._characterData._mapName);
    }

    public void OnExitBtnClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void OnSettingBtnClick()
    {
        bool isSettingPanelActive = _settingPanel.gameObject.activeSelf;

        if (isSettingPanelActive)
        {
            _settingPanel.gameObject.SetActive(false);
        }
        else
        {
            _settingPanel.gameObject.SetActive(true);
            _settingPanel.OpenCategory(_settingPanel._currOpenCategory);
        }
    }

    public void PlayBtnClickBGM(AudioClip clip)
    {
        _audio.PlayOneShot(clip, _audio.volume * SettingData._sfxVolume);
    }
}
