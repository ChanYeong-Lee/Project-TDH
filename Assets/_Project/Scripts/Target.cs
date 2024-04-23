using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Target 
{
    public EnemyModel model;
    public float distance;

    public Target(EnemyModel model, float distance)
    {
        this.model = model;
        this.distance = distance;
    }
}
