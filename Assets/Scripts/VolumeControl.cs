using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Button volumeButton;      // 音量調節ボタン
    [SerializeField] private Slider volumeSlider;      // 音量スライダー

    private void Start()
    {
        // 初期設定
        volumeSlider.value = AudioListener.volume;

        volumeButton.Select(); // 初期選択を音量調節ボタンに
        volumeSlider.gameObject.SetActive(true); // スライダーを常に表示
    }

    private void Update()
    {
        // EscキーでTitleシーンに遷移
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title");
        }

        // AキーとDキーでスライダーの値を調整
        if (Input.GetKey(KeyCode.D))
        {
            volumeSlider.value = Mathf.Clamp(volumeSlider.value + 0.005f, 0, 1);
            AudioListener.volume = volumeSlider.value; // 音量を更新
        }
        else if (Input.GetKey(KeyCode.A))
        {
            volumeSlider.value = Mathf.Clamp(volumeSlider.value - 0.005f, 0, 1);
            AudioListener.volume = volumeSlider.value; // 音量を更新
        }

        // Enterキーでボタン選択に戻る
        if (Input.GetKeyDown(KeyCode.Return))
        {
            volumeButton.Select(); // 音量ボタンにカーソルを戻す
        }
    }
}
