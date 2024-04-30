using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackEffect 
{
    public AttackType attackType;

    public float fixedDamage;       // 스킬 기본 데미지
    public float relativeDamagePercent; // 스탯 비례계수

    public int targetNumber;
    public float attackArea;

    public void Execute(CharacterSkill owner, List<EnemyModel> targets)
    {
        if (owner == null || targets == null)
        {
            return;
        }

        switch (attackType)
        {
            case AttackType.Single:
                SingleAttack(owner, targets);
                break;
            case AttackType.Area:
                AreaAttack(owner, targets[0]);
                break;
        }
    }

    private float CalculateDamage(float ownerDamage)
    {
        return fixedDamage + (ownerDamage * relativeDamagePercent);
    }

    private void SingleAttack(CharacterSkill owner, List<EnemyModel> targets) // 단일 공격
    {
        float applyDamage = CalculateDamage(owner.attack.applyDamage);

        float trueDamage = applyDamage * owner.attack.applyTrueDamagePercent;
        float normalDamage = applyDamage - trueDamage;

        int applyTargetNumber = this.targetNumber + owner.attack.targetNumberIncrease;
        applyTargetNumber = Mathf.Clamp(applyTargetNumber, 0, targets.Count);
        for (int i = 0; i < applyTargetNumber; i++)
        {
            targets[i].health.GetComponent<PhotonView>().RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);
        }
    }

    private void AreaAttack(CharacterSkill owner, EnemyModel target) // 범위 공격
    {
        float applyDamage = CalculateDamage(owner.attack.applyDamage);

        float trueDamage = applyDamage * owner.attack.applyTrueDamagePercent;
        float normalDamage = applyDamage - trueDamage;

        float applyAttackArea = attackArea * owner.attack.attackAreaIncrease;

        Collider[] contectedColliders = Physics.OverlapSphere(target.transform.position, applyAttackArea);

        foreach (Collider collider in contectedColliders)
        {
            if (collider.tag == "Enemy")
            {
                EnemyModel enemy = collider.GetComponent<EnemyModel>();
                enemy.health.GetComponent<PhotonView>().RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);
            }
        }
    }
}
