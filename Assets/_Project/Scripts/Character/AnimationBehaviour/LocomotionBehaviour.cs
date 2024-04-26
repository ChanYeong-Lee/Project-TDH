using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionBehaviour : CharacterBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("TryMove") == false)
        {
            if (owner.attack.canAttack)
            {
                if (owner.skill.CheckAttackSkill(out Skill activatedSkill))
                {
                    activatedSkill.StartSkill();
                }
                else
                {
                    owner.attack.StartAttack();
                }
            }
        }
    }
}
