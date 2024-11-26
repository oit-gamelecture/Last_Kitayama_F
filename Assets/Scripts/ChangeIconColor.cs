using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeIconColor : MonoBehaviour
{
    public Image targetIcon; // �F��ύX����A�C�R�������蓖�Ă�
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
            // F�L�[�������ꂽ���ɐԂ�����
            if (Input.GetKey(KeyCode.F))
            {
                targetIcon.color = Color.red;
            }
            // F�L�[�𗣂������Ɍ��̐F�ɖ߂�
            else
            {
                targetIcon.color = originalColor;
            }
        }
    }
}
