using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class ChatManager : MonoBehaviour
{
    public Dictionary<User,HashSet<CommentView>> comments = new();

    [SerializeField]
    EventManager eventManager;

    [SerializeField]
    CommentView prefab;

    [SerializeField]
    BD dataBase;

    Timer deley;

    [SerializeField]
    int index = 0;

    [SerializeField,Range(1,10)]
    float startDeley;

    [SerializeField, Range(1, 10)]
    float endDeley=5;

    [SerializeField]
    UnityEngine.UI.ContentSizeFitter contain;

    [SerializeField]
    float multiply=1;

    EventParam<(int, int)> onFinishDay;


    private void Awake()
    {
        deley = TimersManager.Create(startDeley, Delay).SetMultiply(multiply).Stop();

        onFinishDay = eventManager.events.SearchOrCreate<EventParam<(int, int)>>("finishday");

        eventManager.events.SearchOrCreate<EventParam<CommentView>>("ban").delegato += Ban;

        eventManager.events.SearchOrCreate<EventParam<CommentView>>("kick").delegato += Kick;

        eventManager.events.SearchOrCreate<EventParam<CommentView>>("eliminate").delegato += Eliminate;
    }

    public void MyStart()
    {
        print("Comienza el juego");
        deley.Start();
    }

    /// <summary>
    /// Calcula la sumatoria final de todo el danio y la cantidad de viewers
    /// </summary>
    public void FinishDay()
    {
        (int damage, int view) sumSeed = (0,0);

        sumSeed = comments
            .Select
            (
                (commentView) => commentView.Value
                .Aggregate
                (
                    sumSeed,
                    (sum, commentView) => 
                    {
                        sum.damage += commentView.comment.Damage;
                        sum.view += commentView.comment.Views;

                        return sum;
                    }
                )
            )
            .Aggregate
            (
                sumSeed, 
                (sum, previusSum) => 
                {
                    sum.damage += previusSum.damage;
                    sum.view += previusSum.view;

                    return sum;
                }
            )
        ;

        onFinishDay.delegato.Invoke(sumSeed);
    }

    public void Ban(CommentView commentView)
    {
        commentView.comment.Parent.Ban = true;

        foreach (var item in comments[commentView.comment.Parent])
        {
            Destroy(item.gameObject);
        }

        comments.Remove(commentView.comment.Parent);

        CheckComment();
    }

    public void Kick(CommentView commentView)
    {
        commentView.comment.Parent.Enable = false;

        Eliminate(commentView);

        CheckComment();
    }

    public void Eliminate(CommentView commentView)
    {
        comments[commentView.comment.Parent].Remove(commentView);

        Destroy(commentView.gameObject);
    }

    void Delay()
    {
        if (dataBase.comments[index].Parent.name == "Empty")
        {
            FinishDay();
            return;
        }

        var newComment = dataBase.comments[index];

        var newCommentView=Instantiate(prefab, contain.transform);

        newCommentView.Create(newComment, contain);

        if(!comments.TryGetValue(newComment.Parent, out var commentsList))
            comments.Add(newComment.Parent, new HashSet<CommentView>() { newCommentView });
        else
            commentsList.Add(newCommentView);

        NextComment();
    }

    void CheckComment()
    {
        if (dataBase.comments[index].Chck)
            return;

        NextComment(dataBase.comments[index].Deley);
    }


    void NextComment(float timeCorrection = 0)
    {
        do
        {
            index++;
        } while (index < dataBase.comments.Length-1 && !dataBase.comments[index].Chck);

        if (index >= dataBase.comments.Length-1)
        {
            TimersManager.Create(endDeley, FinishDay);
            deley.Stop();
            return;
        }

        deley.Set(dataBase.comments[index].Deley - timeCorrection);
    }
}
