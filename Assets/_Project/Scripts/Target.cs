using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Target 
{
    public CharacterModel allyModel;
    public EnemyModel enemyModel;
    public float distance;

    public Target(EnemyModel model, float distance)
    {
        this.enemyModel = model;
        this.distance = distance;
    }

    public Target(CharacterModel model, float distance)
    {
        this.allyModel = model;
        this.distance = distance;
    }
}
