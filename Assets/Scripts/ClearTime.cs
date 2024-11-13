using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClearTime : MonoBehaviour
{
    public Text clearTimeText;    // 経過時間を表示するテキスト
    public float animationDuration = 2.0f;  // アニメーションの再生時間
    public float delayBeforeStart = 1.0f;  // 表示を遅らせる時間（秒）

    void Start()
    {
        float finalTime = PlayerPrefs.GetFloat("ElapsedTime", 0);
        StartCoroutine(AnimateTimeDisplay(finalTime));
    }

    IEnumerator AnimateTimeDisplay(float finalTime)
    {
        // 表示開始を遅らせる
        yield return new WaitForSeconds(delayBeforeStart);

        float displayedTime = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            displayedTime = Mathf.Lerp(0, finalTime, elapsedTime / animationDuration);
            clearTimeText.text = displayedTime.ToString("F2") + "秒";

            yield return null;
        }

        // 最後に最終時間を正確に表示
        clearTimeText.text =  finalTime.ToString("F2") + "秒";
    }
}


