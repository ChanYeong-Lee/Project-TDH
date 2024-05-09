using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : CharacterBehaviour
{
    private bool continueAttack;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.attack.mainTarget == null || owner.attack.mainTarget.gameObject.activeSelf == false)
        {
            if (owner.attack.isAttacking)
            {
                owner.attack.photonView.RPC("SetTriggerRPC", RpcTarget.All, "Cancel");
                owner.attack.CancelAttack();
                Debug.Log("공격을 취소합니다");
            }
        }

        if (owner.attack.isAttacking == false && animator.GetBool("TryMove") == false)
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
                continueAttack = true;
            }
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.attack.mainTarget != null && owner.attack.mainTarget.gameObject.activeSelf && owner.attack.isAttacking)
        {
            Vector3 direction = owner.attack.mainTarget.transform.position - owner.transform.position;
            direction.y = 0.0f;
            direction.Normalize();

            owner.move.Rotate(direction);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.attack.isAttacking && continueAttack == false)
        {
            owner.attack.CancelAttack();
            Debug.Log("공격을 취소합니다");
        }
    }
}
