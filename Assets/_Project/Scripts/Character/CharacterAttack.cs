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
    private const string projectilePath = "Prefabs/Projectiles/";
    private Animator animator;
    [HideInInspector] public CharacterSkill skill;

    public Transform shotPoint;

    [Header("상태")]
    public AttackType attackType;
    public AnimationClip attackClip;
    public Projectile projectilePrefab;

    public bool isAttacking;

    public float applyDamage => damage * damageIncrease;
    public float damage;            // 캐릭터 기본 스탯 + 생성 변수
    public float damageIncrease;    // 기본값 = 1.0, 버프에 따라 변화

    public float applyTrueDamagePercent => Mathf.Clamp(trueDamagePercent + trueDamagePercentIncrease, 0.0f, 1.0f);
    public float trueDamagePercent;
    public float trueDamagePercentIncrease;

    public float applyAttackSpeed => Mathf.Clamp(applyAttackDelay + 0.1f, 0.1f, applyAttackDelay + 0.1f); // 한 공격이 끝마치는데 걸리는 시간
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
    public int mainTargetPoolCount;
    [HideInInspector]

    public bool canAttack;
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

        if (attackDelayDelta > 0.0f)
        {
            attackDelayDelta -= Time.deltaTime;
        }
        canAttack = attackDelayDelta <= 0.0f;
    }

    #region STATS
    public void SetAttackStats(CharacterSO defaultStat)
    {
        this.attackType = defaultStat.attackType;
        this.attackClip = defaultStat.attackClip;
        this.projectilePrefab = defaultStat.projectilePrefab;

        this.damage = defaultStat.defaultDamage;
        this.trueDamagePercent = defaultStat.defaultTrueDamagePercent;
        this.attackDelay = defaultStat.defaultAttackDelay;
        this.attackArea = defaultStat.defaultAttackArea;
        this.attackRange = defaultStat.defaultAttackRange;
        this.attackSpeed = defaultStat.defaultAttackSpeed;
        this.targetNumber = defaultStat.defaultTargetNumber;

        damageIncrease = 1.0f;
        attackDelayIncrease = 1.0f;
        attackAreaIncrease = 1.0f;
        attackRangeIncrease = 1.0f;
        trueDamagePercentIncrease = 0.0f;

        targetNumberIncrease = 0;
    }

    public void AddCrystal(int color)
    {
        switch (color)
        {
            case 0:
                damage += damage * 0.1f;
                attackDelay = attackDelay / 1.1f;
                break;
            case 1:
                attackRangeIncrease += 0.1f;
                attackAreaIncrease += 0.1f;
                targetNumberIncrease += 1;
                break;
        }
    }
