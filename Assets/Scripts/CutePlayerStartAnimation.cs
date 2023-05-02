using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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

    Vector3 originalPosition;

    Coroutine animationCoroutine;

    [SerializeField] AudioSource audioSource;
    private void Start()
    {
        originalPosition = transform.position;
    }
    public void DoAnimation()
    {
        if (animationCoroutine==null) animationCoroutine = StartCoroutine("Animation");
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
        audioSource.Play();
        while(timer < animationLength + 30)
        {
            timer++;
            yield return null;
        }
    }
    public void CancelAnimation()
    {
        animator.Play("Idle");
        StopCoroutine(animationCoroutine);
        animationCoroutine = null;
        transform.position = originalPosition;
        rootsObject.transform.localScale = new Vector3(1, 0, 1);
    }
}
