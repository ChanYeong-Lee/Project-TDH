using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterSkill : MonoBehaviourPun
{
    private const string projectilePath = "Prefabs/Projectiles/";
    private const string particlePath = "Prefabs/Particles/";

    private Animator animator;
    [HideInInspector] public CharacterAttack attack;
    [HideInInspector] public Transform skillParent;

    [Header("ป๓ลย")]
    public List<Skill> skills;
    public Skill currentSkill;
    public bool isCasting;
    public Transform mainTarget;
    public int targetPoolCount;

    public float cooldownIncrease;
    public float percentageIncrease;

    public void Init()
    {
        attack = GetComponent<CharacterAttack>();
        animator = GetComponent<Animator>();
        skillParent = transform.Find("Skills");

        foreach (Transform skillTransform in skillParent)
        {
            Skill newSkill= skillTransform.GetComponent<Skill>();
            if (newSkill != null)
            {
                print($"get {newSkill.defaultStat.skillName} skill");
                skills.Add(newSkill);
            }
        }
    }

    public void SetSkillStats()
    {
        cooldownIncrease = 1.0f;
        percentageIncrease = 0.0f;
    }

    public void AddCrystal(int color)
    {
        if (color == 2)
        {
            cooldownIncrease += 0.1f;
            percentageIncrease += 0.05f;
        }
    }

    public bool CheckNonTargetCooldownSkill(out Skill readySkill)
    {
        List<Skill> readySkills = new List<Skill>();
        foreach (Skill skill in skills)
        {
            if (skill.defaultStat.skillType == SkillType.NonTargetCooldown)
            {
                if (skill.isReady)
                {
                    readySkills.Add(skill);
                }
            }
        }

        if (readySkills.Count > 0)
        {
            readySkills = readySkills.OrderBy(skill => skill.defaultStat.priority).ToList();
            readySkill = readySkills[0];
            return true;
        }

        readySkill = null;
        return false;
    }

    public bool CheckTargetCooldownSkill(out Skill readySkill)
    {
        List<Skill> readySkills = new List<Skill>();
        foreach (Skill skill in skills)
        {
            if (skill.defaultStat.skillType == SkillType.TargetCooldown)
            {
                if (skill.isReady)
                {
                    readySkills.Add(skill);
                }
            }
        }

        if (readySkills.Count > 0)
        {
            readySkills = readySkills.OrderBy(skill => skill.defaultStat.priority).ToList();
            readySkill = readySkills[0];
            return true;
        }

        readySkill = null;
        return false;
    }

    public bool CheckAttackSkill(out Skill activatedSkill)
    {
        List<Skill> activatedSkills = new List<Skill>();

        foreach (Skill skill in skills)
        {
            if (skill.defaultStat.skillType == SkillType.Random)
            {
                float randomValue = Random.Range(0.0f, 1.0f);

                if (randomValue < skill.applyPercentage)
                {
                    activatedSkills.Add(skill);
                }
            }
        }

        if (activatedSkills.Count > 0)
        {
            activatedSkills = activatedSkills.OrderBy(skill => skill.defaultStat.priority).ToList();
            activatedSkill = activatedSkills[0];
            return true;
        }

        activatedSkill = null;
        return false;
    }

    public void StartSkill(Skill skill)
    {
        currentSkill = skill;
        mainTarget = currentSkill.SetTarget(this);

        if (mainTarget == null)
        {
            return;
        }

        isCasting = true;
        
        if (mainTarget.TryGetComponent(out EnemyModel enemyTarget))
        {
            targetPoolCount = enemyTarget.poolCount;
        }

        currentSkill.StartSkill();

        float applySkillSpeed = Mathf.Clamp(skill.defaultStat.skillSpeed, 0.1f, attack.applyAttackDelay + 0.1f);

        animator.SetFloat("SkillSpeed", skill.defaultStat.skillClip.length / applySkillSpeed);
        photonView.RPC("SkillTriggerRPC", RpcTarget.All, skill.defaultStat.triggerName);
    }

    [PunRPC]
    public void SkillTriggerRPC(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    [PunRPC]
    public void ShotSkillProjectileRPC(string prefabName, Vector3 spawnPosition, Quaternion spawnRotation, int targetViewID, int targetPoolCount, Vector3 destination, AttackType attackType, float normalDamage, float trueDamage, float attackArea)
    {
        Projectile projectilePrefab = Resources.Load<Projectile>(projectilePath + prefabName);
        Projectile projectileInstance = PoolManager.Instance.clientPool.Spawn(projectilePrefab.gameObject, spawnPosition, spawnRotation).GetComponent<Projectile>();

        EnemyModel target = EnemyManager.Instance.enemies.Find((model) => model.photonView.ViewID == targetViewID);

        if (target != null)
        {
            projectileInstance.target = target;
            projectileInstance.owner = attack;
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

    [PunRPC]
    public void ShotSkillParticleRPC(string prefabName, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        ParticleSystem particlePrefab = Resources.Load<ParticleSystem>(particlePath + prefabName);
        ParticleSystem particleInstance = PoolManager.Instance.clientPool.Spawn(particlePrefab.gameObject, spawnPosition, spawnRotation).GetComponent<ParticleSystem>();
    }

    public void OnSkill(AnimationEvent animationEvent)
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (currentSkill == null)
        {
            return;
        }

        if (isCasting)
        {
            isCasting = false;
            currentSkill.OnSkill(this);
        }
    }

    public void CancelSkill()
    {
        isCasting = false;
        currentSkill.CancelSkill();
    }
}