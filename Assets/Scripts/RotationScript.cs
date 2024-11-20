using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public float rotationDuration = 1.0f; // ��]�̃X���[�Y�Ȏ��ԁi�b�j
    public AudioClip[] audioClips;        // �Đ�����I�[�f�B�I�N���b�v�̔z��
    private AudioSource audioSource;      // �I�[�f�B�I�\�[�X
    private int entryCount = 0;           // �v���C���[���N��������
    private bool isRotating = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (other.CompareTag("Player") && !isRotating) // �v���C���[�Ƀ^�O�t������Ă���I�u�W�F�N�g�݂̂ɔ���
        {
            StartCoroutine(RotatePlayer(other.transform, -90f));
        }

        if (playerMovement != null)
        {
            playerMovement.SetMovement(false);
            Debug.Log("Player entered the zone. Movement disabled.");
        }

        // �I�[�f�B�I�N���b�v�̍Đ�
        if (audioSource != null && audioClips.Length > 0)
        {
            // ���݂̐N���񐔂ɉ������N���b�v���擾
            int clipIndex = entryCount % audioClips.Length; // �z��͈̔͂𒴂��Ȃ��悤�ɂ���
            audioSource.PlayOneShot(audioClips[clipIndex]);
            entryCount++; // �N���񐔂𑝂₷
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // PlayerMovement���擾���Ĉړ����ĂїL����
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetMovement(true);
                Debug.Log("Player exited the zone. Movement enabled.");
            }
        }
    }

    private IEnumerator RotatePlayer(Transform playerTransform, float targetAngle)
    {
        isRotating = true;

        Quaternion startRotation = playerTransform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, targetAngle, 0);

        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            playerTransform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerTransform.rotation = endRotation; // �ŏI�I�ȉ�]�p�ɃZ�b�g
        isRotating = false;
    }
}
