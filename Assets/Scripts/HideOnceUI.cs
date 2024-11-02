using UnityEngine;
using UnityEngine.EventSystems;

public class HideOnceUI : MonoBehaviour
{
    public GameObject uiPanel; // �\������UI�I�u�W�F�N�g��Inspector�Őݒ�

    private bool hasBeenHidden = false; // UI����x��\���ɂȂ������ǂ����̃t���O

    void Start()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true); // �ŏ��͕\�����
        }
    }

    void Update()
    {
        // �\���L�[��������AUI���܂���\���ɂȂ��Ă��Ȃ��ꍇ
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            HideUI();
        }
    }

    void HideUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // UI���\����
            hasBeenHidden = true;     // ��x��\���ɂ������Ƃ��L�^
        }
    }
}

