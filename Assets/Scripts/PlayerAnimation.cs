using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    public void OnWalk()
    {
        animator.SetTrigger("Walk");
    }

    public void OnIdle()
    {
        animator.SetTrigger("Idle");
    }

    public void OnAttack()
    {
        animator.SetTrigger("Attack");
    }
}
