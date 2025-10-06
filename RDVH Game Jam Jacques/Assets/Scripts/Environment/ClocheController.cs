using System;
using UnityEngine;
using static VaubanController;

public class ClocheController : MonoBehaviour
{
    private Animator Animator;
    [SerializeField] private AudioManager AudioManager;
    [SerializeField] private ParticleSystem VFXRingBell;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.Animator = GetComponent<Animator>();
    }

    public void RingBell()
    {
        Animator.SetTrigger("Ring");
        AudioManager.Play("clochette",2);
        VFXRingBell.Play();
    }

    public void SetAnimation(AnimState state)
    {
        Animator.SetInteger("State", (int)state);
    }

    public enum AnimState : int
    {
        IDLE = 0,
        MOUSEOVER = 1,
    }
}
