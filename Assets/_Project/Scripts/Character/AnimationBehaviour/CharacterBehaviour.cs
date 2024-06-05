using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterBehaviour : StateMachineBehaviour
{
    [SerializeField] protected CharacterModel owner;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (owner == null)
        {
            owner = animator.GetComponent<CharacterModel>();
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner == null)
        {
            owner = animator.GetComponent<CharacterModel>();
        }
    }
}