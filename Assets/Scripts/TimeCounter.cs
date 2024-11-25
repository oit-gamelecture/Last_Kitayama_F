using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    //�J�E���g�_�E��
    public float countdown = 90.00f;
    public float countdownTime = 3f;
    public AudioClip countSound;
    // ���Ԍx���̂��߂̕ϐ�
    private bool isWarning = false; // �x����ԃt���O
    private float scaleSpeed = 2.0f; // �g��k�����x
    private Vector3 originalScale; // ���̃X�P�[����ۑ�

    AudioSource audioSource;

    //���Ԃ�\������Text�^�̕ϐ�
    public Text timeText;


    //�^�C�}�[��\��
    private bool countStart = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(StartCountdown());
        timeText.enabled = false;

        audioSource = GetComponent<AudioSource>();
        StartCoroutine(StartCountdown());
        timeText.enabled = false;
        originalScale = timeText.transform.localScale; // �����X�P�[����ۑ�

    }

    //3�b��Ƀ^�C�}�[�X�^�[�g
    IEnumerator StartCountdown()
    {
        audioSource.PlayOneShot(countSound);
        yield return new WaitForSeconds(countdownTime);
        countStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (countStart)
        {
            timeText.enabled = true;
            //���Ԃ��J�E���g�_�E������
            countdown -= Time.deltaTime;

            //���Ԃ�\������
            timeText.text = countdown.ToString("f2");

            // �x����Ԃ̃`�F�b�N
            if (countdown <= 20f && !isWarning)
            {
                isWarning = true;
                StartCoroutine(WarningEffect());
            }
        }

        //countdown��0�ȉ��ɂȂ����Ƃ�
        if (countdown <= 0)
        {
            countdown = 0;
            countStart = false;
            SceneManager.LoadScene("over scene");
        }
    }

    public void SaveElapsedTime()
    {
        float elapsedTime = 90.00f - countdown;
        PlayerPrefs.SetFloat("ElapsedTime", elapsedTime);
    }

    public void StopCountdown()
    {
        countStart = false;
    }

    IEnumerator WarningEffect()
    {
        while (countdown > 0) // �c�莞�Ԃ�0�ɂȂ�܂Ń��[�v
        {
            // �g��
            for (float t = 0; t < 1; t += Time.deltaTime * scaleSpeed)
            {
                timeText.transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.5f, t);
                yield return null;
            }
            // �k��
            for (float t = 0; t < 1; t += Time.deltaTime * scaleSpeed)
            {
                timeText.transform.localScale = Vector3.Lerp(originalScale * 1.5f, originalScale, t);
                yield return null;
            }
        }
    }

}