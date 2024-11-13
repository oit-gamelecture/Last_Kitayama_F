using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStopMovement : MonoBehaviour
{
    public Animator enemyAnimator;
    public float normalSpeed = 3.0f;
    public float retreatSpeed = 6.0f;
    public float detectionRadius = 10.0f;

    private bool isFalling = false;
    private bool isStopped = false;

    private NavMeshAgent navMeshAgent;
    private Vector3 currentTargetPosition;

    private Transform playerTransform;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();

        if (navMeshAgent == null)
        {
            Debug.LogError($"NavMeshAgent�� {gameObject.name} �Ɍ�����܂���B");
            return;
        }

        navMeshAgent.speed = normalSpeed;
        EnsureOnNavMesh();

        SetRandomizedTargetBasedOnHeight(); // �����Ɋ�Â������_���ȖڕW�n�_��ݒ�
        navMeshAgent.SetDestination(currentTargetPosition);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (isFalling || navMeshAgent == null || !navMeshAgent.isOnNavMesh) return;

        MonitorPlayerDistance();

        if (navMeshAgent.remainingDistance < 0.5f && !isStopped)
        {
            ToggleTargetPosition(); // ���̖ڕW�n�_�ɐ؂�ւ�
            navMeshAgent.SetDestination(currentTargetPosition);
        }
    }

    void MonitorPlayerDistance()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRadius)
        {
            StopMovement();
        }
        else
        {
            ResumeMovement();
        }
    }

    void StopMovement()
    {
        if (!isStopped)
        {
            isStopped = true;
            navMeshAgent.isStopped = true;
            enemyAnimator.SetBool("stop", true);
        }
    }

    void ResumeMovement()
    {
        if (isStopped)
        {
            isStopped = false;
            navMeshAgent.isStopped = false;
            enemyAnimator.SetBool("stop", false);
        }
    }

    void SetRandomizedTargetBasedOnHeight()
    {
        float yPosition = transform.position.y;

        if (yPosition >= 0)
        {
            // ���� 0 �ȏ�̏ꍇ
            currentTargetPosition = new Vector3(Random.Range(3f, -6f), 1, 20);
        }
        else if (yPosition >= -6 && yPosition < 0)
        {
            // ���� -6 �` 0 �̏ꍇ
            currentTargetPosition = new Vector3(0, -4, Random.Range(-106f, -113f));
        }
        else if (yPosition >= -11 && yPosition < -6)
        {
            // ���� -11 �` -6 �̏ꍇ
            currentTargetPosition = new Vector3(Random.Range(103.3f, 110f), -9.4f, -120);
        }
        else
        {
            // ���� -11 �ȉ��̏ꍇ
            currentTargetPosition = new Vector3(120, -14.4f, Random.Range(-20f, -13f));
        }
    }

    void ToggleTargetPosition()
    {
        SetRandomizedTargetBasedOnHeight();
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
