using UnityEngine;

public class IdleOffset : StateMachineBehaviour
{
    // Called when the animator enters this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Only works if state is looping
        animator.Update(UnityEngine.Random.value * stateInfo.length);
    }

    // Called every frame while in this state (if you need it)
    // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    // Called when exiting this state
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
}
