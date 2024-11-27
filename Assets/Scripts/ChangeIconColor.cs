using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeIconColor : MonoBehaviour
{
    public Image targetIcon; // 色を変更するアイコンを割り当てる
    private Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        if (targetIcon != null)
        {
            originalColor = targetIcon.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetIcon != null)
        {
            // Fキーが押された時に赤くする
            if (Input.GetKey(KeyCode.F) || Input.GetKeyDown("joystick button 0"))
            {
                targetIcon.color = Color.red;
            }
            // Fキーを離した時に元の色に戻す
            else if(Input.GetKeyUp(KeyCode.F) || Input.GetKeyUp("joystick button 0"))
            {
                targetIcon.color = originalColor;
            }
        }
    }
}
