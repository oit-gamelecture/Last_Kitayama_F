using System.Collections;
using UnityEngine;

public class enemymovement : MonoBehaviour
{
    public Animator enemyanimator;  // 敵のアニメーションコントローラー
    public float speed = 3.0f;      // 移動速度
    public float downspeed = -0.6f; // 転倒時の後退速度

    [SerializeField] private float moveDirectionZ = 1.0f; // Z軸の移動方向 (1: 前進, -1: 後退)
    private BoxCollider boxCol; // 当たり判定用のBoxCollider
    private bool isFalling = false; // 転倒フラグ

    void Start()
    {
        boxCol = GetComponent<BoxCollider>();           // BoxColliderの取得
        enemyanimator = GetComponent<Animator>();        // Animatorの取得
    }

    void FixedUpdate()
    {
        if (!isFalling)
        {
            MoveEnemy(); // 移動処理
        }
    }

    // 敵の移動処理
    void MoveEnemy()
    {
        Vector3 movement = new Vector3(0, 0, moveDirectionZ) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    // 転倒時の処理
    IEnumerator Down()
    {
        enemyanimator.SetTrigger("Fall"); // 転倒アニメーションを再生
        boxCol.enabled = false; // 当たり判定を無効化

        // 転倒後の後退
        transform.Translate(Vector3.back * downspeed * Time.deltaTime, Space.World);
        yield return new WaitForSeconds(0.8f);

        // 移動停止と消去処理
        downspeed = 0;
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject); // オブジェクトを削除
    }

    // プレイヤーとの接触時の処理
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            isFalling = true; // 転倒フラグを立てる
            enemyanimator.CrossFade("Fall", 0); // アニメーションの再生
            StartCoroutine(Down()); // 転倒処理の開始
            Debug.Log("Collision with player detected");
        }
    }
}
