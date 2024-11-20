using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportManager : MonoBehaviour
{
    // 非表示にしたいUIオブジェクト
    public GameObject uiElement;

    public GameObject reportText;

    // 衝突したプレイヤー
    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーと衝突した場合
        if (other.CompareTag("Player"))
        {
            // UIを表示
            uiElement.SetActive(true);
            reportText.SetActive(true);

            // 3秒後にUIを非表示にするコルーチンを開始
            StartCoroutine(HideUIAfterDelay(3f));
        }
    }

    // UIを指定した時間後に非表示にするコルーチン
    private IEnumerator HideUIAfterDelay(float delay)
    {
        // 指定時間待機
        yield return new WaitForSeconds(delay);

        // UIを非表示
        uiElement.SetActive(false);
        reportText.SetActive(false);
    }
}


