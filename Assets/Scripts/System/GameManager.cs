﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum HandType
{
    left,
    right,
}

[Serializable]
public class UserData
{
    // 서버에 저장된 고유 테이블 번호
    public string dataIndate;
    public string rankingUuid;

    // 유저 데이터
    public string userNickname;
    public int userCharacter;
    public int userClearAI;
    public int userInfinityScore;
}

public class ModeData
{
    public GameMode currentPlayMode;
    public EnemyAI currentPlayAiData;
    public int currentSelectedTarget;
}

public static class SoundValue
{
    public static AudioMixer audioMixer;
    private static int bGM;
    private static int sFX;

    public static int BGM
    {
        get => bGM;
        set
        {
            if (value > 8 || value < 0) return;

            bGM = value;
            TitleManager.instance.SetSoundValueText(BGM, SFX);
        }
    }
    public static int SFX
    {
        get => sFX;
        set
        {
            if (value > 8 || value < 0) return;

            sFX = value;
            TitleManager.instance.SetSoundValueText(BGM, SFX);
        }
    }
}


public class GameManager : MonoBehaviour
{
    #region 싱글톤
    private static GameManager instance;

    public GameObject player;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            Destroy(player);
        }
        player = GameObject.Find("Player");
        instance = this;

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(player);
        player.name = "donPlayer";

        if (audioMixer != null)
        {
            SoundValue.audioMixer = audioMixer;
            SoundValue.BGM = SoundValue.SFX = 8;
        }

    }

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            print("GameManager 인스턴스 없음");
            return null;
        }

        return instance;
    }
    #endregion

    public UserData userData = new UserData();
    public ModeData modeData = new ModeData();

    // 0,1 = left gun-hand , 2,3 = right gun-hand
    public GameObject[] handModel;
    public GameMode playMode;
    public bool gunGuide;
    public AudioMixer audioMixer;

    public void SetUserControllerModel(HandType handtype)
    {
        for (int i = 0; i < handModel.Length; i++)
            handModel[i].SetActive(false);

        switch (handtype)
        {
            case HandType.left:
                handModel[0].SetActive(true);
                handModel[3].SetActive(true);
                break;
            case HandType.right:
                handModel[1].SetActive(true);
                handModel[2].SetActive(true);
                break;
            default:
                break;
        }
    }

    public void SetPlayerCameraPosition(Transform originPlayer)
    {
        player.transform.position = originPlayer.transform.position;
        player.transform.rotation = originPlayer.transform.rotation;
        player.transform.GetComponentInChildren<Camera>().transform.rotation = originPlayer.transform.rotation;
    }

    public void SetSoundValue(string typeValue)
    {
        int index = typeValue.IndexOf('|');
        var type = typeValue.Substring(0, index);
        var flag = typeValue.Substring(index + 1);

        if (type == "BGM")
        {
            if (flag == "Up") SoundValue.BGM++;
            else SoundValue.BGM--;

            SoundValue.audioMixer.SetFloat("BGM", (SoundValue.BGM*10) - 80);
            return;
        }

        if (flag == "Up") SoundValue.SFX++;
        else SoundValue.SFX--;

        SoundValue.audioMixer.SetFloat("SFX", (SoundValue.SFX*10) - 80);
    }
}
