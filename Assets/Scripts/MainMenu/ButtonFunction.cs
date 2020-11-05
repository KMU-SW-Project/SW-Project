using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    public void LoadScene(int type)
    {
        if (type == 1) SceneManager.LoadSceneAsync("Infinity");
        else if (type == 2) SceneManager.LoadSceneAsync("vsAI");
        else if (type == 3) SceneManager.LoadSceneAsync("Training");
        else if (type == 4) SceneManager.LoadSceneAsync("MainMenu");
        else if (type == 5) SceneManager.LoadSceneAsync("Title");
    }
}
