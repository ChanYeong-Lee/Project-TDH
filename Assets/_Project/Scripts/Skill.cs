using System.Collections;
using System.Collections.Generic;
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

    public int targetNumber;
    public int targetNumberIncrease;

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
        if (coolDownTimeout <= 0.0f)
        {
            isReady = true;
        }
        else
        {
            coolDownTimeout -= Time.deltaTime;
        }
    }

    public virtual void Execute()
    {
        if (type == SkillType.CoolDown)
        {
            isReady = false;
            coolDownTimeout = coolDown * coolDownIncrease;
        }
    }
}
