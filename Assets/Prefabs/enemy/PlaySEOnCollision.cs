using UnityEngine;

public class PlaySEOnCollision : MonoBehaviour
{
    public AudioClip seClip; // �Đ�����SE
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = seClip;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Player�^�O�����I�u�W�F�N�g�ƏՓ˂����Ƃ�
        if (collision.gameObject.CompareTag("Player") && !hasPlayed)
        {
            audioSource.Play();
            hasPlayed = true; // ��x�Đ�������t���O�𗧂Ă�
        }
    }
}
