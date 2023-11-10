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

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public void SetTransition(string triggerName, float animationTime, Action endAction = null)
    {
        StartCoroutine(PLayAnimation(triggerName, animationTime, endAction));
    }

    IEnumerator PLayAnimation(string triggerName, float animationTime, Action endAction)
    {
        animator.SetTrigger(triggerName);

        yield return new WaitForSeconds(animationTime);

        endAction?.Invoke();
    }

}
