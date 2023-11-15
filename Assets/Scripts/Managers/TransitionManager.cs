using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TransitionManager : SingletonMono<TransitionManager>
{
    Animator animator;

    public const string Squares = "Squares";
    public const string SquaresStart = "SquaresStart";
    public const string SquaresEnd = "SquaresEnd";
    public const string Lines = "Lines";
    public const string LinesStart = "LinesStart";
    public const string LinesEnd = "LinesEnd";
    public const string WaitStart = "WaitStart";
    public const string WaitEnd = "WaitEnd";

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public void SetTransition(string triggerName)
    {
        StartCoroutine(PLayAnimation(triggerName, 0, null));
    }

    public void SetTransition(string triggerName, float animationTime, Action endAction)
    {
        StartCoroutine(PLayAnimation(triggerName, animationTime, endAction));
    }

    IEnumerator PLayAnimation(string triggerName, float animationTime, Action endAction)
    {
        Debug.Log("llamo a trigger: " + triggerName);
        animator.SetTrigger(triggerName);

        yield return new WaitForSeconds(animationTime);

        Debug.Log("espere la cantidad de " + animationTime);

        endAction?.Invoke();
    }

}
