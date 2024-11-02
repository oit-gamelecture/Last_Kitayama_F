using UnityEngine;
using System.Collections.Generic;

public class PlaySEOnCollision : MonoBehaviour
{
    public List<AudioClip> seClips = new List<AudioClip>(); // �Đ�����SE�̃��X�g
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 1.0f;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Player�^�O�����I�u�W�F�N�g�ƏՓ˂����Ƃ�
        if (collision.gameObject.CompareTag("Player") && !hasPlayed && seClips.Count > 0)
        {
            // �����_���ɃN���b�v��I�����čĐ�
            int randomIndex = Random.Range(0, seClips.Count);
            audioSource.clip = seClips[randomIndex];
            audioSource.Play();
            hasPlayed = true; // ��x�Đ�������t���O�𗧂Ă�
        }
    }
}
