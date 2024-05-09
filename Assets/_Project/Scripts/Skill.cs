using Photon.Pun;
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
    Self
}

public class Skill : MonoBehaviour
{
    [Header("설정")]
    public SkillSO defaultStat;

    [Header("상태")]
    public bool isReady;

    public float applyCooldown => Mathf.Clamp(coolDown / Mathf.Clamp(coolDownIncrease, 0.1f, coolDownIncrease), 0.1f, coolDown / Mathf.Clamp(coolDownIncrease, 0.1f, coolDownIncrease));
    public float coolDown;
    public float coolDownIncrease;
    public float coolDownAmount
    {
        get
        {
            return (currentCooldown - coolDownTimeout) / Mathf.Clamp(currentCooldown, 0.1f, currentCooldown);
        }
    }

    public float applyPercentage => Mathf.Clamp(percentage + percentageIncrease, 0.0f, 1.0f);
    public float percentage;
    public float percentageIncrease;

    public List<AttackEffect> attackEffects;
    public List<BuffEffect> buffEffects;    

    public List<EnemyModel> enemyTargets;
    public List<CharacterModel> allyTargets;

    private float coolDownTimeout;
    private float currentCooldown;

    private void OnEnable()
    {
        attackEffects = defaultStat.attackEffects;
        buffEffects = defaultStat.buffEffects;

        coolDown = defaultStat.defaultCooldown;
        percentage = defaultStat.defaultPercent;

        coolDownIncrease = 1.0f;
        percentageIncrease = 1.0f;

        coolDownTimeout = 0.0f;
        currentCooldown = applyCooldown;
    }

    private void Update()
    {
        if (defaultStat.skillType == SkillType.NonTargetCooldown || defaultStat.skillType == SkillType.TargetCooldown)
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
            float distance = Vector3.Distance(owner.transform.position, enemy.transform.position);
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

        switch (defaultStat.mainTargetType)
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
            case TargetType.Self:
                return owner.transform; 
        }
        return null;
    }

    public void StartSkill()
    {
        currentCooldown = applyCooldown;
        coolDownTimeout = currentCooldown; 
    }

    public void OnSkill(CharacterSkill owner)
    {
        foreach (AttackEffect attackEffect in attackEffects)
        {
            attackEffect.Execute(owner, enemyTargets);
        }

        foreach (BuffEffect buffEffect in buffEffects)
        {
            buffEffect.Execute(owner, allyTargets, enemyTargets);
        }
        print($"{defaultStat.skillName}! {defaultStat.description}"); 
    }

    public void CancelSkill()
    {
        if (defaultStat.skillType == SkillType.NonTargetCooldown || defaultStat.skillType == SkillType.TargetCooldown)
        {
            coolDownTimeout = 0.1f;
        }
    }
}
