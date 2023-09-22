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

        // 로딩씬 불러오기
    }

    public void OnStartBtnClick()
    {
        //LoadingScene.LoadScene("Player");
        //LoadingScene.LoadScene("Only_Castle_Map");

        UnityEngine.SceneManagement.SceneManager.LoadScene("Player");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Only_Castle_Map", UnityEngine.SceneManagement.LoadSceneMode.Additive);
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
