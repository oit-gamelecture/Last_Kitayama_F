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
        AsyncOperation async = SceneManager.LoadSceneAsync("main"); // �V�[�������m�F
        async.allowSceneActivation = false; // �����J�ڂ��ꎞ��~

        while (async.progress < 0.9f)
        {
            _slider.value = async.progress / 0.9f; // �X���C�_�[�l���X�P�[������
            yield return null;
        }

        // �X���C�_�[�𖞃^���ɂ���
        _slider.value = 1.0f;

        // �V�[���J�ڂ�����
        async.allowSceneActivation = true;
    }
}
