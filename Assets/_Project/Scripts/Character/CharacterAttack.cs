using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public enum AttackType
{
    Single, // 정해진 객체 수 만큼 공격
    Area    // 타겟 주위 정해진 범위만큼 공격
}

public class CharacterAttack : MonoBehaviourPun, IPunObservable
{
    private Animator animator;
    private CharacterSkill skill;

    [Header("상태")]
    public AttackType attackType;
    public AnimationClip attackClip;
    
    public bool canAttack;
    public bool isAttacking;

    public float applyDamage => damage * damageIncrease;
    public float damage;            // 캐릭터 기본 스탯 + 생성 변수
    public float damageIncrease;    // 기본값 = 1.0, 버프에 따라 변화

    public float applyTrueDamagePercent => Mathf.Clamp(trueDamagePercent + trueDamagePercentIncrease, 0.0f, 1.0f);
    public float trueDamagePercent;
    public float trueDamagePercentIncrease;

    public float applyAttackSpeed  => Mathf.Clamp(attackSpeed, 0.1f, applyAttackDelay); // 한 공격이 끝마치는데 걸리는 시간
    public float attackSpeed; // 기본 공격 속도

    public float applyAttackDelay => Mathf.Clamp(attackDelay / Mathf.Clamp(attackDelayIncrease, 0.1f, attackDelayIncrease), 0.1f, attackDelay / Mathf.Clamp(attackDelayIncrease, 0.1f, attackDelayIncrease));
    public float attackDelay;
    public float attackDelayIncrease = 1.0f;
    private float attackDelayDelta;

    public float applyAttackRange => Mathf.Clamp(attackRange * attackRangeIncrease, 0.0f, attackRange * attackRangeIncrease);
    public float attackRange;
    public float attackRangeIncrease = 1.0f;

    public int applyTargetNumber => targetNumber + targetNumberIncrease;
    public int targetNumber;
    public int targetNumberIncrease; // 기본값 = 0, 버프에 따라 추가

    public float applyAttackArea => attackArea * attackAreaIncrease;    // 범위 공격의 크기
    public float attackArea;
    public float attackAreaIncrease = 1.0f;

    public EnemyModel mainTarget;
    [HideInInspector]
    public List<EnemyModel> targets;

    private bool attackPrepared;
    private bool haveTarget;
    private bool skillCasting;

    private void Awake()
    {
        skill = GetComponent<CharacterSkill>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

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

        skillCasting = skill.isCasting;

        canAttack = attackPrepared && haveTarget && (skillCasting == false);
    }

    public void SetAttackStats(CharacterSO defaultStat)
    {
        this.attackType = defaultStat.attackType;
        this.attackClip = defaultStat.attackClip;

        this.damage = defaultStat.defaultDamage;
        this.attackDelay = defaultStat.defaultAttackDelay;
        this.attackArea = defaultStat.defaultAttackArea;
        this.attackRange = defaultStat.defaultAttackRange;
        this.attackSpeed = defaultStat.defaultAttackSpeed;
        this.targetNumber = defaultStat.defaultTargetNumber;

        damageIncrease = 1.0f;
        attackDelayIncrease = 1.0f;
        attackAreaIncrease = 1.0f;
        attackRangeIncrease = 1.0f;

        targetNumberIncrease = 0;
    }

    public void AddCrystal(Vector3Int crystals)
    {
        damageIncrease += 0.1f * crystals.x;
        attackDelayIncrease += 0.1f * crystals.x;

        attackAreaIncrease += 0.1f * crystals.y;
        targetNumberIncrease += crystals.y;
    }

    public void StartSkill()
    {
        isAttacking = true;
        attackDelayDelta = applyAttackDelay;
    }

    public void StartAttack()
    {
        isAttacking = true;

        mainTarget = null;
        targets.Clear();

        attackDelayDelta = applyAttackDelay;

        animator.SetFloat("AttackSpeed", attackClip.length / applyAttackSpeed);
        photonView.RPC("SetTriggerRPC", RpcTarget.All, "Attack");

        List<Target> targetInfo = new List<Target>();
        for(int i = 0; i<EnemyManager.Instance.enemies.Count; i++)
        {
            EnemyModel enemy = EnemyManager.Instance.enemies[i];
            if (enemy == null || enemy.gameObject.activeSelf == false)
            {
                continue;
            }

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

    


    public void CancelAttack()
    {
        attackDelayDelta = 0.1f;
        isAttacking = false;
    }

    private void SingleAttack() // 단일 공격
    {
        if (mainTarget == null)
        {
            return;
        }

        float trueDamage = applyDamage * applyTrueDamagePercent;
        float normalDamage = applyDamage - trueDamage;

        int targetNumber = Mathf.Clamp(applyTargetNumber, 0, targets.Count);
        for (int i = 0; i < targetNumber; i++)
        {
            if (targets[i] == null || targets[i].gameObject.activeSelf == false)
            {
                continue;
            }

            targets[i].health.photonView.RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);
        }
    }
    
    private void AreaAttack() // 범위 공격
    {
        if (mainTarget == null)
        {
            return;
        }

        float trueDamage = applyDamage * applyTrueDamagePercent;
        float normalDamage = applyDamage - trueDamage;

        Collider[] contectedColliders = Physics.OverlapSphere(mainTarget.transform.position, applyAttackArea);

        foreach (Collider collider in contectedColliders)
        {
            if (collider.tag == "Enemy")
            {
                EnemyModel enemy = collider.GetComponent<EnemyModel>();

                if (enemy == null || enemy.gameObject.activeSelf == false)
                {
                    continue;
                }

                enemy.health.photonView.RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);
            }
        }
    }

    public void OnAttack(AnimationEvent animationEvent)
    {
        print("ATTACK!");
        if (animationEvent.animatorClipInfo.weight < 0.8f)
        {
            return;
        }

        isAttacking = false;

        if (photonView.IsMine == false)
        {
            return;
        }

        switch (attackType)
        {
            case AttackType.Single:
                SingleAttack();
                break;
            case AttackType.Area:
                AreaAttack();
                break;
        }
    }

    [PunRPC]
    public void SetTriggerRPC(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(damage);
            stream.SendNext(damageIncrease);
        }
        else
        {
            damage = (float)stream.ReceiveNext();
            damageIncrease = (float)stream.ReceiveNext();
        }
    }
}
