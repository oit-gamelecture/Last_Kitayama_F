using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClearTime : MonoBehaviour
{
    public Text clearTimeText;    // �o�ߎ��Ԃ�\������e�L�X�g
    public float animationDuration = 2.0f;  // �A�j���[�V�����̍Đ�����
    public float delayBeforeStart = 1.0f;  // �\����x�点�鎞�ԁi�b�j

    void Start()
    {
        float finalTime = PlayerPrefs.GetFloat("ElapsedTime", 0);
        StartCoroutine(AnimateTimeDisplay(finalTime));
    }

    IEnumerator AnimateTimeDisplay(float finalTime)
    {
        // �\���J�n��x�点��
        yield return new WaitForSeconds(delayBeforeStart);

        float displayedTime = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            displayedTime = Mathf.Lerp(0, finalTime, elapsedTime / animationDuration);
            clearTimeText.text = displayedTime.ToString("F2") + "�b";

            yield return null;
        }

        // �Ō�ɍŏI���Ԃ𐳊m�ɕ\��
        clearTimeText.text =  finalTime.ToString("F2") + "�b";
    }
}


