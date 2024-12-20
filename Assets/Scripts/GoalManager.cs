using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TimeCounter timeCounter = FindObjectOfType<TimeCounter>();
            if (timeCounter != null)
            {
                // 経過時間を保存
                timeCounter.SaveElapsedTime();
                timeCounter.StopCountdown();
            }

            SceneManager.LoadScene("clear scene"); 
        }
    }
}
