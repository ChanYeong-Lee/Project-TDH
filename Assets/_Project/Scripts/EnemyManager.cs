using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public List<EnemyModel> enemies; // 맵에 살아있는 모든 적
    public List<Transform> enemyPaths; // 적들의 경로

    private void Awake()
    {
        Instance = this;
    }
}