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
    public float breakTime = 20.0f; // 모두 생성한 뒤 쉬는 시간

    public int randomCrystal;
    public int specialCrystal;
}
