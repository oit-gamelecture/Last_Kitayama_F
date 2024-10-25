using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMesh を使用するため

public class NoSumahoMove : MonoBehaviour
{
    public Animator enemyAnimator;  // 敵のアニメーションコントローラー
    public float speed = 3.0f;      // 通常移動速度
    public float downSpeed = 6.0f;  // 転倒時の後退速度
    private float retreatDuration = 0.5f; // 後退時間

    private NavMeshAgent navMeshAgent; // NavMeshAgent の参照
    private BoxCollider boxCol;        // 当たり判定用の BoxCollider
    private bool isFalling = false;    // 転倒フラグ

    [SerializeField] private List<Vector3> targetPositions; // 目的地のリスト
    private int currentTargetIndex = 0; // 現在の目標座標のインデックス

    void Start()
    {
        // コンポーネントの取得
        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCol = GetComponent<BoxCollider>();
        enemyAnimator = GetComponent<Animator>();

        if (navMeshAgent == null)
        {
            Debug.LogError($"{gameObject.name} に NavMeshAgent が見つかりません。");
            return;
        }

        if (targetPositions == null || targetPositions.Count == 0)
        {
            Debug.LogError("ターゲット座標が設定されていません。");
            return;
        }

        navMeshAgent.speed = speed;
        enemyAnimator.SetBool("IsWalking", true); // 歩くアニメーションを開始

        EnsureOnNavMesh(); // NavMesh 上の有効な位置に移動する
        SetNextTarget();   // 最初の目標座標を設定
    }

    void Update()
    {
        // 転倒している場合は移動を停止
        if (isFalling || navMeshAgent == null || !navMeshAgent.isOnNavMesh)
            return;

        if (navMeshAgent.remainingDistance < 0.5f)
        {
            AdvanceToNextTarget(); // 次の座標に移動
        }
    }

    void SetNextTarget()
    {
        if (!navMeshAgent.isOnNavMesh) return;

        Vector3 targetPosition = Vector3.zero;
        float yPosition = transform.position.y;

        if (yPosition >= 0)
        {
            if (Random.value < 0.5f)
            {
                float randomX = Random.Range(3f, -1f); // x座標をランダムに
                targetPosition = new Vector3(randomX, 1, 20);
            }
            else
            {
                float randomX = Random.Range(-6f, -2f); // x座標をランダムに
                targetPosition = new Vector3(randomX, 1, -140);
            }
        }
        else if (yPosition >= -6 && yPosition < 0)
        {
            if (Random.value < 0.5f)
            {
                float randomZ = Random.Range(-109f, -106f); // z座標をランダムに
                targetPosition = new Vector3(0, -4, randomZ);
            }
            else
            {
                float randomZ = Random.Range(-110f, -113f); // z座標をランダムに
                targetPosition = new Vector3(130, -4, randomZ);
            }
        }
        else if (yPosition >= -11 && yPosition < -6)
        {
            if (Random.value < 0.5f)
            {
                float randomX = Random.Range(103.3f, 106.3f); // x座標をランダムに
                targetPosition = new Vector3(randomX, -9.4f, -120);
            }
            else
            {
                float randomX = Random.Range(107.3f, 110.3f); // x座標をランダムに
                targetPosition = new Vector3(randomX, -9.4f, 10);
            }
        }
        else
        {
            if (Random.value < 0.5f)
            {
                float randomZ = Random.Range(-20f, -17f); // z座標をランダムに
                targetPosition = new Vector3(120, -14.4f, randomZ);
            }
            else
            {
                float randomZ = Random.Range(-13f, -16f); // z座標をランダムに
                targetPosition = new Vector3(-10, -14.3f, randomZ);
            }
        }

        navMeshAgent.SetDestination(targetPosition);
    }

    void AdvanceToNextTarget()
    {
        if (targetPositions.Count == 0) return;

        currentTargetIndex = (currentTargetIndex + 1) % targetPositions.Count;
        SetNextTarget();
    }

    // プレイヤーとの物理衝突時の処理
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            isFalling = true; // 転倒フラグを立てる
            Debug.Log("fall");
            StartCoroutine(Down(collision.transform)); // 転倒処理の開始
        }
    }

    // 転倒処理のコルーチン
    IEnumerator Down(Transform playerTransform)
    {
        enemyAnimator.SetBool("IsWalking", false); // 歩くアニメーション停止
        enemyAnimator.SetTrigger("Fall");          // 転倒アニメーション再生

        navMeshAgent.isStopped = true; // NavMeshAgent を停止
        boxCol.enabled = false;        // 当たり判定を無効化

        Vector3 retreatDirection = (transform.position - playerTransform.position).normalized;

        float elapsedTime = 0f;

        // プレイヤーから後退する処理
        while (elapsedTime < retreatDuration)
        {
            transform.Translate(retreatDirection * downSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 後退終了後、完全停止
        navMeshAgent.enabled = false; // NavMeshAgent を無効化
        Debug.Log("Enemy stopped.");
    }

    // NavMesh 上にいない場合、近くの有効な位置に移動する
    void EnsureOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            //Debug.Log($"{gameObject.name} を NavMesh 上に移動しました。");
        }
        else
        {
            //Debug.LogError($"{gameObject.name} の近くに NavMesh 上の有効な地点が見つかりませんでした。");
        }
    }
}
