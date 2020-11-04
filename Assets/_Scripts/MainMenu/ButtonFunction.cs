using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    public void LoadScene(int type)
    {
        if (type == 1) SceneManager.LoadScene("Infinity");
        else if (type == 2) SceneManager.LoadScene("vsAI");
        else if (type == 3) SceneManager.LoadScene("Training");
        else if (type == 4) SceneManager.LoadScene("MainMenu");
        else if (type == 5) SceneManager.LoadScene("Title");
    }
}
