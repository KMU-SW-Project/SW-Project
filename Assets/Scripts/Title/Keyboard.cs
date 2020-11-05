using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    public InputField nickname;
    public Login _Login;

    public List<Text> key;
    bool isShift = true;

    public void KeyShift()
    {
        if (!isShift)
        {
            isShift = true;

            for (int i = 0; i < key.Count; i++)
                key[i].text = key[i].text.ToUpper();            
        }
        else
        {
            isShift = false;

            for (int i = 0; i < key.Count; i++)
                key[i].text = key[i].text.ToLower();
        }
    }

    public void InputKey(string key)
    {
        if (!isShift) nickname.text += key;
        else nickname.text += key.ToUpper();
    }

    public void KeyBackspace()
    {
        nickname.text = nickname.text.Substring(0, nickname.text.Length-1);
    }

    public void KeyEnter()
    {
        _Login.SetNickname();
    }

}
