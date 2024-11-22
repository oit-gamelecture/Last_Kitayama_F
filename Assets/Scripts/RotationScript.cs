using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public float rotationDuration = 1.0f; // 回転のスムーズな時間（秒）
    public AudioClip[] audioClips;        // 再生するオーディオクリップの配列
    private AudioSource audioSource;      // オーディオソース
    private int entryCount = 0;           // プレイヤーが侵入した回数
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
                playerMovement.SetMovement(true); // 移動を許可
            }

            // オーディオクリップ再生
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
            // PlayerMovementを取得して移動を再び有効化
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
            playerMovement.SetRotating(true); // 回転開始を通知
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
            playerMovement.SetRotating(false); // 回転終了を通知
        }
    }

}
