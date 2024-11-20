using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public float rotationDuration = 1.0f; // ��]�̃X���[�Y�Ȏ��ԁi�b�j

    private bool isRotating = false;

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
