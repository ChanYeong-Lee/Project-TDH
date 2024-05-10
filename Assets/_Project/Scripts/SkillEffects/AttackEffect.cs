using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class AttackEffect 
{
    public AttackType attackType;
    public Projectile projectilePrefab;
    public ParticleSystem particlePrefab;

    public float fixedDamage;       // 스킬 기본 데미지
    public float relativeDamagePercent; // 스탯 비례계수

    public int targetNumber;
    public float attackArea;

    public void Execute(CharacterSkill owner, EnemyModel mainTarget)
    {
        if (owner == null || mainTarget == null)
        {
            return;
        }

        switch (attackType)
        {
            case AttackType.Single:
                SingleAttack(owner, mainTarget);
                break;
            case AttackType.Area:
                AreaAttack(owner, mainTarget);
                break;
        }
    }

   
    private void SingleAttack(CharacterSkill owner, EnemyModel target) // 단일 공격
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
                targets.Add(targetInfo[i].model);
            }
        }

        float applyDamage = CalculateDamage(owner.attack.applyDamage);

        float trueDamage = applyDamage * owner.attack.applyTrueDamagePercent;
        float normalDamage = applyDamage;

        int applyTargetNumber = this.targetNumber + owner.attack.targetNumberIncrease;
        applyTargetNumber = Mathf.Clamp(applyTargetNumber, 0, targets.Count);
        Debug.Log($"apply Target Number = {applyTargetNumber}");
        for (int i = 0; i < applyTargetNumber; i++)
        {
            if (targets[i] == null || targets[i].gameObject.activeSelf == false)
            {
                continue;
            }

            if (projectilePrefab != null)
            {
                string prefabName = projectilePrefab.name;
                Vector3 spawnPosition = owner.attack.shotPoint.position;
                Quaternion spawnRotation = owner.attack.shotPoint.rotation;
                int targetViewID = targets[i].photonView.ViewID;
                Vector3 destination = targets[i].transform.position + Vector3.up;

                owner.attack.photonView.RPC("ShotSkillProjectileRPC", RpcTarget.All, 
                    prefabName, spawnPosition, spawnRotation, 
                    targetViewID, targets[i].poolCount, destination,
                    attackType, normalDamage, trueDamage, 0.0f);
            }
            else
            {
                targets[i].health.GetComponent<PhotonView>().RPC("TakeHitRPC", RpcTarget.All, targets[i].poolCount, normalDamage, trueDamage);
            }
            
            if (particlePrefab != null)
            {
                string prefabName = particlePrefab.name;
                Vector3 spawnPosition = targets[i].transform.position;
                Quaternion spawnRotation = Quaternion.identity;

                owner.photonView.RPC("ShotSkillParticleRPC", RpcTarget.All, prefabName, spawnPosition, spawnRotation);
            }
        }
    }

    private void AreaAttack(CharacterSkill owner, EnemyModel target) // 범위 공격
    {
        if (target == null 
            || target.gameObject.activeSelf == false
            || target.poolCount != owner.targetPoolCount)
        {
            return;
        }

        float applyDamage = CalculateDamage(owner.attack.applyDamage);

        float trueDamage = applyDamage * owner.attack.applyTrueDamagePercent;
        float normalDamage = applyDamage;

        float applyAttackArea = attackArea * owner.attack.attackAreaIncrease;

        if (projectilePrefab != null)
        {
            string prefabName = projectilePrefab.name;
            Vector3 spawnPosition = owner.attack.shotPoint.position;
            Quaternion spawnRotation = owner.attack.shotPoint.rotation;
            int targetViewID = target.photonView.ViewID;
            Vector3 destination = target.transform.position + Vector3.up;

            owner.attack.photonView.RPC("ShotSkillProjectileRPC", RpcTarget.All,
                prefabName, spawnPosition, spawnRotation,
                targetViewID, target.poolCount, destination,
                attackType, normalDamage, trueDamage, applyAttackArea);
        }
        else
        {
            Collider[] contectedColliders = Physics.OverlapSphere(target.transform.position, applyAttackArea);

            foreach (Collider collider in contectedColliders)
            {
                if (collider.tag == "Enemy")
                {
                    EnemyModel enemy = collider.GetComponent<EnemyModel>();

                    if (enemy == null || enemy.gameObject.activeSelf == false)
                    {
                        continue;
                    }

                    enemy.health.photonView.RPC("TakeHitRPC", RpcTarget.All, enemy.poolCount, normalDamage, trueDamage);
                }
            }
        }

        if (particlePrefab != null)
        {
            string prefabName = particlePrefab.name;
            Vector3 spawnPosition = target.transform.position;
            Quaternion spawnRotation = Quaternion.identity;

            owner.photonView.RPC("ShotSkillParticleRPC", RpcTarget.All, prefabName, spawnPosition, spawnRotation);
        }
    }

    private float CalculateDamage(float ownerDamage)
    {
        return fixedDamage + (ownerDamage * relativeDamagePercent);
    }
}
