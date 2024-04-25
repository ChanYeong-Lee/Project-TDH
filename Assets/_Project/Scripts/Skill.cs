using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum SkillType
{
    Always,
    CoolDown,
    Random,
}

public class Skill : MonoBehaviour
{
    public SkillSO defaultStat;
    public SkillType type;

    public int priority;

    public int targetNumber;
    public int targetNumberIncrease;

    public float skillSpeed;

    public bool isReady;
    public float coolDown;
    public float coolDownIncrease;
    private float coolDownTimeout;

    public float percentage;
    public float percentageIncrease;

    protected virtual void OnEnable()
    {
        targetNumberIncrease = 0;
        coolDownIncrease = 1.0f;
        percentageIncrease = 1.0f;

        coolDownTimeout = 0.0f;
    }

    protected virtual void Update()
    {
        if (type == SkillType.CoolDown)
        {
            if (coolDownTimeout <= 0.0f)
            {
                isReady = true;
            }
            else
            {
                isReady = false;
                coolDownTimeout -= Time.deltaTime;
            }
        }
    }

    public virtual void StartSkill()
    {
        if (type == SkillType.CoolDown)
        {
            coolDownTimeout = coolDown * coolDownIncrease;
        }
    }

    public virtual void OnSkill()
    {

    }

    public virtual void CancelSkill()
    {

    }
}
