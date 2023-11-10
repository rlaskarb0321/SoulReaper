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
            print("�� ������ ��� ���� ����");
            MapData mapData = new MapData();
            DataManage.SaveMData(mapData, "TestMData");
        }

        if (!File.Exists(characterFilePath))
        {
            print("ĳ�� ������ ��� ���� ����");
            CharacterData characterData = new CharacterData();
            DataManage.SaveCData(characterData, "TestCData");
        }

        if (!File.Exists(buffFilePath))
        {
            print("���� ������ ��� ���� ����");
            BuffData buffData = new BuffData();
            DataManage.SaveBData(buffData, "TestBData");
        }

        // �����Ϳ� ����� ���� �÷��̾��� ��ġ(��)�� �´� ���� �ҷ���
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
