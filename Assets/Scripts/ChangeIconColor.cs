using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOutlineColor : MonoBehaviour
{
    public Outline targetOutline; // 色を変更するアウトラインを割り当てる
    private Color originalColor;

    void Start()
    {
        if (targetOutline != null)
        {
            originalColor = targetOutline.effectColor; // 元のアウトライン色を保存
        }
    }

    void Update()
    {
        if (targetOutline != null)
        {
            // Fキーが押された時に赤くする
            if (Input.GetKey(KeyCode.F) || Input.GetKeyDown("joystick button 0"))
            {
                targetOutline.effectColor = Color.red;
            }
            // Fキーを離した時に元の色に戻す
            else if (Input.GetKeyUp(KeyCode.F) || Input.GetKeyUp("joystick button 0"))
            {
                targetOutline.effectColor = originalColor;
            }
        }
    }
}
