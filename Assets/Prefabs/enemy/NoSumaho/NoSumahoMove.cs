using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NoSumahoMove : MonoBehaviour
{
    public Animator enemyAnimator;
    public float speed = 3.0f;
    public float downSpeed = 6.0f;
    private float retreatDuration = 0.5f;
    private bool isFalling = false;
    private bool isAvoiding = false;
    private bool hasAvoided = false; // 回避行動を一回だけ行うためのフラグ

    private NavMeshAgent navMeshAgent;
    private BoxCollider boxCol;
    private Vector3 currentTargetPosition;

    public float avoidDistance = 10.0f;
    private GameObject player;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCol = GetComponent<BoxCollider>();
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgentが見つかりません");
            return;
        }

        navMeshAgent.speed = speed;
        enemyAnimator.SetBool("IsWalking", true);

        EnsureOnNavMesh();
        SetRandomizedTargetBasedOnHeight(); // 高さに応じたランダムな目標地点を設定
        navMeshAgent.SetDestination(currentTargetPosition);
    }

    void Update()
    {
        if (isFalling || navMeshAgent == null || !navMeshAgent.isOnNavMesh) return;

        AvoidPlayer(); // プレイヤーの回避処理

        if (navMeshAgent.remainingDistance < 0.5f)
        {
            ToggleTargetPosition();
            navMeshAgent.SetDestination(currentTargetPosition);
        }
    }

    void SetRandomizedTargetBasedOnHeight()
    {
        float yPosition = transform.position.y;

        if (yPosition >= 0)
        {
            // 高さ 0 以上の場合
            currentTargetPosition = new Vector3(Random.Range(3f, -6f), 1, 20);
        }
        else if (yPosition >= -6 && yPosition < 0)
        {
            // 高さ -6 ～ 0 の場合
            currentTargetPosition = new Vector3(0, -4, Random.Range(-109f, -106f));
        }
        else if (yPosition >= -11 && yPosition < -6)
        {
            // 高さ -11 ～ -6 の場合
            currentTargetPosition = new Vector3(Random.Range(103.3f, 110f), -9.4f, -120);
        }
        else
        {
            // 高さ -11 以下の場合
            currentTargetPosition = new Vector3(120, -14.4f, Random.Range(-20f, -13f));
        }
    }

    void ToggleTargetPosition()
    {
        SetRandomizedTargetBasedOnHeight();
    }

    void AvoidPlayer()
    {
        if (player == null || isAvoiding || hasAvoided) return; // 既に回避済みの場合は終了

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer >= avoidDistance) return; // 避ける必要がない場合終了

        // プレイヤーの進行方向を取得
        Vector3 playerForward = player.transform.forward.normalized;

        // プレイヤーの左右方向を計算
        Vector3 rightDirection = Vector3.Cross(Vector3.up, playerForward).normalized;
        Vector3 leftDirection = -rightDirection;

        // 右側と左側のNavMesh上の距離を計測
        Vector3 rightPosition = transform.position + rightDirection * 2.0f;
        Vector3 leftPosition = transform.position + leftDirection * 2.0f;

        float rightDistance = (NavMesh.SamplePosition(rightPosition, out NavMeshHit rightHit, 2.0f, NavMesh.AllAreas))
                              ? Vector3.Distance(player.transform.position, rightHit.position)
                              : 0;

        float leftDistance = (NavMesh.SamplePosition(leftPosition, out NavMeshHit leftHit, 2.0f, NavMesh.AllAreas))
                             ? Vector3.Distance(player.transform.position, leftHit.position)
                             : 0;

        // プレイヤーから遠い方向を選択
        Vector3 chosenPosition;
        if (rightDistance > leftDistance && rightDistance > 0)
        {
            chosenPosition = rightHit.position;
        }
        else if (leftDistance > 0)
        {
            chosenPosition = leftHit.position;
        }
        else
        {
            // 両方向が無効の場合は回避しない
            return;
        }

        // 回避フラグを設定して回避
        isAvoiding = true;
        hasAvoided = true;

        navMeshAgent.SetDestination(chosenPosition);

        // 元の目標に戻る処理を開始
        StartCoroutine(ReturnToOriginalTarget(currentTargetPosition));
    }



    IEnumerator ReturnToOriginalTarget(Vector3 originalTarget)
    {
        while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.5f)
        {
            if (!navMeshAgent.isOnNavMesh)
            {
                isAvoiding = false;
                yield break;
            }
            yield return null;
        }

        navMeshAgent.SetDestination(originalTarget);
        isAvoiding = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            isFalling = true;
            StartCoroutine(Down(collision.transform));
        }
    }

    IEnumerator Down(Transform playerTransform)
    {
        enemyAnimator.SetBool("IsWalking", false);
        enemyAnimator.SetTrigger("Fall");

        navMeshAgent.isStopped = true;
        boxCol.enabled = false;

        Vector3 retreatDirection = (transform.position - playerTransform.position).normalized;
        float elapsedTime = 0f;

        while (elapsedTime < retreatDuration)
        {
            transform.Translate(retreatDirection * downSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        navMeshAgent.enabled = false;
    }

    void EnsureOnNavMesh()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
    }
}