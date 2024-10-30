using UnityEngine;
using System.Collections;

public class DelayedBGM : MonoBehaviour
{
    public AudioSource bgmSource;
    public float delayInSeconds = 5.0f;

    void Start()
    {
        StartCoroutine(PlayBGMAfterDelay());
    }

    IEnumerator PlayBGMAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        bgmSource.Play();  // íxâÑå„Ç…BGMçƒê∂
    }
}

