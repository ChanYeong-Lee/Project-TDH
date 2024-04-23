using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    public EnemyMove move;
    public EnemyHealth health;

    private void Awake()
    {
        move = GetComponent<EnemyMove>();
        health = GetComponent<EnemyHealth>();
    }
    
    private void OnEnable()
    {
        EnemyManager.Instance.enemies.Add(this);
    }

    private void OnDisable()
    {
        EnemyManager.Instance.enemies.Remove(this);
    }
}
