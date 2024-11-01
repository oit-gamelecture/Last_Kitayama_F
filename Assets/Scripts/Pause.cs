using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button titleButton;

    private bool isPaused = false;
    private float startTime;

    void Start()
    {
        startTime = Time.time; // �Q�[���J�n���̎��Ԃ��L�^

        resumeButton.onClick.AddListener(ResumeGame);
        titleButton.onClick.AddListener(ReturnToTitle);
        pauseMenuUI.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        titleButton.gameObject.SetActive(false); // �Q�[���J�n���ɔ�\��
    }

    void Update()
    {
        // �ŏ���3.3�b�Ԃ̓|�[�Y�𖳌���
        if (Time.time - startTime < 3.3f) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        titleButton.gameObject.SetActive(true); // �|�[�Y��ʂŕ\��
        Time.timeScale = 0f; // �Q�[�����ꎞ��~
        isPaused = true;
    }

    void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        titleButton.gameObject.SetActive(false); // �Q�[���ĊJ���ɔ�\��
        Time.timeScale = 1f; // �Q�[�����ĊJ
        isPaused = false;
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
