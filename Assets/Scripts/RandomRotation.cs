using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float rotationDuration = 1.0f; // Rotation duration in seconds
    public AudioClip[] audioClips;        // Array of audio clips to play
    private AudioSource audioSource;      // AudioSource component
    private int entryCount = 0;           // Counter for how many times the player has entered the trigger
    private bool isRotating = false;      // Flag to check if the player is currently rotating

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
            // Randomly choose whether to rotate right (90 degrees) or left (-90 degrees)
            float rotationDirection = Random.Range(0f, 1f) > 0.5f ? 90f : -90f;
            StartCoroutine(RotatePlayer(other.transform, rotationDirection));

            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetMovement(true); // Allow player movement
            }

            // Play an audio clip when the player enters
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
            // Enable player movement when exiting the trigger zone
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
            playerMovement.SetRotating(true); // Notify that rotation has started
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
            playerMovement.SetRotating(false); // Notify that rotation has finished
        }
    }
}
