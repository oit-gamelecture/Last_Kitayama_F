using UnityEngine;
using UnityEngine.UI;

public class ClearTime : MonoBehaviour
{
    public Text clearTimeText;

    void Start()
    {
        // 保存された経過時間を取得して表示
        float elapsedTime = PlayerPrefs.GetFloat("ElapsedTime", 0);
        clearTimeText.text = "クリアタイム: " + elapsedTime.ToString("F2") + "秒";
    }
}

