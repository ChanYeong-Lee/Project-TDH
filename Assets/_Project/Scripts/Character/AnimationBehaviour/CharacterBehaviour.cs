using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterBehaviour : StateMachineBehaviour
{
    protected CharacterModel owner;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        owner = animator.GetComponent<CharacterModel>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<CharacterModel>();
    }
}