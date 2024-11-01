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

    [SerializeField] private List<Vector3> targetPositions;
    private Vector3 initialTargetPosition; // 初期の目標座標

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

        EnsureOnNavMesh();
        SetInitialTarget(); // 初期の目標座標を設定
    }

    void Update()
    {
        // isFallingがfalseで、NavMeshAgentがNavMesh上にいる場合のみ移動を続ける
        if (!isFalling && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            if (navMeshAgent.remainingDistance < 0.5f)
            {
                navMeshAgent.SetDestination(initialTargetPosition); // 最初の目標に向かい続ける
            }
        }
    }

    // 初期の目標座標を設定
    void SetInitialTarget()
    {
        float yPosition = transform.position.y;
        if (yPosition >= 0)
        {
            initialTargetPosition = Random.value < 0.5f
                ? new Vector3(Random.Range(3f, -1f), 1, 20)
                : new Vector3(Random.Range(-6f, -2f), 1, -140);
        }
        else if (yPosition >= -6 && yPosition < 0)
        {
            initialTargetPosition = Random.value < 0.5f
                ? new Vector3(0, -4, Random.Range(-109f, -106f))
                : new Vector3(130, -4, Random.Range(-110f, -113f));
        }
        else if (yPosition >= -11 && yPosition < -6)
        {
            initialTargetPosition = Random.value < 0.5f
                ? new Vector3(Random.Range(103.3f, 106.3f), -9.4f, -120)
                : new Vector3(Random.Range(107.3f, 110.3f), -9.4f, 10);
        }
        else
        {
            initialTargetPosition = Random.value < 0.5f
                ? new Vector3(120, -14.4f, Random.Range(-20f, -17f))
                : new Vector3(-10, -14.3f, Random.Range(-13f, -16f));
        }

        navMeshAgent.SetDestination(initialTargetPosition);
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
