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

    public event System.Action<StreamerData> onEndStream;

    [field: SerializeField]
    public Tim Life { get; private set; } = new();

    [field: SerializeField]
    public Tim Viewers { get; private set; } = new();

    public bool Enable { get; private set; } = false;


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

    public bool IsServer => streamerManager.IsServer;

    public string textIP => ID.ToString();

    public bool ShowEnd => (State != StreamState.Empate || streamerManager.gameEnd) && Enable;

    public StreamState State => Viewers.current == Viewers.total ? StreamState.Completado : (Life.current == 0 || Viewers.current <= streamer.minimalViews ? StreamState.Fallido : StreamState.Empate);

    StreamerManager.Data streamerManager;

    public (User value, int ID, int index) this[int ID]
    {
        get
        {
            return users.GetTByID(ID);
        }
    }

    public void Stop()
    {
        foreach (var item in users)
        {
            item.Value.Stop();
        }
    }    

    public void Users(int number)
    {
        if (number == 0 && !IsServer)
            return;

        else if (number > 0)
            CreateUsers(number);
        else
            LeaveUsers(-number);        
    }    

    void CreateUsers(int number)
    {
        Debug.Log("Enable antes del for: " + Enable);

        for (int i = 1; i <= number; i++)
        {
            Internal.Pictionary<int, User> idUser = new(users.lastID+ i, new User(users.lastID + i));

            Actions action = Actions.AddUser;

            string ip = textIP;

            DataRpc.Create(action, ip, idUser);
        }

        Debug.Log("Enable despues del for: " + Enable);
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

        StreamerManager.eventQueue.Enqueue(() => users.Add(aux).Value.Create(this));

        aux.Value.onCreateComment += (comment) => onCreateComment?.Invoke(comment);

        aux.Value.onLeaveComment += (comment) => onLeaveComment?.Invoke(comment);

        if(Viewers.current <= streamer.minimalViews && !Enable)
            StreamerManager.eventQueue.Enqueue(() => Enable = true);
        
    }

    //rpc
    public void RemoveUser(int index)
    {
        users.GetTByIndex(index).value.Destroy();
    }

    void InternalShowEnd(IGetPercentage percentage , float dif)
    {
        if (ShowEnd && Enable)
        {
            Stop();
            onEndStream?.Invoke(this);
            Debug.Log("SE EJECUTÓ: InternalShowEnd");
        }
    }

    public void Init()
    {
        this.streamerManager = StreamerManager.instance.streamersData;

        Life.onChange += InternalShowEnd;

        Viewers.onChange += InternalShowEnd;
    }

    public void Create(int ID)
    {
        this.ID = ID;

        Init();

        Life.Set(streamer.Life);

        Viewers.total = streamer.maxViews;

        Users(streamer.minimalViews * 2);
    }

    public StreamerData(int streamID)
    {
        this.streamID = streamID;
    }
}
