using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingMainScene : MonoBehaviour
{
    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private Slider _slider;

    public void LoadNextScene()
    {
        _loadingUI.SetActive(true);
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("main"); // シーン名を確認
        async.allowSceneActivation = false; // 自動遷移を一時停止

        while (async.progress < 0.9f)
        {
            _slider.value = async.progress / 0.9f; // スライダー値をスケール調整
            yield return null;
        }

        // スライダーを満タンにする
        _slider.value = 1.0f;

        // シーン遷移を許可
        async.allowSceneActivation = true;
    }
}
