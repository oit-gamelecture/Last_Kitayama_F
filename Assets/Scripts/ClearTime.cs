using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClearTime : MonoBehaviour
{
    public Text clearTimeText;    // �o�ߎ��Ԃ�\������e�L�X�g
    public float animationDuration = 2.0f;  // �A�j���[�V�����̍Đ�����
    public float delayBeforeStart = 5.0f;  // �\����x�点�鎞�ԁi�b�j
    public AudioClip tickSound;              // ���ʉ��N���b�v
    private AudioSource audioSource;         // ���ʉ��p��AudioSource

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
        // �\���J�n��x�点��
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

        // �Ō�ɍŏI���Ԃ𐳊m�ɕ\��
        clearTimeText.text =  finalTime.ToString("F2") + " s";
    }
}


