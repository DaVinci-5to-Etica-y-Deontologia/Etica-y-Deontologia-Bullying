using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Streamer : MonoBehaviour
{
    public int ID;

    public BD dataBase;

    public DataPic<User> users = new();

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

    DataPic<CommentView> commentViews = new();

    private void Awake()
    {
        deley = TimersManager.Create(startDeley, FinishDay).Stop();

        onFinishDay = eventManager.events.SearchOrCreate<EventParam<(int, int)>>("finishday");

        ID = StreamerManager.instance.streamers.Add(this).Key;
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
            Internal.Pictionary<int, User> idUser = new(users.lastID+1, new User(users.lastID + 1));

            AddUser(JsonUtility.ToJson(idUser));
        }
    }

    void LeaveUsers(int number)
    {
        for (int i = Mathf.Clamp(number - 1, 0 , users.Count - 1); i >= 0; i--)
        {
            var rng = Random.Range(0, users.Count);

            RemoveUser(users.GetIDByIndex(rng));
        }
    }

    public CommentView CreateComment(User user, Comment comment)
    {
        var newCommentView = Instantiate(prefab, contain.transform);

        newCommentView.Create(commentViews.Add(newCommentView).Key, comment);

        newCommentView.Init(ID, user.ID);

        if (commentViews.Count > 50)
        {
            commentViews.GetTByIndex(0).Aplicate();
            LeaveComment(commentViews.GetIDByIndex(0));
        }

        return newCommentView;
    }


    /// <summary>
    /// Calcula la sumatoria final de todo el danio y la cantidad de viewers
    /// </summary>
    public void FinishDay()
    {
      
        //onFinishDay.delegato.Invoke(sumSeed);
    }

    //rpc
    public void AddUser(string jsonPic)
    {
        users.Add(JsonUtility.FromJson<Internal.Pictionary<int, User>>(jsonPic)).Value.Init(this);
    }

    //rpc
    public void RemoveUser(int idUser)
    {
        users.GetTByID(idUser).Destroy();
    }

    //rpc
    public void LeaveComment(int id)
    {
        var aux = commentViews.GetIndexByID(id);

        Destroy(commentViews.GetTByIndex(aux).gameObject);

        commentViews.RemoveAt(aux);
    }


}
