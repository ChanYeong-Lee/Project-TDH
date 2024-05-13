using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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


    public void Execute(CharacterSkill owner, List<CharacterModel> allyTargets, List<EnemyModel> enemyTargets)
    {
        switch (attackType)
        {
            case AttackType.Single:
                switch (targetType)
                {
                    case TargetType.Enemy:
                        int applyEnemyTargetNumber = Mathf.Clamp(targetNumber + owner.attack.targetNumberIncrease, 0, enemyTargets.Count);
                        for (int i = 0; i < applyEnemyTargetNumber; i++)
                        {
                            if (enemyTargets[i] != null && enemyTargets[i].gameObject.activeSelf == true)
                            {
                                ApplyBuff(owner, enemyTargets[i]);
                            }
                        }
                        break;
                    case TargetType.AllAlly:
                        foreach (CharacterModel model in allyTargets)
                        {
                            ApplyBuff(owner, model);
                        }
                        break;
                    case TargetType.StrongestAlly:
                        int applyAllyTargetNumber = Mathf.Clamp(targetNumber + owner.attack.targetNumberIncrease, 0, allyTargets.Count);
                        for (int i = 0; i < applyAllyTargetNumber; i++)
                        {
                            if (allyTargets[i] != null)
                            {
                                ApplyBuff(owner, allyTargets[i]);
                            }
                        }
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
                        float applyAttackArea = attackArea * owner.attack.attackAreaIncrease;

                        Collider[] contectedColliders = Physics.OverlapSphere(enemyTargets[0].transform.position, applyAttackArea);

                        foreach (Collider collider in contectedColliders)
                        {
                            if (collider.tag == "Enemy")
                            {
                                EnemyModel enemy = collider.GetComponent<EnemyModel>();
                                ApplyBuff(owner, enemy);
                            }
                        }
                        break;
                    case TargetType.AllAlly:
                        float applyBuffArea = attackArea * owner.attack.attackAreaIncrease;
                        foreach (CharacterModel model in allyTargets)
                        {
                            if (Vector3.Distance(owner.transform.position, model.transform.position) < applyBuffArea)
                            {
                                ApplyBuff(owner, model);
                            }
                        }
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

}