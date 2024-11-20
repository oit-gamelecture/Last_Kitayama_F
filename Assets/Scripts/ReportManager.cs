using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportManager : MonoBehaviour
{
    // ��\���ɂ�����UI�I�u�W�F�N�g
    public GameObject uiElement;

    public GameObject reportText;

    // �Փ˂����v���C���[
    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[�ƏՓ˂����ꍇ
        if (other.CompareTag("Player"))
        {
            // UI��\��
            uiElement.SetActive(true);
            reportText.SetActive(true);

            // 3�b���UI���\���ɂ���R���[�`�����J�n
            StartCoroutine(HideUIAfterDelay(3f));
        }
    }

    // UI���w�肵�����Ԍ�ɔ�\���ɂ���R���[�`��
    private IEnumerator HideUIAfterDelay(float delay)
    {
        // �w�莞�ԑҋ@
        yield return new WaitForSeconds(delay);

        // UI���\��
        uiElement.SetActive(false);
        reportText.SetActive(false);
    }
}


