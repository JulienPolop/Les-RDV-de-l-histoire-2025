using UnityEngine;

public class VaubanController : MonoBehaviour
{
    private Animator Animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.Animator = GetComponent<Animator>();
    }

    public void SetAnimation(AnimState state)
    {
        Animator.SetInteger("State", (int)state);
    }

    public enum AnimState : int
    {
        IDLE = 0,
        HAPPY = 1,
        RUN = 2,
    }
}
