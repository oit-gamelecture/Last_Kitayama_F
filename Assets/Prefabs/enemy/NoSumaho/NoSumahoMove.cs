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
    private bool hasAvoided = true; // ����s������񂾂��s�����߂̃t���O

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
            Debug.LogError("NavMeshAgent��������܂���");
            return;
        }

        navMeshAgent.speed = speed;
        enemyAnimator.SetBool("IsWalking", true);

        EnsureOnNavMesh();
        SetRandomizedTargetBasedOnHeight(); // �����ɉ����������_���ȖڕW�n�_��ݒ�
        navMeshAgent.SetDestination(currentTargetPosition);
    }

    void Update()
    {
        if (isFalling || navMeshAgent == null || !navMeshAgent.isOnNavMesh) return;

        AvoidPlayer(); // �v���C���[�̉������

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
            // ���� 0 �ȏ�̏ꍇ
            currentTargetPosition = new Vector3(Random.Range(3f, -6f), 1, 20);
        }
        else if (yPosition >= -6 && yPosition < 0)
        {
            // ���� -6 �` 0 �̏ꍇ
            currentTargetPosition = new Vector3(0, -4, Random.Range(-109f, -106f));
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

    void AvoidPlayer()
    {
        if (player == null || isAvoiding || hasAvoided) return; // ���ɉ�������ꍇ�͏I��

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        float fieldOfViewAngle = 45f;
        Vector3 directionToEnemy = (transform.position - player.transform.position).normalized;
        float angleToEnemy = Vector3.Angle(player.transform.forward, directionToEnemy);

        if (distanceToPlayer < avoidDistance && angleToEnemy < fieldOfViewAngle)
        {
            isAvoiding = true;
            hasAvoided = true; // ����ς݂ɐݒ�

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
