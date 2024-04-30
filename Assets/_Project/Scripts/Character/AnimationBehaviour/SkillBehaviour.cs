using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBehaviour : CharacterBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.skill.mainTarget == null || owner.skill.mainTarget.gameObject.activeSelf == false)
        {
            if (owner.skill.isCasting)
            {
                animator.SetTrigger("Cancel");
                owner.skill.CancelSkill();
            }
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.skill.mainTarget != null && owner.skill.mainTarget.gameObject.activeSelf)
        {
            Vector3 direction = owner.skill.mainTarget.transform.position - owner.transform.position;
            direction.y = 0.0f;
            direction.Normalize();

            owner.move.Rotate(direction);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.skill.isCasting)
        {
            owner.skill.CancelSkill();
        }
    }
}
