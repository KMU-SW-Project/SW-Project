using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    public InputField nickname;
    public Login login;

    public List<Text> key;
    private bool _isShift = true;

    public void KeyShift()
    {
        if (!_isShift)
        {
            _isShift = true;

            for (int i = 0; i < key.Count; i++)
                key[i].text = key[i].text.ToUpper();
        }
        else
        {
            _isShift = false;

            for (int i = 0; i < key.Count; i++)
                key[i].text = key[i].text.ToLower();
        }
    }

    public void InputKey(string key)
    {
        if (!_isShift) nickname.text += key;
        else nickname.text += key.ToUpper();
    }

    public void KeyBackspace() => nickname.text = nickname.text.Substring(0, nickname.text.Length - 1);

    public void KeyEnter() => login.SetNickname();

}
