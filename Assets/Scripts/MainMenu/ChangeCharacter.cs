using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCharacter : MonoBehaviour
{
    public GameObject[] character;

    int page = 0;

    public GameObject mainUI;
    public GameObject charUI;

    private void Start()
    {
        page = BackendServerManager.GetInstance().UserInfoData.userCharacter;

        SetCharacter(page);
    }

    public void changeUI(int type)
    {
        if (type == 1)
        {
            mainUI.SetActive(false);
            charUI.SetActive(true);
        }
        else if(type == 2)
        {
            BackendServerManager.GetInstance().UserInfoData.userCharacter = page;

            BackendServerManager.GetInstance().SetData("characterInfo", (bool result) =>
            {
                if (result)
                {
                    mainUI.SetActive(true);
                    charUI.SetActive(false);
                }
                else print("[캐릭터 저장] 예기치 못한 에러로 저장 못함");
            });
        }
        else if(type == 3)
        {
            page = BackendServerManager.GetInstance().UserInfoData.userCharacter;
            SetCharacter(page);
            mainUI.SetActive(true);
            charUI.SetActive(false);            
        }
    }

    public void changeCharater(int direction)
    {
        character[page].SetActive(false);

        // left
        if (direction == 1)
        {
            if (page == 0) page = character.Length - 1;
            else page--;

        }
        else // right
        {
            if (page == character.Length - 1) page = 0;
            else page++;
        }

        character[page].SetActive(true);
    }

    void SetCharacter(int charType)
    {
        for (int i = 0; i < character.Length; i++)
            character[i].SetActive(false);

        character[charType].SetActive(true);
    }

}
