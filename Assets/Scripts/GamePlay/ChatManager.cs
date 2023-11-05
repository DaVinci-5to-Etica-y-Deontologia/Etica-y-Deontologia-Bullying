using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class ChatManager : MonoBehaviour
{
    public BD dataBase;

    public List<User> users = new();

    [SerializeField]
    EventManager eventManager;

    [SerializeField]
    CommentView prefab;   

    Timer deley;

    [SerializeField]
    int index = 0;

    [SerializeField,Range(10,60)]
    float startDeley;

    [SerializeField]
    UnityEngine.UI.ContentSizeFitter contain;

    [SerializeField]
    float multiply=1;

    EventParam<(int, int)> onFinishDay;    

    LinkedList<CommentView> commentViews = new();


    private void Awake()
    {
        deley = TimersManager.Create(startDeley, FinishDay).Stop();

        onFinishDay = eventManager.events.SearchOrCreate<EventParam<(int, int)>>("finishday");
    }

    public void MyStart()
    {
        print("Comienza el juego");
        deley.Start();

        CreateUsers(5);
    }

    public void Users(int number)
    {
        if (number == 0)
            return;
        else if (number > 0)
            CreateUsers(number);
        else
            LeaveUsers(-number);
    }

    void CreateUsers(int number)
    {
        for (int i = number - 1; i >= 0; i--)
        {
            users.Add(new User(this));
        }
    }

    void LeaveUsers(int number)
    {
        for (int i = number - 1; i >= 0; i--)
        {
            users[Random.Range(0, users.Count)].Destroy(); 
        }
    }

    public void LeaveComment(CommentView commentView)
    {
        commentViews.Remove(commentView.node);
        Destroy(commentView.gameObject);
    }

    public CommentView CreateComment(User user, Comment comment)
    {
        while(commentViews.Count > 50)
        {
            commentViews.First.Value.Aplicate();
            LeaveComment(commentViews.First.Value);
        }            

        var newCommentView = Instantiate(prefab, contain.transform);

        newCommentView.Create(user, comment, contain);

        newCommentView.node = commentViews.AddLast(newCommentView);

        return newCommentView;
    }

    /// <summary>
    /// Calcula la sumatoria final de todo el danio y la cantidad de viewers
    /// </summary>
    public void FinishDay()
    {
      
        //onFinishDay.delegato.Invoke(sumSeed);
    }

  


}
