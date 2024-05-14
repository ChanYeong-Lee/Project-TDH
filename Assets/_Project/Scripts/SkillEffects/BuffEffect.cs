using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static Cinemachine.CinemachineTargetGroup;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public class BuffEffect
{
    public string buffName;

    public AttackType attackType;
    public TargetType targetType;
    public BuffType buffType;
    public StatType statType;
    public float increaseAmount;
    public int integerIncreaseAmount;
    public float limitTime;

    public int targetNumber;
    public float attackArea;

    public List<CharacterModel> allyTargets;
    public List<EnemyModel> enemyTargets;

    public void Execute(CharacterSkill owner, CharacterModel allyTarget, EnemyModel enemyTarget)
    {
        switch (attackType)
        {
            case AttackType.Single:
                switch (targetType)
                {
                    case TargetType.Enemy:
                        SingleEnemyBuff(owner);
                        break;
                    case TargetType.AllAlly:
                        SingleAllyBuff(owner);
                        break;
                    case TargetType.StrongestAlly:
                        SingleStrongestBuff(owner);
                        break;
                    case TargetType.Self:
                        CharacterModel ownerModel = owner.GetComponent<CharacterModel>();
                        ApplyBuff(owner, ownerModel);
                        break;
                }
                break;
            case AttackType.Area:
                switch (targetType)
                {
                    case TargetType.Enemy:
                        AreaEnemyBuff(owner, enemyTarget);
                        break;
                    case TargetType.AllAlly:
                        AreaAllyBuff(owner, allyTarget);
                        break;
                    case TargetType.StrongestAlly:
                        break;
                }
                break;
        }
    }
    
    public void ApplyBuff(CharacterSkill owner, CharacterModel model)
    {
        if (statType == StatType.TargetNumber)
        {
            model.photonView.RPC("AddBuff", RpcTarget.All, owner.photonView.ViewID, buffName, buffType, statType, integerIncreaseAmount, limitTime);
        }
        else
        {
            model.photonView.RPC("AddBuff", RpcTarget.All, owner.photonView.ViewID, buffName, buffType, statType, increaseAmount, limitTime);
        }
    }

    public void ApplyBuff(CharacterSkill owner, EnemyModel model)
    {
        model.photonView.RPC("AddBuff", RpcTarget.All,
            owner.photonView.ViewID, model.poolCount,
            buffName, buffType, statType,
            increaseAmount, limitTime);
    }

    public void RemoveBuff(CharacterModel model)
    {
        model.photonView.RPC("RemoveBuff", RpcTarget.All, buffName);
    }

    public void RemoveBuff(EnemyModel model)
    {
        model.photonView.RPC("RemoveBuff", RpcTarget.All, buffName);
    }

    public void ReleaseBuff(CharacterModel model)
    {
        if (allyTargets.Contains(model))
        {
            allyTargets.Remove(model);
        }
    }

    public void ReleaseBuff(EnemyModel model)
    {
        if (enemyTargets.Contains(model))
        {
            enemyTargets.Remove(model);
        }
    }

    #region BuffCase
    private void SingleEnemyBuff(CharacterSkill owner)
    {
        List<Target> targetInfo = new List<Target>();
        List<EnemyModel> targets = new List<EnemyModel>();

        foreach (EnemyModel enemy in EnemyManager.Instance.enemies)
        {
            float distance = Vector3.Distance(owner.attack.transform.position, enemy.transform.position);

            if (distance < owner.attack.applyAttackRange)
            {
                Target newTarget = new Target(enemy, distance);
                targetInfo.Add(newTarget);
            }
        }

        if (targetInfo.Count > 0)
        {
            targetInfo = targetInfo.OrderBy(a => a.distance).ToList();
            for (int i = 0; i < targetInfo.Count; i++)
            {
                targets.Add(targetInfo[i].enemyModel);
            }
        }

        int applyTargetNumber = this.targetNumber + owner.attack.targetNumberIncrease;
        applyTargetNumber = Mathf.Clamp(applyTargetNumber, 0, targets.Count);

        for (int i = 0; i < applyTargetNumber; i++)
        {
            if (targets[i] != null
                && targets[i].gameObject.activeSelf == true)
            {
                ApplyBuff(owner, targets[i]);
            }
        }
    }

    private void SingleAllyBuff(CharacterSkill owner)
    {
        foreach (CharacterModel model in CharacterManager.Instance.wholeCharacters)
        {
            if (model != null
                && model.gameObject.activeSelf == true)
            {
                ApplyBuff(owner, model);
            }
        }
    }

    private void SingleStrongestBuff(CharacterSkill owner)
    {
        List<CharacterModel> targets = CharacterManager.Instance.wholeCharacters.OrderByDescending(model => model.attack.applyDamage).ToList();

        int applyAllyTargetNumber = Mathf.Clamp(targetNumber + owner.attack.targetNumberIncrease, 0, targets.Count);
        for (int i = 0; i < applyAllyTargetNumber; i++)
        {
            if (targets[i] != null)
            {
                ApplyBuff(owner, targets[i]);
            }
        }
    }

    private void AreaEnemyBuff(CharacterSkill owner, EnemyModel target)
    {
        if (target == null
            || target.gameObject.activeSelf == false
            || target.poolCount != owner.targetPoolCount)
        {
            return;
        }

        float applyBuffArea = attackArea * owner.attack.attackAreaIncrease;

        Collider[] contectedColliders = Physics.OverlapSphere(target.transform.position, applyBuffArea, LayerMask.GetMask("Enemy"));

        foreach (Collider collider in contectedColliders)
        {
            EnemyModel enemy = collider.GetComponent<EnemyModel>();
            if (enemy != null
                && enemy.gameObject.activeSelf)
            {
                ApplyBuff(owner, enemy);
            }
        }
    }

    private void AreaAllyBuff(CharacterSkill owner, CharacterModel target)
    {

        float applyBuffArea = attackArea * owner.attack.attackAreaIncrease;

        Collider[] contectedColliders = Physics.OverlapSphere(target.transform.position, applyBuffArea, LayerMask.GetMask("Character"));

        foreach (Collider collider in contectedColliders)
        {
            CharacterModel ally = collider.GetComponent<CharacterModel>();
            if (ally != null
                && ally.gameObject.activeSelf)
            {
                ApplyBuff(owner, ally);
            }
        }
    }

    #endregion
}