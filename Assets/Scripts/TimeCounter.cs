using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    //カウントダウン
    public float countdown = 90.00f;
    public float countdownTime = 3f;
    public AudioClip countSound;
    AudioSource audioSource;

    //時間を表示するText型の変数
    public Text timeText;


    //タイマー非表示
    private bool countStart = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(StartCountdown());
        timeText.enabled = false;

    }

    //3秒後にタイマースタート
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
            //時間をカウントダウンする
            countdown -= Time.deltaTime;

            //時間を表示する
            timeText.text = countdown.ToString("f2");
        }
        //countdownが0以下になったとき
        if (countdown <= 0)
        {
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
}