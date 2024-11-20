using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameovervoice : MonoBehaviour
{
    public AudioClip sound1; // 最初の効果音
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
        // 最初の効果音を再生
        audioSource.clip = sound1;
        audioSource.Play();
        // 再生終了まで待機
        yield return new WaitForSeconds(delayBeforeStart);


        // ランダムなインデックスを取得
        int randomIndex = Random.Range(0, soundClips.Count);

        // ランダムに選ばれた効果音を再生
        audioSource.PlayOneShot(soundClips[randomIndex]);
    }

}
