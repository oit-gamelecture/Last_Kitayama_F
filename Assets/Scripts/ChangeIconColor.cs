using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOutlineColor : MonoBehaviour
{
    public Outline targetOutline; // �F��ύX����A�E�g���C�������蓖�Ă�
    private Color originalColor;

    void Start()
    {
        if (targetOutline != null)
        {
            originalColor = targetOutline.effectColor; // ���̃A�E�g���C���F��ۑ�
        }
    }

    void Update()
    {
        if (targetOutline != null)
        {
            // F�L�[�������ꂽ���ɐԂ�����
            if (Input.GetKey(KeyCode.F) || Input.GetKeyDown("joystick button 0"))
            {
                targetOutline.effectColor = Color.red;
            }
            // F�L�[�𗣂������Ɍ��̐F�ɖ߂�
            else if (Input.GetKeyUp(KeyCode.F) || Input.GetKeyUp("joystick button 0"))
            {
                targetOutline.effectColor = originalColor;
            }
        }
    }
}
