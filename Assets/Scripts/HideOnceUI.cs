using UnityEngine;

public class HideOnceUI : MonoBehaviour
{
    public GameObject uiPanel; // 表示するUIオブジェクトをInspectorで設定

    private bool hasBeenHidden = false; // UIが一度非表示になったかどうかのフラグ

    void Start()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true); // 最初は表示状態
        }
    }

    void Update()
    {
        // 十字キーが押され、UIがまだ非表示になっていない場合
        if (!hasBeenHidden && (Input.GetKeyDown(KeyCode.UpArrow) ||
                               Input.GetKeyDown(KeyCode.DownArrow) ||
                               Input.GetKeyDown(KeyCode.LeftArrow) ||
                               Input.GetKeyDown(KeyCode.RightArrow)))
        {
            HideUI();
        }
    }

    void HideUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // UIを非表示に
            hasBeenHidden = true;     // 一度非表示にしたことを記録
        }
    }
}

