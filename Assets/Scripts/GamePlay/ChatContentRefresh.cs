using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChatContentRefresh : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.ContentSizeFitter contain;

    [SerializeField]
    UnityEngine.UI.Scrollbar containScroll;

    [SerializeField]
    UnityEngine.UI.ScrollRect containScrollRect;

    float prevValue;

    int childCount;

    int min => Mathf.Clamp(middle - 10, 0, comments.Length - 1);

    int max => Mathf.Clamp(middle + 10, 0, comments.Length - 1);

    int middle
    {
        get => _middle;
        set
        {
            _middle = Mathf.Clamp(value, 0, comments.Length - 1);
        }
    }

    int _middle;

    Transform[] comments => transform.GetComponentsInChildren<CommentView>(true)
        .Where((commentView) => commentView.commentData != null)
        .Select((commentView) => commentView.transform)
        .ToArray();

    private void OnTransformChildrenChanged()
    {
        if(childCount < 15)
        {
            Scroll();
            childCount = comments.Length;
        }

        //abajo de todo y no tengo nada mas
        prevValue = containScrollRect.normalizedPosition.y;

        if (prevValue < 0.05f && comments.Length-middle < 10)
        {
            middle = comments.Length;
            Scroll();
            containScroll.value = 0;
            containScrollRect.normalizedPosition = Vector2.zero;
        }
    }

    public void OnValueChange(Vector2 value)
    {
        if(value.y<0.05f)
        {
            middle += 5;
            Scroll();
        }
        else if(value.y > 0.95f)
        {
            middle -= 5;
            Scroll();
        }      

        //prevValue = value.y;
    }

    void Scroll()
    {
        for (int i = 0; i < comments.Length; i++)
        {
            if(i >= min && i<= max)
                comments[i].SetActiveGameObject(true);
            else
                comments[i].SetActiveGameObject(false);
        }

        containScroll.value = 0.5f;
    }

    private void LateUpdate()
    {
        contain.enabled = !contain.enabled;
    }
}
