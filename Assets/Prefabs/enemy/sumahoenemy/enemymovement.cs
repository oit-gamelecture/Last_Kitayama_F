using System.Collections;
using UnityEngine;

public class enemymovement : MonoBehaviour
{
    public Animator enemyanimator;  // 敵のアニメーションコントローラー
    public float speed = 3.0f;      // 通常の移動速度
    public float downspeed = -2.0f; // 転倒時の後退速度（高速化）

    [SerializeField] private float moveDirectionZ = 1.0f; // Z軸の移動方向 (1: 前進, -1: 後退)
    private BoxCollider boxCol;      // 当たり判定用のBoxCollider
    private bool isFalling = false;  // 転倒フラグ

    void Start()
    {
        boxCol = GetComponent<BoxCollider>();           // BoxColliderの取得
        enemyanimator = GetComponent<Animator>();        // Animatorの取得
    }

    void FixedUpdate()
    {
        if (!isFalling)
        {
            MoveEnemy(); // 通常の移動処理
        }
    }

    // 敵の移動処理
    void MoveEnemy()
    {
        Vector3 movement = new Vector3(0, 0, moveDirectionZ) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    // 転倒時の処理（アニメーションの即時切り替えと後退）
    IEnumerator Down()
    {
        enemyanimator.SetBool("IsWalking", false); // 歩くアニメーション停止
        enemyanimator.SetTrigger("Fall"); // 転倒アニメーションを再生
        boxCol.enabled = false; // 当たり判定を無効化

        float elapsedTime = 0f;
        float retreatDuration = 0.5f; // 後退する時間を少し長く調整
        float fastRetreatSpeed = 8.0f; // 瞬間的な高速後退速度

        // 高速で短時間後退する処理
        while (elapsedTime < retreatDuration)
        {
            transform.Translate(Vector3.back * fastRetreatSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 後退を停止
        downspeed = 0;

    }

    // プレイヤーとの接触時の処理
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFalling)
        {
            isFalling = true; // 転倒フラグを立てる
            Debug.Log("fall");
            StartCoroutine(Down()); // 転倒処理の開始
        }
    }
}
