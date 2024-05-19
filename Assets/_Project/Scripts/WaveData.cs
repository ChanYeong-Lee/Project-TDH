using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveData
{
    public EnemyModel enemy;
    public int spawnCount = 40;
    public float spawnDelay = 0.5f; 
    public float breakTime = 20.0f; // ��� ������ �� ���� �ð�

    public int randomCrystal;
    public int specialCrystal;
}
