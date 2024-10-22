using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneChange : MonoBehaviour
{
    public void TitleScene()
    {
        SceneManager.LoadScene("Title");
    }
}
