using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliderTime : MonoBehaviour
{
    public Slider meterSlider; // ���[�^�[�ɑΉ�����Slider
    public float maxTime = 30f; // ���[�^�[�̍ő厞��
    private float currentTime = 0f; // ���݂̎���
    private float delayTime = 3f; // �ŏ��ɑ҂���
    private bool isDelayOver = false; // �ҋ@���Ԃ��I�����������Ǘ�����t���O

    [SerializeField]
    private GameObject player; // �v���C���[�I�u�W�F�N�g�̎Q�� (�V���A���C�Y)

    private PlayerMovement playerMovement; // PlayerMovement�X�N���v�g�̎Q��
    private bool isPausedAfterFalling = false; // �]�|���A��̑ҋ@���t���O

    void Start()
    {
        if (meterSlider != null)
        {
            meterSlider.maxValue = maxTime; // �X���C�_�[�̍ő�l��ݒ�
            meterSlider.value = 0; // �����l��0�ɐݒ�
        }

        // PlayerMovement�X�N���v�g���擾
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(delayTime); // �w��b���ҋ@
        isDelayOver = true; // �ҋ@�I���t���O��ݒ�
    }

    void Update()
    {
        // �v���C���[���]�|���Ȃ�X���C�_�[�̐i�s���~
        if (playerMovement != null && playerMovement.isFalling)
        {
            Debug.Log("�v���C���[�]�|��: �X���C�_�[��~");
            return;
        }

        // �]�|���A���0.5�b�Ԃ̓X���C�_�[���~
        if (isPausedAfterFalling)
        {
            Debug.Log("�]�|���A��̑ҋ@��...");
            return;
        }

        if (isDelayOver && currentTime < maxTime)
        {
            currentTime += Time.deltaTime; // ���Ԃ����Z
            if (meterSlider != null)
            {
                meterSlider.value = currentTime; // ���[�^�[���X�V
            }
        }
        else if (!isDelayOver)
        {
            Debug.Log("�ҋ@��...");
        }
        else
        {
            Debug.Log("�^�C�}�[���ő�ɒB���܂����I");
        }
    }

    // �]�|���A��ɃX���C�_�[���~���鏈�����Ăяo��
    public void HandlePostFallPause()
    {
        StartCoroutine(PostFallPauseCoroutine());
    }

    IEnumerator PostFallPauseCoroutine()
    {
        isPausedAfterFalling = true; // �]�|���A��̑ҋ@�J�n
        yield return new WaitForSeconds(0.5f); // 0.5�b�ҋ@
        isPausedAfterFalling = false; // �ҋ@�I��
    }
}
