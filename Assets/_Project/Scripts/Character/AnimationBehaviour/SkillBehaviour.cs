using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBehaviour : CharacterBehaviour
{
    private bool continueSkill;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.skill.mainTarget == null || owner.skill.mainTarget.gameObject.activeSelf == false)
        {
            if (owner.skill.isCasting)
            {
                owner.attack.photonView.RPC("SetTriggerRPC", RpcTarget.All, "Cancel");
                owner.skill.CancelSkill();
                owner.attack.CancelAttack();
            }
        }

        if (owner.skill.isCasting == false && animator.GetBool("TryMove") == false)
        {
            if (owner.skill.CheckNonTargetCooldownSkill(out Skill readyNonTargetSkill))
            {
                owner.attack.StartSkill();
                owner.skill.StartSkill(readyNonTargetSkill);
            }
            if (owner.attack.canAttack)
            {
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
                continueSkill = true;
            }
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.skill.mainTarget != null && owner.skill.mainTarget.gameObject.activeSelf && owner.skill.isCasting)
        {
            Vector3 direction = owner.skill.mainTarget.transform.position - owner.transform.position;
            direction.y = 0.0f;
            direction.Normalize();

            owner.move.Rotate(direction);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.skill.isCasting && continueSkill == false)
        {
            owner.skill.CancelSkill();
            owner.attack.CancelAttack();
        }
    }
}
