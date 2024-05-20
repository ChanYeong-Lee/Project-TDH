using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const string prefabPath = "Prefabs/Enemies/";
    public static EnemySpawner Instance { get; private set; }
    public List<WaveData> waveDatas;

    public Action<int> onWaveChange;
    public WaveData currentWaveData;
    public int currentSpawnCount;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        //onWaveChange += UIManager.Instance.playerCrystals.UpdateWave;
    }

    public void StartSpawn()
    {
        StartCoroutine(GameCoroutine());
    }

    public IEnumerator GameCoroutine()
    {
        for(int i = 0; i < waveDatas.Count; i++)
        {
            currentWaveData = waveDatas[i];
            onWaveChange?.Invoke(i + 1);

            if (GameManager.Instance.defensePlayer.singlePlay)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < currentWaveData.randomCrystal; k++)
                    {
                        GameManager.Instance.defensePlayer.AddRandomCrystal();
                    }
                    for (int k = 0; k < currentWaveData.specialCrystal; k++)
                    {
                        GameManager.Instance.defensePlayer.AddSpecialCrystal();
                    }
                }
            }
            else
            {
                for (int j = 0; j < currentWaveData.randomCrystal; j++)
                {
                    NetworkManager.Instance.photonView.RPC("GiveRandomCrystal", RpcTarget.All);
                }
                for (int j = 0; j < currentWaveData.specialCrystal; j++)
                {   
                    NetworkManager.Instance.photonView.RPC("GiveSpecialCrystal", RpcTarget.All);
                }
            }

            EnemyModel enemy = currentWaveData.enemy;
            int spawnCount = currentWaveData.spawnCount;
            WaitForSeconds spawnDelay = new WaitForSeconds(currentWaveData.spawnDelay);
            WaitForSeconds breakTime = new WaitForSeconds(currentWaveData.breakTime);

            yield return StartCoroutine(WaveCoroutine(enemy, spawnCount, spawnDelay, breakTime));
        }
    }

    public IEnumerator WaveCoroutine(EnemyModel enemy, int spawnCount, WaitForSeconds spawnDelay, WaitForSeconds breakTime)
    {
        Vector3 spawnPosition = EnemyManager.Instance.enemyPaths[0].startPos.position;
        Quaternion spawnRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        PoolManager.Instance.networkPool.PreSpawn(prefabPath + enemy.name, spawnCount / 2, spawnPosition, spawnRotation);

        for (int i = 0; i < spawnCount; i++)
        {
            EnemyModel enemyInstance = PoolManager.Instance.networkPool.Spawn(prefabPath + enemy.name, spawnPosition, spawnRotation).GetComponent<EnemyModel>();
            currentSpawnCount = i + 1;
            // 생성하는 로직.

            yield return spawnDelay;
        }

        yield return breakTime;
    }
}