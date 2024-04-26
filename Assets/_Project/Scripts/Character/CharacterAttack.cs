using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public enum AttackType
{
    Single, // 정해진 객체 수 만큼 공격
    Area    // 타겟 주위 정해진 범위만큼 공격
}

public class CharacterAttack : MonoBehaviourPun
{
    private Animator animator;

    public AttackType type;
    public AnimationClip attackClip;
    public bool canAttack;
    public bool isAttacking;

    public float damage;            // 캐릭터 기본 스탯 + 생성 변수
    public float damageIncrease = 1.0f;    // 기본값 = 1.0, 버프에 따라 변화

    public float trueDamagePercent;
    public float trueDamagePercentIncrease = 1.0f;

    public float attackSpeed;
    public float attackSpeedIncrease = 1.0f;

    public float attackDelay;
    public float attackDelayIncrease = 1.0f;
    private float attackTimeout;

    public float attackRange;
    public float attackRangeIncrease = 1.0f;

    public int targetNumber;
    public int targetNumberIncrease; // 기본값 = 0, 버프에 따라 추가

    public float attackArea;
    public float attackAreaIncrease = 1.0f;

    public EnemyModel mainTarget;
    public List<EnemyModel> targets;

    private bool attackPrepared;
    private bool haveTarget;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (attackPrepared == false)
        {
            attackTimeout -= Time.deltaTime;
        }
        attackPrepared = attackTimeout <= 0.0f;

        float range = attackRange * attackRangeIncrease;
        haveTarget = false;
        foreach (EnemyModel enemy in EnemyManager.Instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < range)
            {
                haveTarget = true;
                break;
            }
            else
            {
                haveTarget = false;
            }
        }

        canAttack = attackPrepared && haveTarget;
    }

    public void StartAttack()
    {
        isAttacking = true;

        mainTarget = null;
        targets.Clear();
        
        float applyAttackDelayIncrease = Mathf.Clamp(attackDelayIncrease, 0.1f, attackDelayIncrease);

        float applyAttackDelay = attackDelay / applyAttackDelayIncrease;
        applyAttackDelay = Mathf.Clamp(applyAttackDelay, 0.1f, applyAttackDelay);
        attackTimeout = applyAttackDelay;

        float applyAttackSpeed = attackSpeed * attackSpeedIncrease;
        applyAttackSpeed = Mathf.Clamp(applyAttackSpeed, 0.1f, applyAttackDelay);

        animator.SetFloat("AttackSpeed", attackClip.length / applyAttackSpeed);
        photonView.RPC("AttackTriggerRPC", RpcTarget.All);

        List<Target> targetInfo = new List<Target>();
        float range = attackRange * attackRangeIncrease;
        foreach (EnemyModel enemy in EnemyManager.Instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < range)
            {
                Target target = new Target(enemy, distance);
                targetInfo.Add(target);
            }
        }

        if (targetInfo.Count > 0)
        {
            targetInfo = targetInfo.OrderBy(a => a.distance).ToList();
            targets = new List<EnemyModel>();
            foreach (Target target in targetInfo)
            {
                targets.Add(target.model);
            }
            mainTarget = targets[0];
        }
    }

    [PunRPC]
    public void AttackTriggerRPC()
    {
        animator.SetTrigger("Attack");
    }

    public void OnAttack(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight < 0.9f)
        {
            return;
        }

        isAttacking = false;

        if (photonView.IsMine == false)
        {
            return;
        }

        switch (type)
        {
            case AttackType.Single:
                SingleAttack();
                break;
            case AttackType.Area:
                AreaAttack();
                break;
        }
    }

    public void CancelAttack()
    {
        attackTimeout = 0.1f;
        isAttacking = false;
    }

    private void SingleAttack() // 단일 공격
    {
        if (mainTarget == null)
        {
            return;
        }

        float damage = this.damage * damageIncrease;
        float trueDamage = damage * trueDamagePercent * trueDamagePercentIncrease;
        float normalDamage = damage - trueDamage;

        int targetNumber = this.targetNumber + targetNumberIncrease;
        targetNumber = Mathf.Clamp(targetNumber, 0, targets.Count);
        for (int i = 0; i < targetNumber; i++)
        {
            targets[i].health.GetComponent<PhotonView>().RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);
        }
    }
    
    private void AreaAttack() // 범위 공격
    {
        if (mainTarget == null)
        {
            return;
        }

        float damage = this.damage * damageIncrease;
        float trueDamage = damage * trueDamagePercent * trueDamagePercentIncrease;
        float normalDamage = damage - trueDamage;

        float area = attackArea * attackAreaIncrease;
        Collider[] contectedColliders = Physics.OverlapSphere(mainTarget.transform.position, area);

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
