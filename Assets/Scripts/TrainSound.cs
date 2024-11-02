using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainSound : MonoBehaviour
{
    public Transform player; // PlayerのTransformをアサインする
    public float triggerDistance = 5f; // SEを再生する距離のしきい値
    public AudioClip[] seClips; // 再生するSEのリスト
    private AudioSource audioSource;
    private bool hasPlayed = false;
    private int currentClipIndex = 0;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (seClips.Length > 0)
        {
            audioSource.clip = seClips[currentClipIndex];
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= triggerDistance && !hasPlayed)
        {
            PlayNextSE();
            hasPlayed = true;
        }
        else if (distance > triggerDistance && hasPlayed)
        {
            hasPlayed = false;
        }
    }

    void PlayNextSE()
    {
        if (currentClipIndex < seClips.Length)
        {
            audioSource.clip = seClips[currentClipIndex];
            audioSource.Play();
            currentClipIndex++;
            Invoke("PlayNextClipIfAvailable", audioSource.clip.length);
        }
    }

    void PlayNextClipIfAvailable()
    {
        if (currentClipIndex < seClips.Length)
        {
            audioSource.clip = seClips[currentClipIndex];
            audioSource.Play();
            currentClipIndex++;
            Invoke("PlayNextClipIfAvailable", audioSource.clip.length);
        }
    }
}
