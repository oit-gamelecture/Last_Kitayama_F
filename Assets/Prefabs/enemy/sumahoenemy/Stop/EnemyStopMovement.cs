using System.Collections;
using UnityEngine;

public class EnemyStopMovement : MonoBehaviour
{
    public Animator enemyAnimator;  // �G�̃A�j���[�V�����R���g���[���[
    public float speed = 3.0f;      // �ړ����x
    public float stopDistance = -10.0f; // �v���C���[�̎�O�Œ�~���鋗��
    public float downspeed = -2.0f;  // �]�|���̌�ޑ��x

    private Transform player;       // �v���C���[��Transform
    private bool isStopped = false; // ��~�t���O
    private bool isFalling = false; // �]�|�t���O

    void Start()
    {
        // �v���C���[��Transform���擾
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // �A�j���[�V�����R���g���[���[�̎擾
        enemyAnimator = GetComponent<Animator>();

        // �����A�j���[�V�������J�n
        enemyAnimator.SetBool("IsWalking", true);
    }

    void Update()
    {
        if (!isFalling) // �]�|���łȂ��ꍇ�݈̂ړ�����
        {
            MoveTowardsPlayer(); // �v���C���[�Ɍ������Ĉړ����鏈��
        }
    }

    // �v���C���[�Ɍ������Ĉړ����鏈��
    void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            // �v���C���[�Ɍ������Ĉړ�
            Vector3 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
            enemyAnimator.SetBool("IsWalking", true); // �����A�j���[�V�����Đ�
        }
        else
        {
            // ��~�A�j���[�V�����ɑJ��
            StopAndPlayAnimation();
        }
    }

    // ��~�A�j���[�V�����̏���
    void StopAndPlayAnimation()
    {
        if (!isStopped)
        {
            isStopped = true; // ��~�t���O�𗧂Ă�
            enemyAnimator.SetBool("IsWalking", false); // �����A�j���[�V������~
            enemyAnimator.SetTrigger("stop"); // Stop�A�j���[�V�����Đ�
        }
    }

    // �v���C���[�Ƃ̏Փˎ��̏���
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            StartCoroutine(Down()); // �]�|�����̊J�n
        }
    }

    // �]�|����
    IEnumerator Down()
    {
        isFalling = true; // �]�|�t���O�𗧂Ă�
        enemyAnimator.SetBool("IsWalking", false); // �����A�j���[�V�������~
        enemyAnimator.SetTrigger("Fall"); // �]�|�A�j���[�V�����Đ�

        float elapsedTime = 0f;
        float retreatDuration = 0.8f; // �]�|���̌�ގ���

        // �����Ō�ނ��鏈��
        while (elapsedTime < retreatDuration)
        {
            transform.Translate(Vector3.back * downspeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        downspeed = 0; // ��ނ��~

        yield return new WaitForSeconds(5.0f); // 5�b��ɃI�u�W�F�N�g���폜
        Destroy(gameObject); // �I�u�W�F�N�g���폜
    }
}
