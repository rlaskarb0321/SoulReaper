using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public GameObject _gameBtnGroup;
    public Button _firstSelecBtn;
    
    // Field
    private bool _isAlreadyInput;
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
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

        _audio.PlayOneShot(audio);
        yield return new WaitForSeconds(1.5f);

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
}
