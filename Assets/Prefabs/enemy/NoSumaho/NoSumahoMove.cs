using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMesh ���g�p���邽��

public class NoSumahoMove : MonoBehaviour
{
    public Animator enemyAnimator;  // �G�̃A�j���[�V�����R���g���[���[
    public float speed = 3.0f;      // �ʏ�ړ����x
    public float downSpeed = 6.0f;  // �]�|���̌�ޑ��x
    private float retreatDuration = 0.5f; // ��ގ���

    private NavMeshAgent navMeshAgent; // NavMeshAgent �̎Q��
    private BoxCollider boxCol;        // �����蔻��p�� BoxCollider
    private bool isFalling = false;    // �]�|�t���O

    [SerializeField] private List<Vector3> targetPositions; // �ړI�n�̃��X�g
    private int currentTargetIndex = 0; // ���݂̖ڕW���W�̃C���f�b�N�X
    public float avoidDistance = 20.0f; // �v���C���[������鋗��

    private GameObject player; // Player �I�u�W�F�N�g�̎Q��

    void Start()
    {
        // �R���|�[�l���g�̎擾
        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCol = GetComponent<BoxCollider>();
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

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

        AvoidPlayer(); // �v���C���[�̉������

        if (navMeshAgent.remainingDistance < 0.5f)
        {
            AdvanceToNextTarget(); // ���̍��W�Ɉړ�
        }
    }

    private bool isAvoiding = false;      // ��𒆃t���O
    private Vector3 originalTarget;       // ���O�̌��̖ړI�n

    void AvoidPlayer()
    {
        if (player == null || isAvoiding) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // �v���C���[�̐��ʕ����ɂ��邩�ǂ������m�F���邽�߂̊p�x�͈�
        float fieldOfViewAngle = 45f;
        Vector3 directionToEnemy = (transform.position - player.transform.position).normalized;
        float angleToEnemy = Vector3.Angle(player.transform.forward, directionToEnemy);

        // �v���C���[�ɋ߂��A�����E�p�x���ɓG������ꍇ�̂݉�𓮍���s��
        if (distanceToPlayer < avoidDistance && angleToEnemy < fieldOfViewAngle)
        {
            isAvoiding = true;
            originalTarget = navMeshAgent.destination;

            // �v���C���[�̈ʒu�ɑ΂��ĉ������鉡�������v�Z
            Vector3 rightDirection = Vector3.Cross(Vector3.up, directionToEnemy).normalized;
            Vector3 leftDirection = Vector3.Cross(directionToEnemy, Vector3.up).normalized;

            // �E�ƍ��̌��ʒu���v�Z
            Vector3 rightPosition = transform.position + rightDirection * 1.0f;
            Vector3 leftPosition = transform.position + leftDirection * 1.0f;

            // �v���C���[����̋�������������I��
            Vector3 chosenDirection = (Vector3.Distance(player.transform.position, rightPosition) >
                                       Vector3.Distance(player.transform.position, leftPosition))
                                       ? rightDirection : leftDirection;

            // �I�����ꂽ�����ɏ��������ړ�����
            Vector3 sideStepPosition = transform.position + chosenDirection * 1.0f;

            // NavMesh ��ŐV�����ʒu���L�����m�F���Ĉړ�
            NavMeshHit hit;
            if (NavMesh.SamplePosition(sideStepPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
                Debug.Log("�v���C���[���牓����������։��...");

                // �����Ɍ��̖ڕW�ɖ߂�R���[�`�����J�n
                StartCoroutine(ReturnToOriginalTarget());
            }
            else
            {
                // ���悪�����̏ꍇ�A����������̖ڕW�ɖ߂�
                Debug.Log("���悪 NavMesh ��ɑ��݂��Ȃ����߁A������s���܂���B");
                isAvoiding = false; // ������s�킸�ɏI��
            }
        }
    }
    IEnumerator ReturnToOriginalTarget()
    {
        // NavMeshAgent���L������NavMesh��ɂ��邩���m�F
        if (navMeshAgent == null || !navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent �������܂��� NavMesh ��ɂ��Ȃ����߁A���̖ړI�n�ɖ߂鏈���𒆎~���܂��B");
            isAvoiding = false;
            yield break;
        }

        // ����ɓ��B����܂őҋ@
        while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.5f)
        {
            // NavMeshAgent�� NavMesh ��ɂ��Ȃ��Ȃ����ꍇ�͒��~
            if (!navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning("NavMeshAgent �� NavMesh ��ɂ��Ȃ����߁AReturnToOriginalTarget �𒆎~���܂��B");
                isAvoiding = false;
                yield break;
            }

            yield return null;
        }

        // ���̖ړI�n�ɖ߂�
        navMeshAgent.SetDestination(originalTarget);
        isAvoiding = false;
    }



    void SetNextTarget()
    {
        if (!navMeshAgent.isOnNavMesh) return;

        Vector3 targetPosition = Vector3.zero;
        float yPosition = transform.position.y;

        if (yPosition >= 0)
        {
            float randomX = Random.value < 0.5f ? Random.Range(3f, -1f) : Random.Range(-6f, -2f);
            targetPosition = new Vector3(randomX, 1, Random.value < 0.5f ? 20 : -140);
        }
        else if (yPosition >= -6 && yPosition < 0)
        {
            float randomZ = Random.value < 0.5f ? Random.Range(-109f, -106f) : Random.Range(-110f, -113f);
            targetPosition = new Vector3(Random.value < 0.5f ? 0 : 130, -4, randomZ);
        }
        else if (yPosition >= -11 && yPosition < -6)
        {
            float randomX = Random.value < 0.5f ? Random.Range(103.3f, 106.3f) : Random.Range(107.3f, 110.3f);
            targetPosition = new Vector3(randomX, -9.4f, Random.value < 0.5f ? -120 : 10);
        }
        else
        {
            float randomZ = Random.value < 0.5f ? Random.Range(-20f, -17f) : Random.Range(-13f, -16f);
            targetPosition = new Vector3(Random.value < 0.5f ? 120 : -10, -14.4f, randomZ);
        }

        navMeshAgent.SetDestination(targetPosition);
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
        }
        else
        {
            Debug.LogError($"{gameObject.name} �̋߂��� NavMesh ��̗L���Ȓn�_��������܂���ł����B");
        }
    }
}