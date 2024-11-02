using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGM : MonoBehaviour
{
    public AudioClip seClip; // SE�p��AudioClip
    public AudioClip bgmClip; // BGM�p��AudioClip
    public AudioSource audioSource; // ���ʂ�AudioSource

    // Start is called before the first frame update
    void Start()
    {
        // SE�N���b�v���Z�b�g���čĐ�
        audioSource.clip = seClip;
        audioSource.Play();

        // SE�̍Đ��������������BGM�����[�v�Đ�
        StartCoroutine(PlayBGMAfterSE());
    }

    private IEnumerator PlayBGMAfterSE()
    {
        // SE�̍Đ�����������܂őҋ@
        yield return new WaitForSeconds(seClip.length);

        // BGM�N���b�v���Z�b�g���A���[�v�Đ�
        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
