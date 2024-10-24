using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgentを使用するため

public class EnemyStopMovement : MonoBehaviour
{
    public Animator enemyAnimator; // 敵のアニメーションコントローラー
    public float normalSpeed = 3.0f; // 通常移動速度
    public float retreatSpeed = 8.0f; // 転倒後の高速後退速度
    private bool isFalling = false; // 転倒フラグ
    private NavMeshAgent navMeshAgent; // NavMeshAgent の参照

    [SerializeField] private List<Vector3> targetPositions; // 目的地の座標リスト
    private int currentTargetIndex = 0; // 現在の目標座標のインデックス

    public float detectionRadius = 10.0f; // プレイヤーとの距離を監視する半径
    private Transform playerTransform; // PlayerオブジェクトのTransform参照

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError($"NavMeshAgentが {gameObject.name} に見つかりません。");
            return;
        }

        if (targetPositions == null || targetPositions.Count == 0)
        {
            Debug.LogError($"{gameObject.name} の targetPositions が設定されていません。");
            return;
        }

        enemyAnimator = GetComponent<Animator>();
        navMeshAgent.speed = normalSpeed;

        EnsureOnNavMesh(); // NavMesh上にいなければ NavMesh に移動
        SetNextTarget(); // 最初の目標座標を設定

        // プレイヤーのTransformを取得
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (isFalling || navMeshAgent == null || !navMeshAgent.isOnNavMesh) return;

        // プレイヤーとの距離を常に監視
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= detectionRadius)
            {
                StopMovement(); // プレイヤーが近い場合、移動停止
            }
            else
            {
                ResumeMovement(); // プレイヤーが離れた場合、移動再開
            }
        }

        // 目的地への移動処理
        if (navMeshAgent.remainingDistance < 0.5f && !navMeshAgent.isStopped)
        {
            AdvanceToNextTarget(); // 次の目標座標に移動
        }
    }

    void StopMovement()
    {
        navMeshAgent.isStopped = true; // 移動を停止
        enemyAnimator.SetBool("stop", true); // stopアニメーションに遷移
    }

    void ResumeMovement()
    {
        navMeshAgent.isStopped = false; // 移動を再開
        enemyAnimator.SetBool("stop", false); // stopアニメーションを解除
    }

    void AdvanceToNextTarget()
    {
        if (targetPositions.Count == 0) return;

        currentTargetIndex = (currentTargetIndex + 1) % targetPositions.Count;
        SetNextTarget();
    }

    void SetNextTarget()
    {
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(targetPositions[currentTargetIndex]);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            isFalling = true;
            StartCoroutine(HandleFall(collision.transform));
        }
    }

    IEnumerator HandleFall(Transform playerTransform)
    {
        enemyAnimator.SetBool("IsWalking", false);
        enemyAnimator.SetTrigger("Fall");
        navMeshAgent.isStopped = true;

        Vector3 retreatDirection = (transform.position - playerTransform.position).normalized;
        float elapsedTime = 0f;
        float retreatDuration = 0.5f;

        while (elapsedTime < retreatDuration)
        {
            transform.Translate(retreatDirection * retreatSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
    }

    void EnsureOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            Debug.Log($"{gameObject.name} を NavMesh上に移動しました。");
        }
        else
        {
            Debug.LogError($"{gameObject.name} の近くに NavMesh上の有効な地点が見つかりませんでした。");
        }
    }
}