#endregion

    // 공격 범위 내에 적이 한명이라도 있는지 확인
    public bool CheckTarget()
    {
        foreach (EnemyModel enemy in EnemyManager.Instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < applyAttackRange)
            {
                return true;
            }
        }
        return false;
    }

    public void StartSkill()
    {
        isAttacking = true;
        attackDelayDelta = applyAttackDelay;
    }

    // 공격을 시작하며 모든 적들을 순회하며 공격 범위 내의 적들 중 가장 가까운 적을 찾는다.
    public void StartAttack()
    {
        isAttacking = true;
        attackDelayDelta = applyAttackDelay;

        mainTarget = null;
        float minimumDistance = Mathf.Infinity;
        foreach (EnemyModel enemy in EnemyManager.Instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minimumDistance)
            {
                mainTarget = enemy;
                mainTargetPoolCount = mainTarget.poolCount;
                minimumDistance = distance;
            }
        }
        
        animator.SetFloat("AttackSpeed", attackClip.length / applyAttackSpeed);
        photonView.RPC("SetTriggerRPC", RpcTarget.All, "Attack");
    }

    public void CancelAttack()
    {
        isAttacking = false;
        attackDelayDelta = 0.1f;
    }

    private void SingleAttack() // 단일 공격
    {
        if (mainTarget == null 
            || mainTarget.gameObject.activeSelf == false
            || mainTargetPoolCount != mainTarget.poolCount)
        {
            return;
        }

        List<Target> targetInfo = new List<Target>();
        List<EnemyModel> targets = new List<EnemyModel>();
        for (int i = 0; i < EnemyManager.Instance.enemies.Count; i++)
        {
            EnemyModel enemy = EnemyManager.Instance.enemies[i];
            if (enemy == null 
                || enemy.gameObject.activeSelf == false)
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
            for (int i = 0; i < targetInfo.Count; i++)
            {
                targets.Add(targetInfo[i].enemyModel);
            }
        }

        float normalDamage = applyDamage;
        float trueDamage = applyDamage * applyTrueDamagePercent;

        int targetNumber = Mathf.Clamp(applyTargetNumber, 0, targets.Count);
        for (int i = 0; i < targetNumber; i++)
        {
            if (targets[i] == null 
                || targets[i].gameObject.activeSelf == false)
            {
                targetNumber = Mathf.Clamp(targetNumber + 1, 0, targets.Count);
                continue;
            }

            if (projectilePrefab != null)
            {
                Vector3 spawnPosition = shotPoint.position;
                Quaternion spawnRotation = shotPoint.rotation;

                int targetViewID = targets[i].photonView.ViewID;
                Vector3 destination = targets[i].transform.position + Vector3.up;

                photonView.RPC("ShotProjectileRPC", RpcTarget.All, 
                    projectilePrefab.name, spawnPosition, spawnRotation, 
                    targetViewID, targets[i].poolCount, destination, 
                    attackType, normalDamage, trueDamage, 0.0f);
            }
            else
            {
                targets[i].health.photonView.RPC("TakeHitRPC", RpcTarget.All, photonView.ViewID, targets[i].poolCount, normalDamage, trueDamage);
            }
        }
    }

    private void AreaAttack() // 범위 공격
    {
        // 설정 이슈
        // 만약 범위 공격을 시작했는데 중간에 Target이 없어지면
        // 1. 그 위치에 공격을 하도록 할지
        // 2. 취소를 할지
        // 3. 애니메이션만 진행하고 아무도 데미지를 받지 않게 할지 << 현재 선택된 방법

        if (mainTarget == null 
            || mainTarget.gameObject.activeSelf == false
            || mainTargetPoolCount != mainTarget.poolCount)
        {
            return;
        }

        float trueDamage = applyDamage * applyTrueDamagePercent;
        float normalDamage = applyDamage - trueDamage;

        if (projectilePrefab != null)
        {
            Vector3 spawnPosition = shotPoint.position;
            Quaternion spawnRotation = shotPoint.rotation;

            int targetViewID = mainTarget.photonView.ViewID;

            Vector3 destination = mainTarget.transform.position + Vector3.up;

            photonView.RPC("ShotProjectileRPC", RpcTarget.All,
                projectilePrefab.name, spawnPosition, spawnRotation,
                targetViewID, mainTarget.poolCount, destination,
                attackType, normalDamage, trueDamage, applyAttackArea);
        }
        else
        {
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

                    enemy.health.photonView.RPC("TakeHitRPC", RpcTarget.All, photonView.ViewID, enemy.poolCount, normalDamage, trueDamage);
                }
            }
        }
    }

    public void OnAttack(AnimationEvent animationEvent)
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (isAttacking)
        {
            isAttacking = false;

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
    }

    [PunRPC]
    public void SetTriggerRPC(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    [PunRPC]
    public void ShotProjectileRPC
        (string prefabName, Vector3 spawnPosition, Quaternion spawnRotation, 
        int targetViewID, int targetPoolCount, Vector3 destination, 
        AttackType attackType, float normalDamage, float trueDamage, float attackArea)
    {
        GameObject projectilePrefab = Resources.Load<GameObject>(projectilePath + prefabName);
        Projectile projectileInstance = PoolManager.Instance.clientPool.Spawn(projectilePrefab, spawnPosition, spawnRotation).GetComponent<Projectile>();
        EnemyModel target = EnemyManager.Instance.enemies.Find((model) => model.photonView.ViewID == targetViewID);

        if (target != null)
        {
            projectileInstance.target = target;
            projectileInstance.owner = this;
            projectileInstance.targetPoolCount = targetPoolCount;
            projectileInstance.destination = destination;
            projectileInstance.attackType = attackType;
            projectileInstance.normalDamage = normalDamage;
            projectileInstance.trueDamage = trueDamage;
            projectileInstance.attackArea = attackArea;
        }
        else
        {
            PoolManager.Instance.clientPool.Despawn(projectileInstance.gameObject);
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
