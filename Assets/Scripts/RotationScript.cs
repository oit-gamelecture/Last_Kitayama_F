using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public float rotationDuration = 1.0f; // 回転のスムーズな時間（秒）

    private bool isRotating = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRotating) // プレイヤーにタグ付けされているオブジェクトのみに反応
        {
            StartCoroutine(RotatePlayer(other.transform, -90f));
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

        playerTransform.rotation = endRotation; // 最終的な回転角にセット
        isRotating = false;
    }

}
