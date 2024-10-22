using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public void End()
    {
        UnityEditor.EditorApplication.isPlaying = false;

        Application.Quit(); 
    }
}
