using UnityEngine;
using UnityEngine.UI;

public class ClearTime : MonoBehaviour
{
    public Text clearTimeText;

    void Start()
    {
        // �ۑ����ꂽ�o�ߎ��Ԃ��擾���ĕ\��
        float elapsedTime = PlayerPrefs.GetFloat("ElapsedTime", 0);
        clearTimeText.text = "�N���A�^�C��: " + elapsedTime.ToString("F2") + "�b";
    }
}

