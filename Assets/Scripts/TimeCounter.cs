using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    //カウントダウン
    public float countdown = 90.00f;
    public float countdownTime = 3f;
    public AudioClip countSound;
    // 時間警告のための変数
    private bool isWarning = false; // 警告状態フラグ
    private float scaleSpeed = 2.0f; // 拡大縮小速度
    private Vector3 originalScale; // 元のスケールを保存

    public Image warningImage; // 震えるイメージ
    private Vector2 originalImagePosition; // イメージの元の位置を保存

    AudioSource audioSource;

    //時間を表示するText型の変数
    public Text timeText;
    public Text warningText;


    //タイマー非表示
    private bool countStart = false;

    public AudioSource bgmSource; // BGM用のAudioSource
    public float fastForwardPitch = 1.5f; // 早送り時のピッチ
    private float normalPitch; // 通常時のピッチ
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(StartCountdown());
        timeText.enabled = false;

        audioSource = GetComponent<AudioSource>();
        StartCoroutine(StartCountdown());
        timeText.enabled = false;
        originalScale = timeText.transform.localScale; // 初期スケールを保存

        if (warningImage != null)
        {
            originalImagePosition = warningImage.rectTransform.anchoredPosition;
        }

        if (warningText != null)
        {
            warningText.enabled = false;
        }


    }

    //3秒後にタイマースタート
    IEnumerator StartCountdown()
    {
        audioSource.PlayOneShot(countSound);
        yield return new WaitForSeconds(countdownTime);
        countStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (countStart)
        {
            timeText.enabled = true;

            // 残り時間を減少させ、0より小さくならないようにする
            countdown -= Time.deltaTime;
            if (countdown < 0)
            {
                countdown = 0; // マイナス値を防ぐ
                countStart = false; // カウントダウンを停止
            }

            timeText.text = countdown.ToString("f2");

            if (countdown <= 20f && !isWarning)
            {
                isWarning = true;
                StartCoroutine(WarningEffect());
                StartCoroutine(ShakeImage());

                if (warningText != null)
                {
                    warningText.enabled = true;
                    StartCoroutine(WarningTextEffect());
                }

                // BGMの早送りを開始
                if (bgmSource != null)
                {
                    bgmSource.pitch = fastForwardPitch;
                }
            }

            if (countdown <= 0)
            {
                // 残り時間がゼロのとき、シーンを切り替える
                countdown = 0;
                countStart = false;

                // BGMのピッチをリセット
                if (bgmSource != null)
                {
                    bgmSource.pitch = normalPitch;
                }

                SceneManager.LoadScene("over scene");
            }
        }
    }


    public void SaveElapsedTime()
    {
        float elapsedTime = 90.00f - countdown;
        PlayerPrefs.SetFloat("ElapsedTime", elapsedTime);
    }

    public void StopCountdown()
    {
        countStart = false;
    }

    IEnumerator WarningEffect()
    {
        Color originalColor = timeText.color; // 初期カラーを保存
        Color warningColor = Color.red; // 警告カラー

        while (countdown > 0) // 残り時間が0になるまでループ
        {
            // 拡大 & 色変更
            for (float t = 0; t < 1; t += Time.deltaTime * scaleSpeed)
            {
                timeText.transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.5f, t);
                timeText.color = Color.Lerp(originalColor, warningColor, t); // 色を徐々に赤く
                yield return null;
            }
            // 縮小 & 色変更
            for (float t = 0; t < 1; t += Time.deltaTime * scaleSpeed)
            {
                timeText.transform.localScale = Vector3.Lerp(originalScale * 1.5f, originalScale, t);
                timeText.color = Color.Lerp(warningColor, originalColor, t); // 色を元に戻す
                yield return null;
            }
        }

        // 残り時間が0になったら色を元に戻す
        timeText.color = originalColor;
    }

    IEnumerator ShakeImage()
    {
        float shakeAmount = 360f; // 回転の振れ幅（角度）
        float shakeSpeed = 0.03f; // 振動のスピード（秒間隔）
        while (countdown > 0)
        {
            if (warningImage != null)
            {
                // ランダムな左右移動
                float randomAngle = Random.Range(-shakeAmount, shakeAmount);
                warningImage.rectTransform.localRotation = Quaternion.Euler(0, 0, randomAngle);

            }
            yield return new WaitForSeconds(shakeSpeed); // 震えのスピード
        }

        // 震え終了後に元の位置に戻す
        if (warningImage != null)
        {
            warningImage.rectTransform.localRotation = Quaternion.identity;
        }

    }

    IEnumerator WarningTextEffect()
    {
        Color originalColor = warningText.color; // 初期カラーを保存
        Color warningColor = Color.red; // 警告カラー
        Vector3 originalScale = warningText.transform.localScale; // 初期スケールを保存
        float scaleSpeed = 2.0f; // 拡大縮小速度

        while (countdown > 0) // 残り時間が0になるまでループ
        {
            // 拡大 & 色変更
            for (float t = 0; t < 1; t += Time.deltaTime * scaleSpeed)
            {
                warningText.transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.5f, t);
                warningText.color = Color.Lerp(originalColor, warningColor, t); // 色を赤く
                yield return null;
            }
            // 縮小 & 色変更
            for (float t = 0; t < 1; t += Time.deltaTime * scaleSpeed)
            {
                warningText.transform.localScale = Vector3.Lerp(originalScale * 1.5f, originalScale, t);
                warningText.color = Color.Lerp(warningColor, originalColor, t); // 色を元に戻す
                yield return null;
            }
        }

        // 残り時間が0になったら非表示
        warningText.enabled = false;
        warningText.color = originalColor;
    }
}