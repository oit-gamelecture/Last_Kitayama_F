using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private GameObject _loadingUI; // ロード画面UI
    [SerializeField] private Slider _slider;       // スライダー

    public void LoadNextScene()
    {
        _loadingUI.SetActive(true); // ロード画面を表示
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("main");
        async.allowSceneActivation = false; // 自動でシーン遷移しないように設定

        float progress = 0f;

        while (async.progress < 0.9f) // ロード進行状況が0.9未満の場合
        {
            progress = Mathf.Lerp(progress, async.progress, Time.deltaTime * 3f); // スムーズな更新
            _slider.value = progress; // スライダーを更新
            yield return null;
        }

        // ロード進行状況を1.0に更新
        _slider.value = 1f;

        // 1秒間待機
        yield return new WaitForSeconds(1f);

        // シーン遷移を許可
        async.allowSceneActivation = true;
    }
}
