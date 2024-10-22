using System.Collections;
using UnityEngine;

public class NoSumahoMove : MonoBehaviour
{
    public Animator enemyanimator;  // �G�̃A�j���[�V�����R���g���[���[
    public float speed = 3.0f;      // �ړ����x
    public float downspeed = -2.0f; // �]�|���̍�����ޑ��x

    [SerializeField] private float moveDirectionZ = 1.0f; // Z���̈ړ����� (1: �O�i, -1: ���)
    private BoxCollider boxCol;  // �����蔻��p��BoxCollider
    private bool isFalling = false; // �]�|�t���O

    void Start()
    {
        boxCol = GetComponent<BoxCollider>();           // BoxCollider�̎擾
        enemyanimator = GetComponent<Animator>();        // Animator�̎擾

        enemyanimator.SetBool("IsWalking", true); // �����A�j���[�V�������J�n
    }

    void FixedUpdate()
    {
        if (!isFalling)
        {
            MoveEnemy(); // �ʏ�ړ�����
        }
    }

    // �G�̈ړ�����
    void MoveEnemy()
    {
        Vector3 movement = new Vector3(0, 0, moveDirectionZ) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    // �]�|���̏����i��ނƃA�j���[�V�����̃X���[�Y�Ȑ؂�ւ��j
    IEnumerator Down()
    {
        enemyanimator.SetBool("IsWalking", false); // �����A�j���[�V������~
        enemyanimator.SetTrigger("Fall"); // �]�|�A�j���[�V�����Đ�
        boxCol.enabled = false; // �����蔻��𖳌���

        float elapsedTime = 0f;
        float retreatDuration = 0.5f; // ��ނ��鎞�Ԃ�������������
        float fastRetreatSpeed = 8.0f; // �u�ԓI�ȍ�����ޑ��x

        // �����ŒZ���Ԍ�ނ��鏈��
        while (elapsedTime < retreatDuration)
        {
            transform.Translate(Vector3.back * fastRetreatSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        downspeed = 0; // ��ނ��~
        
    }

    // �v���C���[�Ƃ̐ڐG���̏���
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFalling)
        {
            isFalling = true; // �]�|�t���O�𗧂Ă�
            Debug.Log("fall");
            StartCoroutine(Down()); // �]�|�����̊J�n
        }
    }
}
