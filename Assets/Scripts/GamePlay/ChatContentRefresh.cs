using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public class ChatContentRefresh : MonoBehaviour
{
    public int lenght;

    public CommentView[] commentViews;

    public bool flagScroll;

    public bool flagClamp;

    public bool clampCondition;

    public UnityEngine.UI.Button topButton;

    public UnityEngine.UI.Button bottomButton;

    [SerializeField]
    UnityEngine.UI.ContentSizeFitter contain;

    [SerializeField]
    UnityEngine.UI.Scrollbar containScroll;

    [SerializeField]
    UnityEngine.UI.ScrollRect containScrollRect;

    //[SerializeField]
    float valueY;

    bool clampedEnd;

    bool clampedStart;

    int _middle = 10;

    List<CommentData> commentDatas = new();

    Streamer Actual => StreamerManager.instance.Actual;

    IEnumerable<CommentData> comments => Actual.commentViews.Select((pic)=>pic.Value);

    int actual
    {
        get
        {
            return (int)Mathf.Round((min + (max - min) * (1- valueY)));
        }
    }

    int min => Mathf.Clamp(middle - 10, 0, lenght - 1);

    int max => Mathf.Clamp(middle + 10, 0, lenght - 1);

    public int middle
    {
        get => Mathf.Clamp(_middle, 10, lenght - 11);
        set
        {
            _middle = value;
        }
    }


    
    public void OnValueChange(Vector2 value)
    {
        valueY = value.y;

        if (value.y < 0.02f)
        {
            middle = actual;

            flagScroll = true;

            if (actual != max)
            {
                containScroll.value = 0.75f;
                containScrollRect.verticalNormalizedPosition = 0.75f;
            }
        }
        else if (value.y > 0.98f)
        {
            middle = actual;

            flagScroll = true;

            if (actual != min)
            {
                containScroll.value = 0.25f;
                containScrollRect.verticalNormalizedPosition = 0.25f;
            }
        }        
    }

    void Scroll()
    {
        lenght = this.comments.Count();

        commentDatas = this.comments.Skip(min).Take(max - min).ToList();

        for (int i = 0; i < commentViews.Length; i++)
        {
            bool _break=false;

            for (int j = 0; j < commentDatas.Count; j++)
            {
                if(commentDatas[j] == commentViews[i].commentData)
                {
                    commentDatas.RemoveAt(j);
                    _break = true;
                    break;
                }
            }

            if (!_break)
            {
                commentViews[i].Destroy();
            }
        }

        for (int i = 0; i < commentDatas.Count; i++)
        {
            StreamerManager.CreateComment(commentDatas[i]);
        }
    }

    private void LateUpdate()
    {

        contain.enabled = !contain.enabled;
        if(flagScroll)
        {
            Scroll();
            flagScroll = false;
        }

        if(flagClamp)
            Clamp(clampCondition);
    }

    void Clamp(bool condition)
    {
        if (condition)
            middle = 0;
        else
            middle = this.comments.Count();

        Scroll();
        containScroll.value = Convert.ToInt32(condition);
        containScrollRect.verticalNormalizedPosition = Convert.ToInt32(condition);
    }

    public void SetClamp(bool condition)
    {
        clampCondition = condition;
        flagClamp = !flagClamp;

        if (condition)
            bottomButton.interactable = !bottomButton.interactable;
        else
            topButton.interactable = !topButton.interactable;
    }
}
