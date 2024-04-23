using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const string prefabPath = "Prefabs/Enemies/";
    public static EnemySpawner Instance { get; private set; }
    public List<WaveData> waveDatas;

    public WaveData currentWaveData;
    public int currentSpawnCount;

    private void Awake()
    {
        Instance = this;
    }

    public void ReadySpawn()
    {
        foreach (WaveData waveData in waveDatas)
        {
            PoolManager.Instance.photonPool.AddResource(waveData.enemy.gameObject);
            PoolManager.Instance.photonPool.ReadyResource(waveData.enemy.gameObject.name, 10);
        }
    }

    public void StartSpawn()
    {
        StartCoroutine(GameCoroutine());
    }

    public IEnumerator GameCoroutine()
    {
        foreach (WaveData waveData in waveDatas)
        {
            currentWaveData = waveData;

            EnemyModel enemy = waveData.enemy;
            int spawnCount = waveData.spawnCount;
            WaitForSeconds spawnDelay = new WaitForSeconds(waveData.spawnDelay);
            WaitForSeconds breakTime = new WaitForSeconds(waveData.breakTime);

            yield return StartCoroutine(WaveCoroutine(enemy, spawnCount, spawnDelay, breakTime));
        }
    }

    public IEnumerator WaveCoroutine(EnemyModel enemy, int spawnCount, WaitForSeconds spawnDelay, WaitForSeconds breakTime)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = EnemyManager.Instance.enemyPaths[0].startPos.position;
            Quaternion spawnRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

            EnemyModel enemyInstance = PhotonNetwork.Instantiate(enemy.name, spawnPosition, spawnRotation).GetComponent<EnemyModel>();

            currentSpawnCount = i + 1;
            // 생성하는 로직.

            yield return spawnDelay;
        }

        yield return breakTime;
    }
}