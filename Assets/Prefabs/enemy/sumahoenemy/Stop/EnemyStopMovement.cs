using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent���g�p���邽��

public class EnemyStopMovement : MonoBehaviour
{
    public Animator enemyAnimator; // �G�̃A�j���[�V�����R���g���[���[
    public float normalSpeed = 3.0f; // �ʏ�ړ����x
    public float retreatSpeed = 8.0f; // �]�|��̍�����ޑ��x
    private bool isFalling = false; // �]�|�t���O
    private NavMeshAgent navMeshAgent; // NavMeshAgent �̎Q��

    [SerializeField] private List<Vector3> targetPositions; // �ړI�n�̍��W���X�g
    private int currentTargetIndex = 0; // ���݂̖ڕW���W�̃C���f�b�N�X

    public float detectionRadius = 10.0f; // �v���C���[�Ƃ̋������Ď����锼�a
    private Transform playerTransform; // Player�I�u�W�F�N�g��Transform�Q��

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

        // �v���C���[��Transform���擾
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (isFalling || navMeshAgent == null || !navMeshAgent.isOnNavMesh) return;

        // �v���C���[�Ƃ̋�������ɊĎ�
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= detectionRadius)
            {
                StopMovement(); // �v���C���[���߂��ꍇ�A�ړ���~
            }
            else
            {
                ResumeMovement(); // �v���C���[�����ꂽ�ꍇ�A�ړ��ĊJ
            }
        }

        // �ړI�n�ւ̈ړ�����
        if (navMeshAgent.remainingDistance < 0.5f && !navMeshAgent.isStopped)
        {
            AdvanceToNextTarget(); // ���̖ڕW���W�Ɉړ�
        }
    }

    void StopMovement()
    {
        navMeshAgent.isStopped = true; // �ړ����~
        enemyAnimator.SetBool("stop", true); // stop�A�j���[�V�����ɑJ��
    }

    void ResumeMovement()
    {
        navMeshAgent.isStopped = false; // �ړ����ĊJ
        enemyAnimator.SetBool("stop", false); // stop�A�j���[�V����������
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
            //Debug.Log($"{gameObject.name} �� NavMesh��Ɉړ����܂����B");
        }
        else
        {
            //Debug.LogError($"{gameObject.name} �̋߂��� NavMesh��̗L���Ȓn�_��������܂���ł����B");
        }
    }
}
