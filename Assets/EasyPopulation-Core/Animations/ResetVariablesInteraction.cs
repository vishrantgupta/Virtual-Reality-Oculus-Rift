using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVariablesInteraction : StateMachineBehaviour
{

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("ataca", false);
        animator.SetBool("mate", false);
        animator.SetBool("interact", false);       
        animator.SetBool("noInitiativeWin", false);
        animator.SetBool("noInitiativeLose", false);
        animator.SetBool("initiativeWin", false);
        animator.SetBool("initiativeLose", false);
    }
    
}
