using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private GameObject _loadingUI; // ���[�h���UI
    [SerializeField] private Slider _slider;       // �X���C�_�[

    public void LoadNextScene()
    {
        _loadingUI.SetActive(true); // ���[�h��ʂ�\��
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("main");
        async.allowSceneActivation = false; // �����ŃV�[���J�ڂ��Ȃ��悤�ɐݒ�

        float progress = 0f;

        while (async.progress < 0.9f) // ���[�h�i�s�󋵂�0.9�����̏ꍇ
        {
            progress = Mathf.Lerp(progress, async.progress, Time.deltaTime * 3f); // �X���[�Y�ȍX�V
            _slider.value = progress; // �X���C�_�[���X�V
            yield return null;
        }

        // ���[�h�i�s�󋵂�1.0�ɍX�V
        _slider.value = 1f;

        // 1�b�ԑҋ@
        yield return new WaitForSeconds(1f);

        // �V�[���J�ڂ�����
        async.allowSceneActivation = true;
    }
}
