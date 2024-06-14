using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : CharacterBehaviour
{
    private bool continueAttack;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.attack.isAttacking)
        {
            if (owner.attack.mainTarget == null 
                || owner.attack.mainTarget.gameObject.activeSelf == false
                || owner.attack.mainTarget.poolCount != owner.attack.mainTargetPoolCount)
            {
                //switch (owner.attack.attackType)
                //{
                //    case AttackType.Single:
                //        if (owner.attack.CheckTarget() == false)
                //        {
                //            owner.attack.photonView.RPC("SetTriggerRPC", RpcTarget.All, "Cancel");
                //            owner.attack.CancelAttack();
                //            Debug.Log("공격을 취소합니다");
                //        }
                //        break;
                //    case AttackType.Area:
                //        owner.attack.photonView.RPC("SetTriggerRPC", RpcTarget.All, "Cancel");
                //        owner.attack.CancelAttack();
                //        Debug.Log("공격을 취소합니다");
                //        break;
                //}
            }
        }
        else
        {
            if (animator.GetBool("TryMove") == false
                && owner.attack.canAttack
                && owner.attack.CheckTarget())
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
        if (owner.attack.isAttacking
            && owner.attack.mainTarget != null 
            && owner.attack.mainTarget.gameObject.activeSelf)
        {
            Vector3 direction = owner.attack.mainTarget.transform.position - owner.transform.position;
            direction.y = 0.0f;
            direction.Normalize();

            owner.move.Rotate(direction);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.attack.isAttacking 
            && continueAttack == false)
        {
            owner.attack.CancelAttack();
            Debug.Log("공격을 취소합니다");
        }
    }
}
