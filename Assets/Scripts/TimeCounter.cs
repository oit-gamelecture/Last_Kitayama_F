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


    //タイマー非表示
    private bool countStart = false;

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
            //時間をカウントダウンする
            countdown -= Time.deltaTime;

            //時間を表示する
            timeText.text = countdown.ToString("f2");

            // 警告状態のチェック
            if (countdown <= 20f && !isWarning)
            {
                isWarning = true;
                StartCoroutine(WarningEffect());
                StartCoroutine(ShakeImage());
            }
        }

        //countdownが0以下になったとき
        if (countdown <= 0)
        {
            countdown = 0;
            countStart = false;
            SceneManager.LoadScene("over scene");
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
}