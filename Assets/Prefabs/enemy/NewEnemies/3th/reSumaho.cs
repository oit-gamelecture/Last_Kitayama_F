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

    [SerializeField] private Transform playerTarget; // �v���C���[�̈ʒu���Q��
    private Vector3 initialTargetPosition; // �����̖ڕW���W

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError($"NavMeshAgent�� {gameObject.name} �Ɍ�����܂���B");
            return;
        }

        enemyAnimator = GetComponent<Animator>();
        navMeshAgent.speed = normalSpeed;

        EnsureOnNavMesh();
        SetInitialTarget(); // �����̖ڕW���W��ݒ�
    }

    void Update()
    {
        // isFalling��false�ŁANavMeshAgent��NavMesh��ɂ���ꍇ�݈̂ړ��𑱂���
        if (!isFalling && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            if (navMeshAgent.remainingDistance < 0.5f)
            {
                navMeshAgent.SetDestination(initialTargetPosition); // �ŏ��̖ڕW�Ɍ�����������
            }
        }
    }

    // �����̖ڕW���W��ݒ�i�v���C���[�Ɣ��Ε����j
    void SetInitialTarget()
    {
        Vector3 directionAwayFromPlayer = (transform.position - playerTarget.position).normalized; // �v���C���[���痣������
        float distance = 20f; // �ړI�n�܂ł̋����i�����\�j

        initialTargetPosition = transform.position + directionAwayFromPlayer * distance; // �v���C���[���甽�Ε����ɋ��������

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
