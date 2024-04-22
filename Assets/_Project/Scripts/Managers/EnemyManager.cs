using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public List<EnemyModel> enemies; // �ʿ� ����ִ� ��� ��
    public List<Path> enemyPaths; // ������ ���

    private void Awake()
    {
        Instance = this;
    }

    public void TestFunction1()
    {
        foreach (EnemyModel enemy in enemies)
        {
            enemy.move.moveIncrease = 2.0f;
        }
    }

    public void TestFunction2()
    {
        foreach (EnemyModel enemy in enemies)
        {
            enemy.move.moveIncrease = 1.0f;
        }
    }
}