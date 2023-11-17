using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class StreamerData : DataElement<StreamerData>
{
    public int streamID;

    public DataPic<User> users = new();

    public event System.Action<CommentData> onCreateComment;

    public event System.Action<CommentData> onLeaveComment;

    public event System.Action<StreamerData> onEndStream;

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


    public Streamer streamer => dataBase.Streamers[streamID];

    public override string textIP => ID.ToString();

    public bool ShowEnd => (State != StreamState.Empate || streamerManager.gameEnd) && Enable;

    public StreamState State => Viewers.current == Viewers.total ? StreamState.Completado : (Life.current == 0 || Viewers.current <= streamer.minimalViews ? StreamState.Fallido : StreamState.Empate);

    protected override IDataElement parent => streamerManager;

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
        if (number > 0)
            CreateUsers(number);
        else
            LeaveUsers(-number);        
    }    

    void CreateUsers(int number)
    {
        for (int i = 1; i <= number; i++)
        {
            var idUser = users.Prepare(new User(users.lastID + 1));

            Actions action = Actions.AddUser;

            string ip = textIP;

            DataRpc.Create(action, ip, idUser);
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

        users.Add(aux).Value.Create(this);

        aux.Value.onCreateComment += (comment) => onCreateComment?.Invoke(comment);

        aux.Value.onLeaveComment += (comment) => onLeaveComment?.Invoke(comment);        
    }

    //rpc
    public void SetEnable()
    {
        Enable = true;
    }

    //rpc
    public void RemoveUser(int index)
    {
        users.GetTByIndex(index).value.Destroy();
    }

    void InternalShowEnd(IGetPercentage percentage , float dif)
    {
        if (ShowEnd)
        {
            Stop();
            onEndStream?.Invoke(this);
            //Debug.Log("SE EJECUTÓ: InternalShowEnd");
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

        if (!IsServer)
            return;

        Users(streamer.minimalViews * 2);

        DataRpc.Create(Actions.ActEnableStream, textIP);
    }

    public StreamerData(int streamID)
    {
        this.streamID = streamID;
    }
}



public abstract class DataElement<T> : IDataElement,IDirection where T: DataElement<T>
{
    public int ID;

    public abstract string textIP { get; }

    protected abstract IDataElement parent { get; }

    [field: SerializeField]
    public virtual bool Enable { get; set; } = true;

    public BD dataBase => parent.dataBase;

    public EventManager eventManager => parent.eventManager;

    public Player player => parent.player;
    
    public bool IsServer => parent.IsServer;

    public Internal.Pictionary<int, T> Prepare()
    {
        return new Internal.Pictionary<int, T>(ID, (T)this);
    }

}

public interface IDataElement
{
    public BD dataBase { get; }

    public EventManager eventManager { get; }

    public Player player { get; }

    public bool IsServer { get; }
}

public interface IDirection
{
    public string textIP { get; }
}