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
    public float avoidDistance = 20.0f; // プレイヤーを避ける距離

    private GameObject player; // Player オブジェクトの参照

    void Start()
    {
        // コンポーネントの取得
        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCol = GetComponent<BoxCollider>();
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

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

        AvoidPlayer(); // プレイヤーの回避処理

        if (navMeshAgent.remainingDistance < 0.5f)
        {
            AdvanceToNextTarget(); // 次の座標に移動
        }
    }

    private bool isAvoiding = false;      // 回避中フラグ
    private Vector3 originalTarget;       // 回避前の元の目的地

    void AvoidPlayer()
    {
        if (player == null || isAvoiding) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // プレイヤーの正面方向にいるかどうかを確認するための角度範囲
        float fieldOfViewAngle = 45f;
        Vector3 directionToEnemy = (transform.position - player.transform.position).normalized;
        float angleToEnemy = Vector3.Angle(player.transform.forward, directionToEnemy);

        // プレイヤーに近く、かつ視界角度内に敵がいる場合のみ回避動作を行う
        if (distanceToPlayer < avoidDistance && angleToEnemy < fieldOfViewAngle)
        {
            isAvoiding = true;
            originalTarget = navMeshAgent.destination;

            // プレイヤーの位置に対して遠ざかる横方向を計算
            Vector3 rightDirection = Vector3.Cross(Vector3.up, directionToEnemy).normalized;
            Vector3 leftDirection = Vector3.Cross(directionToEnemy, Vector3.up).normalized;

            // 右と左の候補位置を計算
            Vector3 rightPosition = transform.position + rightDirection * 1.0f;
            Vector3 leftPosition = transform.position + leftDirection * 1.0f;

            // プレイヤーからの距離が遠い方を選ぶ
            Vector3 chosenDirection = (Vector3.Distance(player.transform.position, rightPosition) >
                                       Vector3.Distance(player.transform.position, leftPosition))
                                       ? rightDirection : leftDirection;

            // 選択された方向に少しだけ移動する
            Vector3 sideStepPosition = transform.position + chosenDirection * 1.0f;

            // NavMesh 上で新しい位置が有効か確認して移動
            NavMeshHit hit;
            if (NavMesh.SamplePosition(sideStepPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
                Debug.Log("プレイヤーから遠ざかる方向へ回避中...");

                // 回避後に元の目標に戻るコルーチンを開始
                StartCoroutine(ReturnToOriginalTarget());
            }
            else
            {
                // 回避先が無効の場合、回避せず元の目標に戻る
                Debug.Log("回避先が NavMesh 上に存在しないため、回避を行いません。");
                isAvoiding = false; // 回避を行わずに終了
            }
        }
    }
    IEnumerator ReturnToOriginalTarget()
    {
        // NavMeshAgentが有効かつNavMesh上にいるかを確認
        if (navMeshAgent == null || !navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent が無効または NavMesh 上にいないため、元の目的地に戻る処理を中止します。");
            isAvoiding = false;
            yield break;
        }

        // 回避先に到達するまで待機
        while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.5f)
        {
            // NavMeshAgentが NavMesh 上にいなくなった場合は中止
            if (!navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning("NavMeshAgent が NavMesh 上にいないため、ReturnToOriginalTarget を中止します。");
                isAvoiding = false;
                yield break;
            }

            yield return null;
        }

        // 元の目的地に戻す
        navMeshAgent.SetDestination(originalTarget);
        isAvoiding = false;
    }



    void SetNextTarget()
    {
        if (!navMeshAgent.isOnNavMesh) return;

        Vector3 targetPosition = Vector3.zero;
        float yPosition = transform.position.y;

        if (yPosition >= 0)
        {
            float randomX = Random.value < 0.5f ? Random.Range(3f, -1f) : Random.Range(-6f, -2f);
            targetPosition = new Vector3(randomX, 1, Random.value < 0.5f ? 20 : -140);
        }
        else if (yPosition >= -6 && yPosition < 0)
        {
            float randomZ = Random.value < 0.5f ? Random.Range(-109f, -106f) : Random.Range(-110f, -113f);
            targetPosition = new Vector3(Random.value < 0.5f ? 0 : 130, -4, randomZ);
        }
        else if (yPosition >= -11 && yPosition < -6)
        {
            float randomX = Random.value < 0.5f ? Random.Range(103.3f, 106.3f) : Random.Range(107.3f, 110.3f);
            targetPosition = new Vector3(randomX, -9.4f, Random.value < 0.5f ? -120 : 10);
        }
        else
        {
            float randomZ = Random.value < 0.5f ? Random.Range(-20f, -17f) : Random.Range(-13f, -16f);
            targetPosition = new Vector3(Random.value < 0.5f ? 120 : -10, -14.4f, randomZ);
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
        }
        else
        {
            Debug.LogError($"{gameObject.name} の近くに NavMesh 上の有効な地点が見つかりませんでした。");
        }
    }
}