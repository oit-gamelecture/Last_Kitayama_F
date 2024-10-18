using System.Collections;
using UnityEngine;

public class enemymovement : MonoBehaviour
{
    public Animator enemyanimator;  // �G�̃A�j���[�V�����R���g���[���[
    public float speed = 3.0f;      // �ړ����x
    public float downspeed = -0.6f; // �]�|���̌�ޑ��x

    [SerializeField] private float moveDirectionZ = 1.0f; // Z���̈ړ����� (1: �O�i, -1: ���)
    private BoxCollider boxCol; // �����蔻��p��BoxCollider
    private bool isFalling = false; // �]�|�t���O

    void Start()
    {
        boxCol = GetComponent<BoxCollider>();           // BoxCollider�̎擾
        enemyanimator = GetComponent<Animator>();        // Animator�̎擾
    }

    void FixedUpdate()
    {
        if (!isFalling)
        {
            MoveEnemy(); // �ړ�����
        }
    }

    // �G�̈ړ�����
    void MoveEnemy()
    {
        Vector3 movement = new Vector3(0, 0, moveDirectionZ) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    // �]�|���̏���
    IEnumerator Down()
    {
        enemyanimator.SetTrigger("Fall"); // �]�|�A�j���[�V�������Đ�
        boxCol.enabled = false; // �����蔻��𖳌���

        // �]�|��̌��
        transform.Translate(Vector3.back * downspeed * Time.deltaTime, Space.World);
        yield return new WaitForSeconds(0.8f);

        // �ړ���~�Ə�������
        downspeed = 0;
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject); // �I�u�W�F�N�g���폜
    }

    // �v���C���[�Ƃ̐ڐG���̏���
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            isFalling = true; // �]�|�t���O�𗧂Ă�
            enemyanimator.CrossFade("Fall", 0); // �A�j���[�V�����̍Đ�
            StartCoroutine(Down()); // �]�|�����̊J�n
            Debug.Log("Collision with player detected");
        }
    }
}
