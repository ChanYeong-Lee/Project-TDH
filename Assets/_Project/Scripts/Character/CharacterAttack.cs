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

public class CharacterAttack : MonoBehaviour
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

    public List<Target> targets;
    public Target mainTarget;

    private bool attackPrepared;
    private bool haveTarget;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        attackPrepared = attackTimeout <= 0.0f;
        if(attackPrepared == false) attackTimeout -= Time.deltaTime;

        float range = attackRange * attackRangeIncrease;

        haveTarget = false;
        foreach (EnemyModel model in EnemyManager.Instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, model.transform.position);
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

        targets.Clear();
        mainTarget = null;
        
        float applyAttackDelayIncrease = Mathf.Clamp(attackDelayIncrease, 0.1f, attackDelayIncrease);

        float applyAttackDelay = attackDelay / applyAttackDelayIncrease;
        applyAttackDelay = Mathf.Clamp(applyAttackDelay, 0.1f, applyAttackDelay);
        attackTimeout = applyAttackDelay;

        float applyAttackSpeed = attackSpeed * attackSpeedIncrease;
        applyAttackSpeed = Mathf.Clamp(applyAttackSpeed, 0.1f, applyAttackDelay);
        
        animator.SetFloat("AttackSpeed", attackClip.length / applyAttackSpeed);
        animator.SetTrigger("Attack");

        float range = attackRange * attackRangeIncrease;
        foreach (EnemyModel enemy in EnemyManager.Instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < range)
            {
                Target target = new Target(enemy, distance);
                targets.Add(target);
            }
        }

        if (targets.Count > 0)
        {
            targets = targets.OrderBy(a => a.distance).ToList();
            mainTarget = targets[0];
        }
    }

    public void StopAttack()
    {
        animator.SetTrigger("Cancel");

        if (isAttacking)
        {
            attackTimeout = 0.1f;
            isAttacking = false;
        }
    }

    public void OnAttack(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight < 0.9f)
        {
            print("Cannot Attack");
            return;
        }
        print("Can Attack");

        isAttacking = false;

        switch (type)
        {
            case AttackType.Single:
                SingleAttack(targets);
                break;
            case AttackType.Area:
                AreaAttack(targets[0]);
                break;
        }
    }

    private void SingleAttack(List<Target> targets) // 단일 공격
    {
        if (targets == null)
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
            if (targets[i].model != null)
            {
                targets[i].model.health.GetComponent<PhotonView>().RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);
            }
        }
    }
    
    private void AreaAttack(Target target) // 범위 공격
    {
        if (target.model == null)
        {
            return;
        }

        float damage = this.damage * damageIncrease;
        float trueDamage = damage * trueDamagePercent * trueDamagePercentIncrease;
        float normalDamage = damage - trueDamage;

        float area = attackArea * attackAreaIncrease;
        Collider[] contectedColliders = Physics.OverlapSphere(target.model.transform.position, area);

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
