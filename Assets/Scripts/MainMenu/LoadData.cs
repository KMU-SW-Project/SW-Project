using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadData : MonoBehaviour
{
    public Text nickname;
    ModeManager modeManager;

    private void Awake()
    {
        modeManager = GetComponent<ModeManager>();       
    }

    void Start()
    {
        if (BackendServerManager.GetInstance().isConnected)
        {
            nickname.text = BackendServerManager.GetInstance().UserInfoData.userNickname;
            modeManager.ReSetMode();
        }
        else nickname.text = "OFFLINE";
    }
}
