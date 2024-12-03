using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliderTime : MonoBehaviour
{
    public Slider meterSlider; // メーターに対応するSlider
    public float maxTime = 30f; // メーターの最大時間
    private float currentTime = 0f; // 現在の時間
    private float delayTime = 3f; // 最初に待つ時間
    private bool isDelayOver = false; // 待機時間が終了したかを管理するフラグ

    [SerializeField]
    private GameObject player; // プレイヤーオブジェクトの参照 (シリアライズ)

    private PlayerMovement playerMovement; // PlayerMovementスクリプトの参照
    private bool isPausedAfterFalling = false; // 転倒復帰後の待機中フラグ

    void Start()
    {
        if (meterSlider != null)
        {
            meterSlider.maxValue = maxTime; // スライダーの最大値を設定
            meterSlider.value = 0; // 初期値を0に設定
        }

        // PlayerMovementスクリプトを取得
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(delayTime); // 指定秒数待機
        isDelayOver = true; // 待機終了フラグを設定
    }

    void Update()
    {
        // プレイヤーが転倒中ならスライダーの進行を停止
        if (playerMovement != null && playerMovement.isFalling)
        {
            Debug.Log("プレイヤー転倒中: スライダー停止");
            return;
        }

        // 転倒復帰後の0.5秒間はスライダーを停止
        if (isPausedAfterFalling)
        {
            Debug.Log("転倒復帰後の待機中...");
            return;
        }

        if (isDelayOver && currentTime < maxTime)
        {
            currentTime += Time.deltaTime; // 時間を加算
            if (meterSlider != null)
            {
                meterSlider.value = currentTime; // メーターを更新
            }
        }
        else if (!isDelayOver)
        {
            Debug.Log("待機中...");
        }
        else
        {
            Debug.Log("タイマーが最大に達しました！");
        }
    }

    // 転倒復帰後にスライダーを停止する処理を呼び出す
    public void HandlePostFallPause()
    {
        StartCoroutine(PostFallPauseCoroutine());
    }

    IEnumerator PostFallPauseCoroutine()
    {
        isPausedAfterFalling = true; // 転倒復帰後の待機開始
        yield return new WaitForSeconds(0.5f); // 0.5秒待機
        isPausedAfterFalling = false; // 待機終了
    }
}
