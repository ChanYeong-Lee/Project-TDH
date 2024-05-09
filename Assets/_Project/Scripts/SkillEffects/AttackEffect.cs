using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        float normalDamage = applyDamage;

        int applyTargetNumber = this.targetNumber + owner.attack.targetNumberIncrease;
        applyTargetNumber = Mathf.Clamp(applyTargetNumber, 0, targets.Count);

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

                owner.photonView.RPC("ShotSkillProjectileRPC", RpcTarget.All, prefabName, spawnPosition, spawnRotation, targetViewID, destination);
            }
            
            if (particlePrefab != null)
            {
                string prefabName = particlePrefab.name;
                Vector3 spawnPosition = targets[i].transform.position;
                Quaternion spawnRotation = Quaternion.identity;

                owner.photonView.RPC("ShotSkillParticleRPC", RpcTarget.All, prefabName, spawnPosition, spawnRotation);
            }

            targets[i].health.GetComponent<PhotonView>().RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);
        }
    }

    private void AreaAttack(CharacterSkill owner, EnemyModel target) // 범위 공격
    {
        if (target == null || target.gameObject.activeSelf == false)
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

            owner.photonView.RPC("ShotSkillProjectileRPC", RpcTarget.All, prefabName, spawnPosition, spawnRotation, targetViewID, destination);
        }

        if (particlePrefab != null)
        {
            string prefabName = particlePrefab.name;
            Vector3 spawnPosition = target.transform.position;
            Quaternion spawnRotation = Quaternion.identity;

            owner.photonView.RPC("ShotSkillParticleRPC", RpcTarget.All, prefabName, spawnPosition, spawnRotation);
        }

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
