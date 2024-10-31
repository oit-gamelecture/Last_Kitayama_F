using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button titleButton;

    private bool isPaused = false;

    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        titleButton.onClick.AddListener(ReturnToTitle);
        pauseMenuUI.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        titleButton.gameObject.SetActive(false); // ゲーム開始時に非表示
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
        resumeButton.gameObject.SetActive(true);
        titleButton.gameObject.SetActive(true); // ポーズ画面で表示
        Time.timeScale = 0f; // ゲームを一時停止
        isPaused = true;
    }

    void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        titleButton.gameObject.SetActive(false); // ゲーム再開時に非表示
        Time.timeScale = 1f; // ゲームを再開
        isPaused = false;
    }

    public void ReturnToTitle()
    {
         // ゲームを再開してからシーンを切り替えます
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title"); 
    }
}
