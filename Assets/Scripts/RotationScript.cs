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
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (other.CompareTag("Player") && !isRotating) // プレイヤーにタグ付けされているオブジェクトのみに反応
        {
            StartCoroutine(RotatePlayer(other.transform, -90f));
        }

        if (playerMovement != null)
        {
            playerMovement.SetMovement(false);
            Debug.Log("Player entered the zone. Movement disabled.");
        }

        // オーディオクリップの再生
        if (audioSource != null && audioClips.Length > 0)
        {
            // 現在の侵入回数に応じたクリップを取得
            int clipIndex = entryCount % audioClips.Length; // 配列の範囲を超えないようにする
            audioSource.PlayOneShot(audioClips[clipIndex]);
            entryCount++; // 侵入回数を増やす
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
