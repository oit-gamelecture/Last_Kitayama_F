using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Button volumeButton;
    public Button titleButton;
    public Slider volumeSlider;
    public GameObject defaultSelectedButton;

    private bool isAdjustingVolume = false;

    void Start()
    {
        // 最初に選択されるボタンを設定
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        EventSystem.current.SetSelectedGameObject(volumeButton.gameObject);

        // スライダーを非表示にして無効化
        volumeSlider.interactable = false;
        volumeSlider.value = AudioListener.volume;
    }

    void Update()
    {
        if (isAdjustingVolume)
        {
            HandleVolumeAdjustment();
        }
        else
        {
            HandleButtonSelection();
        }
    }

    void HandleButtonSelection()
    {
        // Enterキーで音量調節モードに切り替え
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0")) && EventSystem.current.currentSelectedGameObject == volumeButton.gameObject)
        {
            EnterVolumeAdjustmentMode();
        }

        // タイトルシーンに戻る処理
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0")) && EventSystem.current.currentSelectedGameObject == titleButton.gameObject)
        {
            SceneManager.LoadScene("Title");
        }
    }

    void HandleVolumeAdjustment()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        volumeSlider.value += horizontalInput * Time.deltaTime;

        // AudioListener.volumeにスライダーの値を反映
        AudioListener.volume = volumeSlider.value;

        // Enterキーで音量調節モードを終了
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            ExitVolumeAdjustmentMode();
        }
    }

    void EnterVolumeAdjustmentMode()
    {
        isAdjustingVolume = true;

        // スライダーを表示して選択
        volumeSlider.interactable = true;
        EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);

        // 他のボタンを無効化
        volumeButton.interactable = false;
        titleButton.interactable = false;
    }

    void ExitVolumeAdjustmentMode()
    {
        isAdjustingVolume = false;

        // スライダーを非表示にしてリセット

        volumeSlider.interactable = false;
        // 他のボタンを再び有効化
        volumeButton.interactable = true;
        titleButton.interactable = true;

        // 音量調節ボタンを再び選択
        EventSystem.current.SetSelectedGameObject(volumeButton.gameObject);
    }
}
