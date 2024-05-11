using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionBehaviour : CharacterBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("TryMove") == false
            && owner.attack.canAttack
            && owner.attack.CheckTarget())
        {
            // 현재 NonTargetCooldownSkill이 없으므로 주석 처리
            //if (owner.skill.CheckNonTargetCooldownSkill(out Skill readyNonTargetSkill))
            //{
            //    owner.attack.StartSkill();
            //    owner.skill.StartSkill(readyNonTargetSkill);
            //}

            if (owner.skill.CheckTargetCooldownSkill(out Skill readyTargetSkill))
            {
                owner.attack.StartSkill();
                owner.skill.StartSkill(readyTargetSkill);
            }
            else if (owner.skill.CheckAttackSkill(out Skill activatedAttackSkill))
            {
                owner.attack.StartSkill();
                owner.skill.StartSkill(activatedAttackSkill);
            }
            else
            {
                owner.attack.StartAttack();
            }
        }
    }
}
