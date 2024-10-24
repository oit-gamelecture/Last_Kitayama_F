using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    //�J�E���g�_�E��
    public float countdown = 60.00f;
    public float countdownTime = 3f;

    //���Ԃ�\������Text�^�̕ϐ�
    public Text timeText;


    //�^�C�}�[��\��
    private bool countStart = false;

    void Start()
    {
        StartCoroutine(StartCountdown());
        timeText.enabled = false;

    }

    //3�b��Ƀ^�C�}�[�X�^�[�g
    IEnumerator StartCountdown()
    {
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
        }
        //countdown��0�ȉ��ɂȂ����Ƃ�
        if (countdown <= 0)
        {
            SceneManager.LoadScene("over scene");
        }
    }
}