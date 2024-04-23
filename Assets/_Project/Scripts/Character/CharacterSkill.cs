using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterSkill : MonoBehaviour
{
    public List<Skill> skills;

    public bool CheckAttackSkill(out Skill activatedSkill)
    {
        List<Skill> activatedSkills = new List<Skill>();

        foreach (Skill skill in skills)
        {
            if (skill.type == SkillType.Random)
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
}
