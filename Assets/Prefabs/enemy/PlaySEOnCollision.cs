using UnityEngine;
using System.Collections.Generic;

public class PlaySEOnCollision : MonoBehaviour
{
    public List<AudioClip> seClips = new List<AudioClip>(); // 再生するSEのリスト
    private AudioSource audioSource;

    [SerializeField]
    private float volume = 1.0f; // 音量をInspectorで設定可能

    private bool hasPlayed = false;

    void Start()
    {
        // AudioSourceを追加し、初期設定
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume; // Inspectorで設定した音量を適用
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
