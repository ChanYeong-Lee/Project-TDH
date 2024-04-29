using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public enum AttackType
{
    Single, // ������ ��ü �� ��ŭ ����
    Area    // Ÿ�� ���� ������ ������ŭ ����
}

public class CharacterAttack : MonoBehaviourPun
{
    private Animator animator;

    public AttackType type;
    public AnimationClip attackClip;
    public bool canAttack;
    public bool isAttacking;

    public float applyDamage => damage * damageIncrease;
    public float damage;            // ĳ���� �⺻ ���� + ���� ����
    public float damageIncrease = 1.0f;    // �⺻�� = 1.0, ������ ���� ��ȭ

    public float applyTrueDamagePercent => trueDamagePercent * trueDamagePercentIncrease;
    public float trueDamagePercent;
    public float trueDamagePercentIncrease = 1.0f;

    public float applyAttackSpeed  => Mathf.Clamp(attackSpeed * attackSpeedIncrease, 0.1f, applyAttackDelay); // �� ������ ����ġ�µ� �ɸ��� �ð�
    public float attackSpeed;
    public float attackSpeedIncrease = 1.0f;

    public float applyAttackDelay => Mathf.Clamp(attackDelay / Mathf.Clamp(attackDelayIncrease, 0.1f, attackDelayIncrease), 0.1f, attackDelay / Mathf.Clamp(attackDelayIncrease, 0.1f, attackDelayIncrease));
    public float attackDelay;
    public float attackDelayIncrease = 1.0f;
    private float attackDelayDelta;

    public float applyAttackRange => attackRange * attackRangeIncrease;
    public float attackRange;
    public float attackRangeIncrease = 1.0f;

    public int applyTargetNumber => targetNumber + targetNumberIncrease;
    public int targetNumber;
    public int targetNumberIncrease; // �⺻�� = 0, ������ ���� �߰�

    public float applyAttackArea => attackArea * attackAreaIncrease;    // ���� ������ ũ��
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
            attackDelayDelta -= Time.deltaTime;
        }
        attackPrepared = attackDelayDelta <= 0.0f;

        haveTarget = false;
        foreach (EnemyModel enemy in EnemyManager.Instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < applyAttackRange)
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

        attackDelayDelta = applyAttackDelay;

        animator.SetFloat("AttackSpeed", attackClip.length / applyAttackSpeed);
        photonView.RPC("AttackTriggerRPC", RpcTarget.All);

        List<Target> targetInfo = new List<Target>();
        foreach (EnemyModel enemy in EnemyManager.Instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < applyAttackRange)
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
        attackDelayDelta = 0.1f;
        isAttacking = false;
    }

    private void SingleAttack() // ���� ����
    {
        if (mainTarget == null)
        {
            return;
        }

        float trueDamage = applyDamage * applyTrueDamagePercent;
        float normalDamage = damage - trueDamage;

        int targetNumber = this.targetNumber + targetNumberIncrease;
        targetNumber = Mathf.Clamp(targetNumber, 0, targets.Count);
        for (int i = 0; i < targetNumber; i++)
        {
            targets[i].health.GetComponent<PhotonView>().RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);
        }
    }
    
    private void AreaAttack() // ���� ����
    {
        if (mainTarget == null)
        {
            return;
        }

        float damage = this.damage * damageIncrease;
        float trueDamage = damage * trueDamagePercent * trueDamagePercentIncrease;
        float normalDamage = damage - trueDamage;

        Collider[] contectedColliders = Physics.OverlapSphere(mainTarget.transform.position, applyAttackArea);

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
