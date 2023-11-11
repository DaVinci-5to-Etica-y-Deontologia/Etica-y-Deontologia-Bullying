using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public class ChatContentRefresh : MonoBehaviour
{
    public UnityEngine.UI.Button topButton;

    public UnityEngine.UI.Button bottomButton;

    public TMPro.TextMeshProUGUI commentText;

    [SerializeField]
    EventManager eventManager;

    [SerializeField]
    UnityEngine.UI.ContentSizeFitter contain;

    [SerializeField]
    UnityEngine.UI.Scrollbar containScroll;

    [SerializeField]
    UnityEngine.UI.ScrollRect containScrollRect;

    //[SerializeField]
    float valueY;

    int _middle = 10;

    int lenght;

    bool flagScroll;

    bool flagClamp;

    bool clampTopOrBotton;

    CommentView[] commentViews;

    List<CommentData> commentDatas;

    StreamerData Actual => StreamerManager.instance.Actual;

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

        SetText();

        if (flagClamp)
        {
            if (value.y < 0.025f)
            {
                containScroll.value = 0;
                containScrollRect.verticalNormalizedPosition = 0;
            }
            return;
        }

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

    public void SetClamp(bool _clampBotton)
    {
        if (_clampBotton && !flagClamp)
        {
            flagClamp = true;
            containScroll.value = 0;
            containScrollRect.verticalNormalizedPosition = 0;
            middle = this.comments.Count();
        }
        else
        {
            flagClamp = false;
        }

        if(!_clampBotton)
        {
            containScroll.value = 1;
            containScrollRect.verticalNormalizedPosition = 1;
            middle = 0;
        }
    }

    void SetText()
    {
        commentText.text = $"Comentarios: {actual}/{lenght}";
    }

    void Scroll()
    {
        lenght = this.comments.Count();

        SetText();

        commentDatas = this.comments.Skip(min).Take(max - min).ToList();

        var commentViews = this.commentViews.Where((commentView) => commentView.commentData != null)
            .OrderBy((commentView) => commentView.commentData.timeOnCreate).ToArray();

        for (int i = 0; i < commentViews.Length; i++)
        {
            bool _break=false;

            for (int j = 0; j < commentDatas.Count; j++)
            {
                if(commentDatas[j] == commentViews[i].commentData)
                {
                    commentDatas.RemoveAt(j);
                    commentViews[i].transform.SetAsLastSibling();
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

    void OnLeaveComment(CommentData commentData)
    {
        if(min==0)
            flagScroll = true;
    }

    void OnCreateComment(CommentData commentData)
    {
        if (lenght<30)
        {
            flagScroll = true;
        }

        if(flagClamp)
        {
            middle = this.comments.Count();
            flagScroll = true;
        }
    }



    private void LateUpdate()
    {
        //contain.enabled = !contain.enabled;

        if (flagScroll)
        {
            Scroll();
            flagScroll = false;
        }
    }
    
    public void Load()
    {
        eventManager.events.SearchOrCreate<EventParam<CommentData>>("createcomment").delegato += OnCreateComment;
        eventManager.events.SearchOrCreate<EventParam<CommentData>>("leavecomment").delegato += OnLeaveComment;

        eventManager.events.SearchOrCreate<EventParam>("poolloaded").delegato += () => commentViews = GetComponentsInChildren<CommentView>(true);

        eventManager.events.SearchOrCreate<EventParam<StreamerData>>("streamchange").delegato += (streamer) => { flagScroll = true; middle = 0;};
    }
}
