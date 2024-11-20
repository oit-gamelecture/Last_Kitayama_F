using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClearTime : MonoBehaviour
{
    public Text clearTimeText;    // 経過時間を表示するテキスト
    public Text valueText1;
    public Text valueText2;
    public Text valueText3;
    public Text valueText4;
    public float animationDuration = 2.0f;  // アニメーションの再生時間
    public float delayBeforeStart = 5.0f;  // 表示を遅らせる時間（秒）
    public float delayBeforeStart1 = 5.0f;
    public AudioClip tickSound;              // 効果音クリップ
    private AudioSource audioSource;         // 効果音用のAudioSource

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not attached to this GameObject!");
            return;
        }

        float finalTime = PlayerPrefs.GetFloat("ElapsedTime", 0);
        StartCoroutine(AnimateTimeDisplay(finalTime));
    }

    IEnumerator AnimateTimeDisplay(float finalTime)
    {
        // 表示開始を遅らせる
        yield return new WaitForSeconds(delayBeforeStart);

        if (tickSound != null)
        {
            audioSource.PlayOneShot(tickSound);
        }
        else
        {
            Debug.LogWarning("tickSound is not assigned. Skipping PlayOneShot.");
        }
        
        float displayedTime = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            displayedTime = Mathf.Lerp(0, finalTime, elapsedTime / animationDuration);
            clearTimeText.text = displayedTime.ToString("F2") + " s";

            yield return null;
        }

        // 最後に最終時間を正確に表示
        clearTimeText.text =  finalTime.ToString("F2") + " s";

        yield return new WaitForSeconds(delayBeforeStart1);

        if (finalTime < 75.00f)
        {
            valueText1.text = "S";
        }
        else if(finalTime < 80.00f)
        {
            valueText2.text = "A";
        }
        else if(finalTime < 85.00f)
        {
            valueText3.text = "B";
        }
        else 
        {
            valueText4.text = "C";
        }

    }
}


