using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMesh ���g�p���邽��

public class NoSumahoMove : MonoBehaviour
{
    public Animator enemyAnimator;  // �G�̃A�j���[�V�����R���g���[���[
    public float speed = 3.0f;      // �ʏ�ړ����x
    public float downSpeed = 8.0f;  // �]�|���̌�ޑ��x
    private float retreatDuration = 0.5f; // ��ގ���

    private NavMeshAgent navMeshAgent; // NavMeshAgent �̎Q��
    private BoxCollider boxCol;        // �����蔻��p�� BoxCollider
    private bool isFalling = false;    // �]�|�t���O

    [SerializeField] private List<Vector3> targetPositions; // �ړI�n�̃��X�g
    private int currentTargetIndex = 0; // ���݂̖ڕW���W�̃C���f�b�N�X

    void Start()
    {
        // �R���|�[�l���g�̎擾
        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCol = GetComponent<BoxCollider>();
        enemyAnimator = GetComponent<Animator>();

        if (navMeshAgent == null)
        {
            Debug.LogError($"{gameObject.name} �� NavMeshAgent ��������܂���B");
            return;
        }

        if (targetPositions == null || targetPositions.Count == 0)
        {
            Debug.LogError("�^�[�Q�b�g���W���ݒ肳��Ă��܂���B");
            return;
        }

        navMeshAgent.speed = speed;
        enemyAnimator.SetBool("IsWalking", true); // �����A�j���[�V�������J�n

        EnsureOnNavMesh(); // NavMesh ��̗L���Ȉʒu�Ɉړ�����
        SetNextTarget();   // �ŏ��̖ڕW���W��ݒ�
    }

    void Update()
    {
        // �]�|���Ă���ꍇ�͈ړ����~
        if (isFalling || navMeshAgent == null || !navMeshAgent.isOnNavMesh)
            return;

        if (navMeshAgent.remainingDistance < 0.5f)
        {
            AdvanceToNextTarget(); // ���̍��W�Ɉړ�
        }
    }

    void SetNextTarget()
    {
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(targetPositions[currentTargetIndex]);
        }
    }

    void AdvanceToNextTarget()
    {
        if (targetPositions.Count == 0) return;

        currentTargetIndex = (currentTargetIndex + 1) % targetPositions.Count;
        SetNextTarget();
    }

    // �v���C���[�Ƃ̕����Փˎ��̏���
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            isFalling = true; // �]�|�t���O�𗧂Ă�
            Debug.Log("fall");
            StartCoroutine(Down(collision.transform)); // �]�|�����̊J�n
        }
    }

    // �]�|�����̃R���[�`��
    IEnumerator Down(Transform playerTransform)
    {
        enemyAnimator.SetBool("IsWalking", false); // �����A�j���[�V������~
        enemyAnimator.SetTrigger("Fall");          // �]�|�A�j���[�V�����Đ�

        navMeshAgent.isStopped = true; // NavMeshAgent ���~
        boxCol.enabled = false;        // �����蔻��𖳌���

        Vector3 retreatDirection = (transform.position - playerTransform.position).normalized;

        float elapsedTime = 0f;

        // �v���C���[�����ނ��鏈��
        while (elapsedTime < retreatDuration)
        {
            transform.Translate(retreatDirection * downSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��ޏI����A���S��~
        navMeshAgent.enabled = false; // NavMeshAgent �𖳌���
        Debug.Log("Enemy stopped.");
    }

    // NavMesh ��ɂ��Ȃ��ꍇ�A�߂��̗L���Ȉʒu�Ɉړ�����
    void EnsureOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            Debug.Log($"{gameObject.name} �� NavMesh ��Ɉړ����܂����B");
        }
        else
        {
            Debug.LogError($"{gameObject.name} �̋߂��� NavMesh ��̗L���Ȓn�_��������܂���ł����B");
        }
    }
}
