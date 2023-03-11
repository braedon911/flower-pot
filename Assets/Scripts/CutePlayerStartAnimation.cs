using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class CutePlayerStartAnimation : MonoBehaviour
{
    public Animator animator;
    [Range(1, 300)]
    public int animationLength;
    public int transitionFrame = 6;
    public GameObject rootsObject;

    public UnityEvent finishedAnimation = new UnityEvent();
    public void DoAnimation()
    {
        StartCoroutine("Animation");
    }
    IEnumerator Animation()
    {
        int timer = -1;
        
        animator.Play("Grow Start");

        while (timer < transitionFrame)
        {
            timer++;
            yield return null;
        }
        rootsObject.SetActive(true);
        animator.Play("Grow");

        while (timer < animationLength)
        {
            timer++;
            transform.position = transform.position + 2*Vector3.up;
            rootsObject.transform.localScale = new Vector3(1, -0.25f * (timer-transitionFrame), 1);
            yield return null;
        }
        animator.Play("Grow Stop");
        while(timer < animationLength + 30)
        {
            timer++;
            yield return null;
        }
        finishedAnimation.Invoke();
    }
}
