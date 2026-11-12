using UnityEngine;

public class DiceAnimationScript : MonoBehaviour
{

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShakeDice()
    {
        animator.SetBool("IsRolling", true);
    }

    public void StopShakeDice()
    {
        animator.SetBool("IsRolling", false);
    }
}
