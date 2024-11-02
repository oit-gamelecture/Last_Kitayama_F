using UnityEngine;
using System.Collections.Generic;

public class PlaySEOnCollision : MonoBehaviour
{
    public List<AudioClip> seClips = new List<AudioClip>(); // 再生するSEのリスト
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 1.0f;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Playerタグを持つオブジェクトと衝突したとき
        if (collision.gameObject.CompareTag("Player") && !hasPlayed && seClips.Count > 0)
        {
            // ランダムにクリップを選択して再生
            int randomIndex = Random.Range(0, seClips.Count);
            audioSource.clip = seClips[randomIndex];
            audioSource.Play();
            hasPlayed = true; // 一度再生したらフラグを立てる
        }
    }
}
