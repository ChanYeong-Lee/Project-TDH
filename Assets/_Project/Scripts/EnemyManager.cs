using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public List<EnemyModel> enemies; // �ʿ� ����ִ� ��� ��
    public List<Transform> enemyPaths; // ������ ���

    private void Awake()
    {
        Instance = this;
    }
}