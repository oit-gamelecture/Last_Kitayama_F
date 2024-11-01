using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Animator enemyAnimator;
    public float normalSpeed = 3.0f;
    public float retreatSpeed = 6.0f;
    private bool isFalling = false;

    private NavMeshAgent navMeshAgent;
    private Vector3 targetPositionA;
    private Vector3 targetPositionB;
    private Vector3 currentTargetPosition;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError($"NavMeshAgentが {gameObject.name} に見つかりません。");
            return;
        }

        enemyAnimator = GetComponent<Animator>();
        navMeshAgent.speed = normalSpeed;

        EnsureOnNavMesh();
        SetRandomTargetsBasedOnHeight(); // 高さに応じた目標地点を設定
        navMeshAgent.SetDestination(currentTargetPosition);
    }

    void Update()
    {
        if (!isFalling && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            if (navMeshAgent.remainingDistance < 0.5f)
            {
                ToggleTargetPosition();
                navMeshAgent.SetDestination(currentTargetPosition);
            }
        }
    }

    void SetRandomTargetsBasedOnHeight()
    {
        float yPosition = transform.position.y;
        if (yPosition >= 0)
        {
            targetPositionA = new Vector3(Random.Range(3f, -1f), 1, 20);
            targetPositionB = new Vector3(Random.Range(-6f, -2f), 1, -140);
        }
        else if (yPosition >= -6 && yPosition < 0)
        {
            targetPositionA = new Vector3(0, -4, Random.Range(-109f, -106f));
            targetPositionB = new Vector3(130, -4, Random.Range(-110f, -113f));
        }
        else if (yPosition >= -11 && yPosition < -6)
        {
            targetPositionA = new Vector3(Random.Range(103.3f, 106.3f), -9.4f, -120);
            targetPositionB = new Vector3(Random.Range(107.3f, 110.3f), -9.4f, 10);
        }
        else
        {
            targetPositionA = new Vector3(120, -14.4f, Random.Range(-20f, -17f));
            targetPositionB = new Vector3(-10, -14.3f, Random.Range(-13f, -16f));
        }

        currentTargetPosition = Random.value < 0.5f ? targetPositionA : targetPositionB;
    }

    void ToggleTargetPosition()
    {
        currentTargetPosition = currentTargetPosition == targetPositionA ? targetPositionB : targetPositionA;
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
        }
    }
}
