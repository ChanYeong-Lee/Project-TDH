using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    public EnemyMove move;

    private void Awake()
    {
        move = GetComponent<EnemyMove>();
    }
}
