using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class reSumaho : MonoBehaviour
{
    public Animator enemyAnimator;
    public float normalSpeed = 3.0f;
    public float retreatSpeed = 6.0f;
    private bool isFalling = false;

    private NavMeshAgent navMeshAgent;

    [SerializeField] private Transform playerTarget; // プレイヤーの位置を参照
    private Vector3 initialTargetPosition; // 初期の目標座標

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

    // 初期の目標座標を設定（プレイヤーと反対方向）
    void SetInitialTarget()
    {
        Vector3 directionAwayFromPlayer = (transform.position - playerTarget.position).normalized; // プレイヤーから離れる方向
        float distance = 20f; // 目的地までの距離（調整可能）

        initialTargetPosition = transform.position + directionAwayFromPlayer * distance; // プレイヤーから反対方向に距離を取る

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
