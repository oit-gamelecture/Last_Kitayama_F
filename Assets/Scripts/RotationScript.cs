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
        if (other.CompareTag("Player") && !isRotating)
        {
            StartCoroutine(RotatePlayer(other.transform, -90f));
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetMovement(true); // �ړ�������
            }

            // �I�[�f�B�I�N���b�v�Đ�
            if (audioSource != null && audioClips.Length > 0)
            {
                int clipIndex = entryCount % audioClips.Length;
                audioSource.PlayOneShot(audioClips[clipIndex]);
                entryCount++;
            }
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
        PlayerMovement playerMovement = playerTransform.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SetRotating(true); // ��]�J�n��ʒm
        }

        Quaternion startRotation = playerTransform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, targetAngle, 0);

        float elapsedTime = 0f;
        while (elapsedTime < rotationDuration)
        {
            playerTransform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerTransform.rotation = endRotation;
        isRotating = false;

        if (playerMovement != null)
        {
            playerMovement.SetRotating(false); // ��]�I����ʒm
        }
    }

}
