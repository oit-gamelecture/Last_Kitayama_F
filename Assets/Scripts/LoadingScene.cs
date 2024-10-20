using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
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
        AsyncOperation async = SceneManager.LoadSceneAsync("main");
        while (!async.isDone)
        {
            _slider.value = async.progress;
            yield return null;
        }
    }
}
