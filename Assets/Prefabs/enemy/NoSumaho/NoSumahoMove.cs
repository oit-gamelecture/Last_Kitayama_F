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
    private bool hasAvoided = true; // 回避行動を一回だけ行うためのフラグ

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
        if (player == null || isAvoiding || hasAvoided) return; // 既に回避した場合は終了

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        float fieldOfViewAngle = 45f;
        Vector3 directionToEnemy = (transform.position - player.transform.position).normalized;
        float angleToEnemy = Vector3.Angle(player.transform.forward, directionToEnemy);

        if (distanceToPlayer < avoidDistance && angleToEnemy < fieldOfViewAngle)
        {
            isAvoiding = true;
            hasAvoided = true; // 回避済みに設定

            Vector3 originalTarget = navMeshAgent.destination;

            Vector3 rightDirection = Vector3.Cross(Vector3.up, directionToEnemy).normalized;
            Vector3 leftDirection = Vector3.Cross(directionToEnemy, Vector3.up).normalized;

            Vector3 chosenDirection = (Vector3.Distance(player.transform.position, transform.position + rightDirection) >
                                       Vector3.Distance(player.transform.position, transform.position + leftDirection))
                                       ? rightDirection : leftDirection;

            Vector3 sideStepPosition = transform.position + chosenDirection;

            if (NavMesh.SamplePosition(sideStepPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
                StartCoroutine(ReturnToOriginalTarget(originalTarget));
            }
            else
            {
                isAvoiding = false;
            }
        }
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
