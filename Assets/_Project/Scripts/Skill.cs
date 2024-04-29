using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using UnityEngine;

public enum SkillType
{
    Always,
    NonTargetCooldown,
    TargetCooldown,
    Random,
}

public enum TargetType
{
    Enemy,
    AllAlly,
    StrongestAlly,
}

public class Skill : MonoBehaviour
{
    public SkillSO defaultStat;
    public SkillType skillType;
    public TargetType mainTargetType;

    public int priority;

    public int targetNumber;
    public int targetNumberIncrease;

    public float skillSpeed;

    public bool isReady;

    public float coolDown;
    public float coolDownIncrease;
    public float coolDownAmount
    {
        get
        {
            return (currentCooldown - coolDownTimeout) / Mathf.Clamp(currentCooldown, 0.1f, currentCooldown);
        }
    }

    public List<EnemyModel> enemyTargets;
    public List<CharacterModel> allyTargets;

    private float coolDownTimeout;
    private float currentCooldown;

    public float percentage;
    public float percentageIncrease;

    private void OnEnable()
    {
        targetNumberIncrease = 0;
        coolDownIncrease = 1.0f;
        percentageIncrease = 1.0f;

        coolDownTimeout = 0.0f;
        currentCooldown = coolDown * coolDownIncrease;
    }

    private void Update()
    {
        if (skillType == SkillType.NonTargetCooldown || skillType == SkillType.TargetCooldown)
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

    public Transform SetTarget(CharacterSkill owner)
    {
        enemyTargets.Clear();
        allyTargets.Clear();

        List<Target> targetInfo = new List<Target>();
        foreach (EnemyModel enemy in EnemyManager.Instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < owner.attack.applyAttackRange)
            {
                Target target = new Target(enemy, distance);
                targetInfo.Add(target);
            }
        }

        if (targetInfo.Count > 0)
        {
            targetInfo = targetInfo.OrderBy(a => a.distance).ToList();
            enemyTargets = new List<EnemyModel>();
            foreach (Target target in targetInfo)
            {
                enemyTargets.Add(target.model);
            } 
        }

        allyTargets = CharacterManager.Instance.wholeCharacters;
        allyTargets = allyTargets.OrderByDescending((model) => model.attack.applyDamage).ToList();

        switch (mainTargetType)
        {
            case TargetType.Enemy:
                if (enemyTargets.Count > 0)
                {
                    return enemyTargets[0].transform;
                }
                else
                {
                    return null;
                }

            case TargetType.AllAlly:
                return owner.transform;

            case TargetType.StrongestAlly:
                return allyTargets[0].transform;
        }
        return null;
    }

    public void StartSkill()
    {

    }

    public void OnSkill()
    {

    }

    public void CancelSkill()
    {

    }
}
