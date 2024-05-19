using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public Action<int> onEnemyCountChange;

    public List<EnemyModel> enemies; // 맵에 살아있는 모든 적
    public List<Path> enemyPaths; // 적들의 경로

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        onEnemyCountChange += UIManager.Instance.playerCrystals.UpdateEnemyCount;
    }

    public void AddEnemy(EnemyModel model)
    {
        enemies.Add(model);
        onEnemyCountChange?.Invoke(enemies.Count);
    }

    public void RemoveEnemy(EnemyModel model)
    {
        if (enemies.Contains(model) == false)
        {
            return;
        }

        enemies.Remove(model);
        onEnemyCountChange?.Invoke(enemies.Count);
    }
}