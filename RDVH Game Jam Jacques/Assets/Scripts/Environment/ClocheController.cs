using UnityEngine;
using static VaubanController;

public class ClocheController : MonoBehaviour
{
    private Animator Animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.Animator = GetComponent<Animator>();
    }

    public void RingBell()
    {
        Animator.SetTrigger("Ring");
    }
}
