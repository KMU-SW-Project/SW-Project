using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadData : MonoBehaviour
{
    public Text nickname;

    // Start is called before the first frame update
    void Start()
    {
        nickname.text = BackendServerManager.GetInstance().UserInfoData.userNickname;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
