using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterSkill : MonoBehaviourPun
{
    private Animator animator;
    [HideInInspector] public CharacterAttack attack;
    [HideInInspector] public Transform skillParent;

    [Header("ป๓ลย")]
    public List<Skill> skills;
    public Skill currentSkill;
    public bool isCasting;
    public Transform mainTarget;
    
    public float cooldownIncrease;
    public float percentageIncrease;

    private void Awake()
    {
        attack = GetComponent<CharacterAttack>();
        animator = GetComponent<Animator>();
        skillParent = transform.Find("Skills");

        foreach (Transform skillTransform in skillParent)
        {
            Skill newSkill= skillTransform.GetComponent<Skill>();
            if (newSkill != null)
            {
                skills.Add(newSkill);
            }
        }
    }

    private void Update()
    {
        foreach (Skill skill in skills)
        {
            skill.coolDownIncrease = cooldownIncrease;
            skill.percentageIncrease = percentageIncrease;

            if (skill.defaultStat.skillType == SkillType.Always)
            {
                foreach (BuffEffect buff in skill.defaultStat.buffEffects)
                {
                    switch (buff.attackType)
                    {
                        case AttackType.Single:
                            switch (buff.targetType)
                            {
                                case TargetType.Enemy:
                                    break;
                                case TargetType.AllAlly:
                                    foreach (CharacterModel model in CharacterManager.Instance.wholeCharacters)
                                    {
                                        if (skill.allyTargets.Contains(model) == false)
                                        {
                                            skill.allyTargets.Add(model);
                                            buff.ApplyBuff(this, model);
                                        }
                                    }
                                    break;
                                case TargetType.StrongestAlly:
                                    int allyTargetNumber = Mathf.Clamp(buff.targetNumber + attack.targetNumberIncrease, 0, CharacterManager.Instance.wholeCharacters.Count);
                                    List<CharacterModel> strongestModels = CharacterManager.Instance.wholeCharacters.OrderByDescending((model) => (model.attack.applyDamage)).ToList();
                                    if (skill.allyTargets.Count >= allyTargetNumber)
                                    {
                                        while (skill.allyTargets.Count < allyTargetNumber)
                                        {
                                            skill.allyTargets[skill.allyTargets.Count - 1].photonView.RPC("RemoveBuff", RpcTarget.All, buff.buffName);
                                            skill.allyTargets.RemoveAt(skill.allyTargets.Count - 1);
                                        }
                                    }
                                    for (int i = 0; i < allyTargetNumber; i++)
                                    {
                                        if ((skill.allyTargets[i] == strongestModels[i]) == false)
                                        {
                                            skill.allyTargets[i].photonView.RPC("RemoveBuff",RpcTarget.All, buff.buffName);
                                            skill.allyTargets[i] = strongestModels[i];
                                            buff.ApplyBuff(this, skill.allyTargets[i]);
                                        }
                                    }
                                    break;
                            }
                            break;
                        case AttackType.Area:
                            float applyBuffArea = buff.attackArea * attack.attackAreaIncrease;
                            switch (buff.targetType)
                            {
                                case TargetType.Enemy:
                                    foreach (EnemyModel enemy in EnemyManager.Instance.enemies)
                                    {
                                        if (Vector3.Distance(transform.position, enemy.transform.position) < applyBuffArea)
                                        {
                                            if (skill.enemyTargets.Contains(enemy) == false)
                                            {
                                                skill.enemyTargets.Add(enemy);
                                                buff.ApplyBuff(this, enemy);
                                            }
                                        }
                                        else
                                        {
                                            if (skill.enemyTargets.Contains(enemy))
                                            {
                                                skill.enemyTargets.Remove(enemy);
                                                enemy.photonView.RPC("RemoveBuff", RpcTarget.All, buff.buffName);
                                            }
                                        }
                                    }
                                    break;
                                case TargetType.AllAlly:
                                    foreach (CharacterModel ally in CharacterManager.Instance.wholeCharacters)
                                    {
                                        if (Vector3.Distance(transform.position, ally.transform.position) < applyBuffArea)
                                        {
                                            if (skill.allyTargets.Contains(ally) == false)
                                            {
                                                skill.allyTargets.Add(ally);
                                                buff.ApplyBuff(this, ally);
                                            }
                                        }
                                    }
                                    break;
                                case TargetType.StrongestAlly:
                                    break;
                            }
                            break;

                    }
                }
            }
        }
    }

    public void SetSkillStats()
    {
        cooldownIncrease = 1.0f;
        percentageIncrease = 0.0f;
    }

    public void AddCrystal(Vector3Int crystals)
    {
        cooldownIncrease += 0.1f * crystals.z;
        percentageIncrease += 0.1f * crystals.z;
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
        isCasting = true;

        mainTarget = currentSkill.SetTarget(this);
        currentSkill.StartSkill();

        float applySkillSpeed = Mathf.Clamp(skill.defaultStat.skillSpeed, 0.1f, attack.applyAttackDelay);

        animator.SetFloat("SkillSpeed", skill.defaultStat.skillClip.length / applySkillSpeed);
        photonView.RPC("SkillTriggerRPC", RpcTarget.All, skill.defaultStat.triggerName);
    }

    [PunRPC]
    public void SkillTriggerRPC(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void OnSkill(AnimationEvent animationEvent)
    {
        if (currentSkill == null)
        {
            return;
        }

        //if (animationEvent.animatorClipInfo.weight < 0.9f)
        //{
        //    return;
        //}

        isCasting = false;

        if (photonView.IsMine == false)
        {
            return;
        }
        print("Skill Execute");

        currentSkill.OnSkill(this);
    }

    public void CancelSkill()
    {
        isCasting = false;
        currentSkill.CancelSkill();
    }
}