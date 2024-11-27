using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInTextWithActivation : MonoBehaviour
{
    public Text uiText; // �Ώۂ�UI�e�L�X�g
    public float fadeDuration = 2f; // �t�F�[�h�C���ɂ����鎞��
    public GameObject[] objectsToActivate; // �L��������I�u�W�F�N�g�Q
    public float activationDelay = 0.5f; // �e�L�X�g�\����̒x������

    private float fadeTimer = 0f; // �t�F�[�h�p�^�C�}�[
    private Color initialColor; // �e�L�X�g�̌��̐F
    private Vector3 initialScale; // �����X�P�[��
    public float maxScale = 1.2f; // �ő�X�P�[��

    private bool fadeComplete = false; // �t�F�[�h�����t���O

    private void Start()
    {
        if (uiText == null)
        {
            uiText = GetComponent<Text>();
        }

        // ������Ԃ̐ݒ�
        initialColor = uiText.color;
        uiText.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0); // �A���t�@�l��0��
        initialScale = uiText.transform.localScale;
        uiText.transform.localScale = Vector3.zero; // �����X�P�[����0��
    }

    private void Update()
    {
        if (!fadeComplete)
        {
            FadeInAndScale();
        }
    }

    private void FadeInAndScale()
    {
        if (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;

            // �A���t�@�l����`���
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            uiText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            // �X�P�[���̕ύX
            float scale = Mathf.Lerp(0, maxScale, fadeTimer / fadeDuration);
            uiText.transform.localScale = initialScale * scale;
        }
        else if (!fadeComplete)
        {
            // �t�F�[�h������������t���O���Z�b�g���A�x���t���ŃI�u�W�F�N�g��L����
            fadeComplete = true;
            StartCoroutine(ActivateObjectsWithDelay());
        }
    }

    private IEnumerator ActivateObjectsWithDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true); // �I�u�W�F�N�g��L����
            }
        }
    }
}
