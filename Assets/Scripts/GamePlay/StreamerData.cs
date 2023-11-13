using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class StreamerData : IDirection
{
    public int ID;

    public int streamID;

    public DataPic<User> users = new();

    public event System.Action<CommentData> onCreateComment;

    public event System.Action<CommentData> onLeaveComment;

    [field: SerializeField]
    public Tim Life { get; private set; } = new();

    [field: SerializeField]
    public Tim Viewers { get; private set; } = new();

    public IEnumerable<Internal.Pictionary<int, CommentData>> commentViews
    {
        get
        {
            return users.SelectMany((user) => user.Value.comments).OrderBy((comment)=>comment.Value.timeOnCreate);
        }
    }

    public BD dataBase => streamerManager.dataBase;

    public EventManager eventManager => streamerManager.eventManager;

    public Streamer streamer => dataBase.Streamers[streamID];

    public Player player => streamerManager.player;

    public string textIP => ID.ToString();

    

    StreamerManager streamerManager;

    public (User value, int ID, int index) this[int ID]
    {
        get
        {
            return users.GetTByID(ID);
        }
    }

    public void Create(int ID)
    {
        this.ID = ID;

        this.streamerManager = StreamerManager.instance;

        Life.Set(100);

        Viewers.total = streamer.maxViews;

        Users(streamer.minimalViews * 2);
    }

    public void Users(int number)
    {
        if (number == 0)
            return;

        else if (number > 0)
            CreateUsers(number);
        else
            LeaveUsers(-number);

        Viewers.current += number;
    }    

    void CreateUsers(int number)
    {
        for (int i = number - 1; i >= 0; i--)
        {
            Internal.Pictionary<int, User> idUser = new(users.lastID+1, new User(users.lastID + 1));

            DataRpc.Create(Actions.AddUser, textIP, idUser);
        }
    }

    void LeaveUsers(int number)
    {
        for (int i = Mathf.Clamp(number - 1, 0 , users.Count - 1); i >= 0; i--)
        {
            var rng = Random.Range(0, users.Count);

            DataRpc.Create(Actions.RemoveUser, users.GetTByIndex(rng).value.textIP);
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
    public void RemoveUser(int index)
    {
        users.GetTByIndex(index).value.Destroy();
    }

    public StreamerData(int streamID)
    {
        this.streamID = streamID;
    }
}
