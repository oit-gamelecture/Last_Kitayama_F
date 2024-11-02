using UnityEngine;

public class PlaySEOnCollision : MonoBehaviour
{
    public AudioClip seClip; // 再生するSE
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = seClip;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Playerタグを持つオブジェクトと衝突したとき
        if (collision.gameObject.CompareTag("Player") && !hasPlayed)
        {
            audioSource.Play();
            hasPlayed = true; // 一度再生したらフラグを立てる
        }
    }
}
