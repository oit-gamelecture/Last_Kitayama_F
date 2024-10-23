using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Animator enemyAnimator; // �G�̃A�j���[�V�����R���g���[���[
    public float normalSpeed = 3.0f; // �ʏ�ړ����x
    public float retreatSpeed = 8.0f; // �]�|��̍�����ޑ��x
    private bool isFalling = false; // �]�|�t���O

    private NavMeshAgent navMeshAgent; // NavMeshAgent �̎Q��

    // �V���A���C�Y���ꂽ�ړI�n�̍��W���X�g
    [SerializeField] private List<Vector3> targetPositions;
    private int currentTargetIndex = 0; // ���݂̖ڕW���W�̃C���f�b�N�X

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError($"NavMeshAgent�� {gameObject.name} �Ɍ�����܂���B");
            return;
        }

        if (targetPositions == null || targetPositions.Count == 0)
        {
            Debug.LogError($"{gameObject.name} �� targetPositions ���ݒ肳��Ă��܂���B");
            return;
        }

        enemyAnimator = GetComponent<Animator>();
        navMeshAgent.speed = normalSpeed;

        EnsureOnNavMesh(); // NavMesh��ɂ��Ȃ���� NavMesh �Ɉړ�
        SetNextTarget(); // �ŏ��̖ڕW���W��ݒ�
    }

    void Update()
    {
        if (!isFalling && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            if (navMeshAgent.remainingDistance < 0.5f)
            {
                AdvanceToNextTarget(); // ���̖ڕW���W�Ɉړ�
            }
        }
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
            Debug.Log($"{gameObject.name} �� NavMesh��Ɉړ����܂����B");
        }
        else
        {
            Debug.LogError($"{gameObject.name} �̋߂��� NavMesh��̗L���Ȓn�_��������܂���ł����B");
        }
    }
}
