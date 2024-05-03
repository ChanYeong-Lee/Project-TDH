using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : CharacterBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.attack.mainTarget == null || owner.attack.mainTarget.gameObject.activeSelf == false)
        {
            if (owner.attack.isAttacking)
            {
                owner.attack.photonView.RPC("SetTriggerRPC", RpcTarget.All, "Cancel");
                owner.attack.CancelAttack();
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
        if (owner.attack.isAttacking)
        {
            owner.attack.CancelAttack();
        }
    }
}
