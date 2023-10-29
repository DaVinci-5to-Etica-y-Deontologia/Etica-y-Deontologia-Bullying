using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChatManager : MonoBehaviour
{
    public List<CommentView> comments = new List<CommentView>();

    [SerializeField]
    UnityEvent finishDay;

    [SerializeField]
    CommentView prefab;

    [SerializeField]
    BD dataBase;

    Timer deley;

    [SerializeField]
    int index = 0;

    [SerializeField,Range(1,10)]
    float startDeley;

    [SerializeField]
    UnityEngine.UI.ContentSizeFitter contain;


    private void Awake()
    {
        deley = TimersManager.Create(startDeley, Delay).Stop();
    }

    public void MyStart()
    {
        print("Comienza el juego");
        deley.Start();
    }

    void Delay()
    {
        if (index >= dataBase.comments.Length || dataBase.comments[index].Parent.name == "Empty")
        {
            finishDay.Invoke();
            return;
        }

        var newComment=Instantiate(prefab, contain.transform);

        newComment.Create(dataBase.comments[index], contain);        

        comments.Add(newComment);

        index++;

        deley.Set(dataBase.comments[index].Deley);
    }
}
