using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public static string _nextScene;
    public Image _loadingFillImg;

    private void Start()
    {
        StartCoroutine(LoadSceneCor());
    }

    // 매개변수로 불러올 씬 이름을받는다, 로딩씬을 불러오고 로딩씬 해당스크립트의 Start함수로인해 매개변수명과 맞는 씬을 불러온다.
    public static void LoadScene(string sceneName)
    {
        _nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private IEnumerator LoadSceneCor()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(_nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                _loadingFillImg.fillAmount = Mathf.Lerp(_loadingFillImg.fillAmount, op.progress, timer);
                if (_loadingFillImg.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                _loadingFillImg.fillAmount = Mathf.Lerp(_loadingFillImg.fillAmount, 1f, timer);
                if (_loadingFillImg.fillAmount == 1.0f)
                {
                    yield return new WaitForSeconds(1.5f);

                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
