using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSkill : MonoBehaviourPun
{
    private Animator animator;
    public CharacterAttack attack;

    public List<Skill> skills;
    public Skill currentSkill;

    public Transform mainTarget;

    public bool isCasting;

    private void Awake()
    {
        attack = GetComponent<CharacterAttack>();
        animator = GetComponent<Animator>();
    }

    public bool CheckNonTargetCooldownSkill(out Skill readySkill)
    {
        List<Skill> readySkills = new List<Skill>();
        foreach (Skill skill in skills)
        {
            if (skill.skillType == SkillType.NonTargetCooldown)
            {
                if (skill.isReady)
                {
                    readySkills.Add(skill);
                }
            }
        }

        if (readySkills.Count > 0)
        {
            readySkills = readySkills.OrderBy(skill => skill.priority).ToList();
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
            if (skill.skillType == SkillType.TargetCooldown)
            {
                if (skill.isReady)
                {
                    readySkills.Add(skill);
                }
            }
        }

        if (readySkills.Count > 0)
        {
            readySkills = readySkills.OrderBy(skill => skill.priority).ToList();
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
            if (skill.skillType == SkillType.Random)
            {
                float percentage = skill.percentage * skill.percentageIncrease;
                float randomValue = Random.Range(0.0f, 1.0f);

                if (randomValue < percentage)
                {
                    activatedSkills.Add(skill);
                }
            }
        }

        if (activatedSkills.Count > 0)
        {
            activatedSkills = activatedSkills.OrderBy(skill => skill.priority).ToList();
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

        animator.SetFloat("SkillSpeed", skill.defaultStat.skillClip.length / skill.skillSpeed);
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

        if (animationEvent.animatorClipInfo.weight < 0.9f)
        {
            return;
        }

        isCasting = false;
        
        if (photonView.IsMine == false)
        {
            return;
        }

        currentSkill.OnSkill();
    }

    public void CancelSkill()
    {
        isCasting = false;
        currentSkill.CancelSkill();
    }
}