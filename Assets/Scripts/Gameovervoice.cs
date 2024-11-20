using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameovervoice : MonoBehaviour
{
    public AudioClip sound1; // �ŏ��̌��ʉ�
    public List<AudioClip> soundClips;
    public float delayBeforeStart = 5.0f;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlaySoundsInSequence());
    }


    private IEnumerator PlaySoundsInSequence()
    {
        // �ŏ��̌��ʉ����Đ�
        audioSource.clip = sound1;
        audioSource.Play();
        // �Đ��I���܂őҋ@
        yield return new WaitForSeconds(delayBeforeStart);


        // �����_���ȃC���f�b�N�X���擾
        int randomIndex = Random.Range(0, soundClips.Count);

        // �����_���ɑI�΂ꂽ���ʉ����Đ�
        audioSource.PlayOneShot(soundClips[randomIndex]);
    }

}
