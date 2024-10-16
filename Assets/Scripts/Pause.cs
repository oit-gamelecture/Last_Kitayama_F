using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;

    private bool isPaused = false;

    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        pauseMenuUI.SetActive(false);
        resumeButton.gameObject.SetActive(false); // �Q�[���J�n���ɔ�\��
    }

    void Update()
    {
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
        resumeButton.gameObject.SetActive(true); // �|�[�Y��ʂŕ\��
        Time.timeScale = 0f; // �Q�[�����ꎞ��~
        isPaused = true;
    }

    void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        resumeButton.gameObject.SetActive(false); // �Q�[���ĊJ���ɔ�\��
        Time.timeScale = 1f; // �Q�[�����ĊJ
        isPaused = false;
    }
}