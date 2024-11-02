using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSettingScene : MonoBehaviour
{
    public void ChangeScene(string SettingScene)
    {
        SceneManager.LoadScene(SettingScene);
    }
}
