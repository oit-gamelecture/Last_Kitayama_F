using UnityEngine;

public class stopanimation : MonoBehaviour
{
    public Animator animator;

    public void StopAnimation()
    {
        animator.speed = 0f; // アニメーションを停止
    }
}


