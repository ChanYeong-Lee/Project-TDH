using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public enum AttackType
{
    Single, // ������ ��ü �� ��ŭ ����
    Area    // Ÿ�� ���� ������ ������ŭ ����
}

public class CharacterAttack : MonoBehaviourPun, IPunObservable
{
    private const string projectilePath = "Prefabs/Projectiles/";
    private Animator animator;
    private CharacterSkill skill;

    public Transform shotPoint;

    [Header("����")]
    public AttackType attackType;
    public AnimationClip attackClip;
    public Projectile projectilePrefab;

    public bool canAttack;
    public bool isAttacking;

    public float applyDamage => damage * damageIncrease;
    public float damage;            // ĳ���� �⺻ ���� + ���� ����
    public float damageIncrease;    // �⺻�� = 1.0, ������ ���� ��ȭ

    public float applyTrueDamagePercent => Mathf.Clamp(trueDamagePercent + trueDamagePercentIncrease, 0.0f, 1.0f);
    public float trueDamagePercent;
    public float trueDamagePercentIncrease;

    public float applyAttackSpeed => Mathf.Clamp(attackSpeed, 0.1f, applyAttackDelay + 0.1f); // �� ������ ����ġ�µ� �ɸ��� �ð�
    public float attackSpeed; // �⺻ ���� �ӵ�

    public float applyAttackDelay => Mathf.Clamp(attackDelay / Mathf.Clamp(attackDelayIncrease, 0.1f, attackDelayIncrease), 0.1f, attackDelay / Mathf.Clamp(attackDelayIncrease, 0.1f, attackDelayIncrease));
    public float attackDelay;
    public float attackDelayIncrease = 1.0f;
    private float attackDelayDelta;

    public float applyAttackRange => Mathf.Clamp(attackRange * attackRangeIncrease, 0.0f, attackRange * attackRangeIncrease);
    public float attackRange;
    public float attackRangeIncrease = 1.0f;

    public int applyTargetNumber => targetNumber + targetNumberIncrease;
    public int targetNumber;
    public int targetNumberIncrease; // �⺻�� = 0, ������ ���� �߰�

    public float applyAttackArea => attackArea * attackAreaIncrease;    // ���� ������ ũ��
    public float attackArea;
    public float attackAreaIncrease = 1.0f;

    public EnemyModel mainTarget;
    [HideInInspector]
    public List<EnemyModel> targets;

    private bool attackPrepared;
    private bool haveTarget;

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

        canAttack = attackPrepared && haveTarget;
    }

    public void SetAttackStats(CharacterSO defaultStat)
    {
        this.attackType = defaultStat.attackType;
        this.attackClip = defaultStat.attackClip;
        this.projectilePrefab = defaultStat.projectilePrefab;

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
        for (int i = 0; i < EnemyManager.Instance.enemies.Count; i++)
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

    private void SingleAttack() // ���� ����
    {
        if (mainTarget == null)
        {
            return;
        }

        float normalDamage = applyDamage;
        float trueDamage = applyDamage * applyTrueDamagePercent;

        int targetNumber = Mathf.Clamp(applyTargetNumber, 0, targets.Count);
        for (int i = 0; i < targetNumber; i++)
        {
            if (targets[i] == null || targets[i].gameObject.activeSelf == false)
            {
                continue;
            }

            if (projectilePrefab != null)
            {
                Vector3 spawnPosition = shotPoint.position;
                Quaternion spawnRotation = shotPoint.rotation;

                int targetViewID = targets[i].photonView.ViewID;
                Vector3 destination = targets[i].transform.position + Vector3.up;

                photonView.RPC("ShotProjectileRPC", RpcTarget.All, projectilePrefab.name, spawnPosition, spawnRotation, targetViewID, destination);
            }

            targets[i].health.photonView.RPC("TakeHitRPC", RpcTarget.All, normalDamage, trueDamage);
        }
    }

    private void AreaAttack() // ���� ����
    {
        if (mainTarget == null)
        {
            return;
        }

        float trueDamage = applyDamage * applyTrueDamagePercent;
        float normalDamage = applyDamage - trueDamage;

        Collider[] contectedColliders = Physics.OverlapSphere(mainTarget.transform.position, applyAttackArea);

        if (projectilePrefab != null)
        {
            Vector3 spawnPosition = shotPoint.position;
            Quaternion spawnRotation = shotPoint.rotation;

            int targetViewID = mainTarget.photonView.ViewID;

            Vector3 destination = mainTarget.transform.position + Vector3.up;

            photonView.RPC("ShotProjectileRPC", RpcTarget.All, projectilePrefab.name, spawnPosition, spawnRotation, targetViewID, destination);
        }

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

    [PunRPC]
    public void ShotProjectileRPC(string prefabName, Vector3 spawnPosition, Quaternion spawnRotation, int targetViewID, Vector3 destination)
    {
        Projectile projectilePrefab = Resources.Load<Projectile>(projectilePath + prefabName);
        Projectile projectileInstance = PoolManager.Instance.clientPool.Spawn(projectilePrefab.gameObject, spawnPosition, spawnRotation).GetComponent<Projectile>();

        EnemyModel target = EnemyManager.Instance.enemies.Find((model) => model.photonView.ViewID == targetViewID);

        if (target != null)
        {
            projectileInstance.target = target.transform;
            projectileInstance.destination = destination;
        }
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
