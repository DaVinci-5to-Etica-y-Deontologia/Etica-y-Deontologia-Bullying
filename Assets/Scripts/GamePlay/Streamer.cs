using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class Streamer : IDirection
{
    public int ID;

    public DataPic<User> users = new();

    public event System.Action<CommentData> onCreateComment;

    public event System.Action<CommentData> onLeaveComment;
    public IEnumerable<Internal.Pictionary<int, CommentData>> commentViews
    {
        get
        {
            return users.SelectMany((user) => user.Value.comments).OrderBy((comment)=>comment.Value.timeOnCreate);
        }
    }

    public BD dataBase => streamerManager.dataBase;

    public EventManager eventManager => streamerManager.eventManager;

    public string textIP => ID.ToString();


    StreamerManager streamerManager;

    public Streamer()
    {
        this.streamerManager = StreamerManager.instance;
    }

    public User this[int ID]
    {
        get
        {
            return users.GetTByID(ID);
        }
    }

    public void Create(int ID , int users)
    {
        this.ID = ID;

        TimersManager.Create(5, ()=> CreateUsers(users));
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

    //rpc
    public void AddUser(string jsonPic)
    {
        var aux = JsonUtility.FromJson<Internal.Pictionary<int, User>>(jsonPic);

        users.Add(aux).Value.Init(this);

        aux.Value.onCreateComment += (comment) => onCreateComment?.Invoke(comment);

        aux.Value.onLeaveComment += (comment) => onLeaveComment?.Invoke(comment);
    }

    //rpc
    public void RemoveUser(int idUser)
    {
        users.GetTByID(idUser).Destroy();
    }
}
