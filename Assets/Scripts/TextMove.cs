using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInTextWithActivation : MonoBehaviour
{
    public Text uiText; // 対象のUIテキスト
    public float fadeDuration = 2f; // フェードインにかかる時間
    public GameObject[] objectsToActivate; // 有効化するオブジェクト群
    public float activationDelay = 0.5f; // テキスト表示後の遅延時間

    private float fadeTimer = 0f; // フェード用タイマー
    private Color initialColor; // テキストの元の色
    private Vector3 initialScale; // 初期スケール
    public float maxScale = 1.2f; // 最大スケール

    private bool fadeComplete = false; // フェード完了フラグ

    private void Start()
    {
        if (uiText == null)
        {
            uiText = GetComponent<Text>();
        }

        // 初期状態の設定
        initialColor = uiText.color;
        uiText.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0); // アルファ値を0に
        initialScale = uiText.transform.localScale;
        uiText.transform.localScale = Vector3.zero; // 初期スケールを0に
    }

    private void Update()
    {
        if (!fadeComplete)
        {
            FadeInAndScale();
        }
    }

    private void FadeInAndScale()
    {
        if (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;

            // アルファ値を線形補間
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            uiText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            // スケールの変更
            float scale = Mathf.Lerp(0, maxScale, fadeTimer / fadeDuration);
            uiText.transform.localScale = initialScale * scale;
        }
        else if (!fadeComplete)
        {
            // フェードが完了したらフラグをセットし、遅延付きでオブジェクトを有効化
            fadeComplete = true;
            StartCoroutine(ActivateObjectsWithDelay());
        }
    }

    private IEnumerator ActivateObjectsWithDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true); // オブジェクトを有効化
            }
        }
    }
}
