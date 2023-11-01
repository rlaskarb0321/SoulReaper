using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public GameObject _pressAnyKey;
    public GameObject _gameBtnGroup;
    public Button _firstSelecBtn;
    [Range(0.0f, 3.0f)] public float _buttonShowDelay;
    [Range(0.0f, 3.0f)] public float _loadNextSceneDelay;

    private WaitForSeconds _waitbtnShow;
    private WaitForSeconds _waitLoadNextScene;
    private bool _isAnyKeyInput;

    private void Awake()
    {
        _waitbtnShow = new WaitForSeconds(_buttonShowDelay);
        _waitLoadNextScene = new WaitForSeconds(_loadNextSceneDelay);
    }

    private void Start()
    {
        StartCoroutine(ShowAnyKeyText());
    }

    private void Update()
    {
        if (Input.anyKeyDown && _pressAnyKey.activeSelf && !_isAnyKeyInput)
        {
            StartCoroutine(OnPressAnyKey());
        }
    }

    private IEnumerator ShowAnyKeyText()
    {
        yield return _waitbtnShow;

        _pressAnyKey.SetActive(true);
    }

    private IEnumerator OnPressAnyKey()
    {
        _isAnyKeyInput = true;
        _pressAnyKey.SetActive(false);

        yield return _waitLoadNextScene;

        _gameBtnGroup.SetActive(true);
        _firstSelecBtn.Select();
    }

    public void OnStartBtnClick()
    {
        string mapFilePath = DataManage.SavePath + "TestMData" + ".json";
        string characterFilePath = DataManage.SavePath + "TestCData" + ".json";

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

        // 데이터에 저장된 현재 플레이어의 위치(맵)에 맞는 씬을 불러옴
        CharacterData cDataPack = DataManage.LoadCData("TestCData");
        CharacterDataPackage._characterData = cDataPack;
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
}
