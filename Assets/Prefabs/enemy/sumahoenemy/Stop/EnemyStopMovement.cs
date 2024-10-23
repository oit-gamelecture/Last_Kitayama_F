using System.Collections;
using UnityEngine;

public class EnemyStopMovement : MonoBehaviour
{
    public Animator enemyAnimator;  // 敵のアニメーションコントローラー
    public float speed = 3.0f;      // 移動速度
    public float stopDistance = -10.0f; // プレイヤーの手前で停止する距離
    public float downspeed = -2.0f;  // 転倒時の後退速度

    private Transform player;       // プレイヤーのTransform
    private bool isStopped = false; // 停止フラグ
    private bool isFalling = false; // 転倒フラグ

    void Start()
    {
        // プレイヤーのTransformを取得
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // アニメーションコントローラーの取得
        enemyAnimator = GetComponent<Animator>();

        // 歩くアニメーションを開始
        enemyAnimator.SetBool("IsWalking", true);
    }

    void Update()
    {
        if (!isFalling) // 転倒中でない場合のみ移動する
        {
            MoveTowardsPlayer(); // プレイヤーに向かって移動する処理
        }
    }

    // プレイヤーに向かって移動する処理
    void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            // プレイヤーに向かって移動
            Vector3 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
            enemyAnimator.SetBool("IsWalking", true); // 歩くアニメーション再生
        }
        else
        {
            // 停止アニメーションに遷移
            StopAndPlayAnimation();
        }
    }

    // 停止アニメーションの処理
    void StopAndPlayAnimation()
    {
        if (!isStopped)
        {
            isStopped = true; // 停止フラグを立てる
            enemyAnimator.SetBool("IsWalking", false); // 歩くアニメーション停止
            enemyAnimator.SetTrigger("stop"); // Stopアニメーション再生
        }
    }

    // プレイヤーとの衝突時の処理
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            StartCoroutine(Down()); // 転倒処理の開始
        }
    }

    // 転倒処理
    IEnumerator Down()
    {
        isFalling = true; // 転倒フラグを立てる
        enemyAnimator.SetBool("IsWalking", false); // 歩くアニメーションを停止
        enemyAnimator.SetTrigger("Fall"); // 転倒アニメーション再生

        float elapsedTime = 0f;
        float retreatDuration = 0.8f; // 転倒時の後退時間

        // 高速で後退する処理
        while (elapsedTime < retreatDuration)
        {
            transform.Translate(Vector3.back * downspeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        downspeed = 0; // 後退を停止

        yield return new WaitForSeconds(5.0f); // 5秒後にオブジェクトを削除
        Destroy(gameObject); // オブジェクトを削除
    }
}
