using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Move : StateMachineBehaviour
{
    Character character;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<Character>();
        }
    }

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(character.Direction == Vector3.zero)
        {
            animator.SetBool("IsMove", false);
        }
        else
        {

            character.gameObject.transform.position += character.Speed * character.Direction * Time.deltaTime;
            Vector3 newdir = Vector3.RotateTowards(character.transform.forward, character.Direction, Time.deltaTime * 30f, Time.deltaTime * 30f);
            character.transform.rotation = Quaternion.LookRotation(newdir);
        }
    }

    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsMove", false);
    }

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
