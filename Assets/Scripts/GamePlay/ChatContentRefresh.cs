using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public class ChatContentRefresh : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent<string> onCommentTextChange;

    public Player player;

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

    int oldMiddle = 10;

    int lenght;

    bool flagScroll;

    bool flagClamp;

    CommentView[] commentViews;

    List<CommentData> commentDatas;

    StreamerData ActualStreamer => StreamerManager.instance.Actual;
    IEnumerable<CommentData> comments => ActualStreamer.commentViews.Select((pic)=>pic.Value);

    int MaxCommentsToView => player.NumerOfCommentsToView;

    int MiddleCommentsToView => MaxCommentsToView / 2;

    float PercentageComment => 1f / MaxCommentsToView;

    int Actual
    {
        get
        {
            return (int)Mathf.Round((Min + (Max - Min) * (1- valueY)));
        }
    }

    int Min => Mathf.Clamp(Middle - MiddleCommentsToView, 0, lenght);

    int Max => Mathf.Clamp(Middle + MiddleCommentsToView, 0, lenght);

    int Middle
    {
        get => Mathf.Clamp(_middle, MiddleCommentsToView, lenght - MiddleCommentsToView);
        set
        {
            oldMiddle = _middle;
            _middle = value;
        }
    }
    
    public void OnValueChange(Vector2 value)
    {
        valueY = value.y;

        SetText();

        if (flagClamp && valueY < 0.35f)
        {
            Bottom();
            return;
        }

        if (valueY < PercentageComment || valueY > 1 - PercentageComment)
        {
            Middle = Actual;

            flagScroll = true;
        }
    }


    public void SetClamp(bool _clampBotton)
    {
        if (_clampBotton && !flagClamp)
        {
            flagClamp = true;
            Bottom();
        }
        else
        {
            flagClamp = false;
        }

        if(!_clampBotton)
        {
            containScroll.value = 1;
            containScrollRect.verticalNormalizedPosition = 1;
            Middle = 0;
        }

        SetText();
    }

    void Bottom()
    {
        containScroll.value = 0;
        containScrollRect.verticalNormalizedPosition = 0;
        Middle = this.comments.Count();
        flagScroll = true;
    }

    void SetText()
    {
        //Mira esa concatenacion
        onCommentTextChange.Invoke($"Viendo comentarios {(flagClamp? "Mas recientes".RichText("color", "green") : ((Min == 0) && lenght>30 ? "Mas Antiguos".RichText("color", "red") : "con el indice: ".RichText("color", "yellow") + Actual))}");
    }

    void BarScroll()
    {
        if (flagClamp)
        {
            if (valueY < PercentageComment)
            {
                containScroll.value = 0;
                containScrollRect.verticalNormalizedPosition = 0;
            }
            return;
        }

        if (valueY < PercentageComment)
        {
            if(lenght != Max)
            {
                float aux = UnityEngine.Mathf.Clamp(((float)_middle - oldMiddle) / MaxCommentsToView , 0 , 0.75f);

                containScroll.value = aux;
                containScrollRect.verticalNormalizedPosition = aux;
            }
            
        }

        else if (valueY > 1 - PercentageComment)
        {
            if(0 != Min)
            {
                float aux = 1 + UnityEngine.Mathf.Clamp(((float)_middle - oldMiddle) / MaxCommentsToView, -0.75f, 0);

                containScroll.value = aux;
                containScrollRect.verticalNormalizedPosition = aux;
            }
        }
    }

    void Scroll()
    {
        lenght = this.comments.Count();

        SetText();

        commentDatas = this.comments.Skip(Min).Take(Max - Min).ToList();

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

        BarScroll();
    }

    void OnLeaveComment(CommentData commentData)
    {
        if (!flagClamp)
        {
            Middle--;
        }

        if(Min==0)
            flagScroll = true;
    }

    void OnCreateComment(CommentData commentData)
    {
        if (lenght <= MaxCommentsToView)
        {
            flagScroll = true;
        }

        if(flagClamp)
        {
            //Middle = this.comments.Count();
            //flagScroll = true;
            //Scroll();

            Bottom();
        }
    }

    private void LateUpdate()
    {
        contain.enabled = !contain.enabled;

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
        eventManager.events.SearchOrCreate<EventParam<StreamerData>>("streamchange").delegato += (streamer) => { flagScroll = true; Middle = 0;};
    }
}
