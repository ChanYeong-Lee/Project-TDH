using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<WaveData> waveDatas;

    public IEnumerator WaveCoroutine(WaveData waveData)
    {
        for (int i = 0; i < waveData.enemyCount; i++)
        {
            // �����ϴ� ����.

            yield return waveData.spawnDelay;
        }

        yield return waveData.breakTime;
    }
}