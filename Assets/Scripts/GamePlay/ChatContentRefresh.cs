using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class ChatContentRefresh : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.ContentSizeFitter contain;

    [SerializeField]
    UnityEngine.UI.Scrollbar containScroll;

    [SerializeField]
    UnityEngine.UI.ScrollRect containScrollRect;

    //[SerializeField]
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

    //[SerializeField]
    int _middle;

    Transform[] comments => transform.GetComponentsInChildren<CommentView>(true)
        .Where((commentView) => commentView.commentData != null)
        .Select((commentView) => commentView.transform)
        .ToArray();

    /*
    [SerializeField]
    int commentsLenght;

    [SerializeField]
    float valueVertical;
    */


    private void Update()
    {
        //commentsLenght = comments.Length;
    }

    private void OnTransformChildrenChanged()
    {
        if(childCount < 15)
        {
            ScrollAndCenter();
            childCount = comments.Length;
        }

        //abajo de todo y no tengo nada mas
        prevValue = containScrollRect.normalizedPosition.y;
        
        if (prevValue < 0.05f && comments.Length - middle < 10)
            ClampBar();
        
    }

    public void OnValueChange(Vector2 value)
    {
        if (value.y < 0.3f && comments.Length - middle < 10)
        {
            ClampBar();
            return;
        }
        
        if (value.y < 0.05f)
        {
            middle += 5;
            ScrollAndCenter();
        }
        else if(value.y > 0.95f)
        {
            middle -= 5;
            ScrollAndCenter();
        }
        
        //valueVertical = value.y;
    }

    void ClampBar()
    {
        middle = comments.Length;
        Scroll();
        containScroll.value = 0;
        containScrollRect.verticalNormalizedPosition = 0f;
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
    }

    void ScrollAndCenter()
    {
        Scroll();
        containScroll.value = 0.5f;
    }

    private void LateUpdate()
    {
        contain.enabled = !contain.enabled;
    }
}
