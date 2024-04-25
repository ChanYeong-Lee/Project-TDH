using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionBehaviour : StateMachineBehaviour
{
    private CharacterModel model;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        model = animator.GetComponent<CharacterModel>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //float currentMoveSpeed = animator.GetFloat("MoveSpeed");
        //bool isRotating = animator.GetBool("Rotate");

        //if (currentMoveSpeed > 0.1f || isRotating)
        //{
        //    model.state = CharacterState.Move;
        //}
        //else
        //{
        //    model.state = CharacterState.Idle;
        //}
        model.state = model.move.tryMove ? CharacterState.Move : CharacterState.Idle;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
