using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGM : MonoBehaviour
{
    public AudioClip seClip; // SE用のAudioClip
    public AudioClip bgmClip; // BGM用のAudioClip
    public AudioSource audioSource; // 共通のAudioSource

    // Start is called before the first frame update
    void Start()
    {
        // SEクリップをセットして再生
        audioSource.clip = seClip;
        audioSource.Play();

        // SEの再生が完了した後にBGMをループ再生
        StartCoroutine(PlayBGMAfterSE());
    }

    private IEnumerator PlayBGMAfterSE()
    {
        // SEの再生が完了するまで待機
        yield return new WaitForSeconds(seClip.length);

        // BGMクリップをセットし、ループ再生
        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
