﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyModeLoad : MonoBehaviour
{
    public Transform spawnPos;
    public EnemyAI[] ai;

    private void Start()
    {
        Instantiate(ai[GameManager.GetInstance().modeData.currentPlayAI].model, spawnPos);
    }
}
