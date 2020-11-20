using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAI", menuName = "Scriptable Object/Enemy Data" , order = int.MaxValue)]
public class EnemyAI : ScriptableObject
{
    public string enemyName;
    public int enemyID;
    public int diffculty;
    public int bountyMoney;
    public float fireTime;
    public GameObject model;
    public Vector3 scale;
}
