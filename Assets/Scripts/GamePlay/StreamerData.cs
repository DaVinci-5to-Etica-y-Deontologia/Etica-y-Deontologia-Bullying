using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class StreamerData : DataElement<StreamerData>
{
    public int streamID;

    public DataPic<UserData> users = new();

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

    public (UserData value, int ID, int index) this[int ID]
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
            var userData = new UserData(users.Prepare());

            DataRpc.Create(Actions.AddUser, textIP, userData);
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
    public void AddUser(string jsonUser)
    {
        var aux = JsonUtility.FromJson<UserData>(jsonUser);

        users.Add(aux.CreatePic<UserData>()).Value.Create(this);

        aux.onCreateComment += (comment) => onCreateComment?.Invoke(comment);

        aux.onLeaveComment += (comment) => onLeaveComment?.Invoke(comment);        
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

    public void Create()
    {
        Init();

        Life.Set(streamer.Life);

        Viewers.total = streamer.maxViews;

        if (!IsServer)
            return;

        Users(streamer.minimalViews * 2);

        DataRpc.Create(Actions.ActEnableStream, textIP);
    }

    public StreamerData(int ID, int streamID)
    {
        this.ID = ID;
        this.streamID = streamID;
    }
}



public abstract class DataElement<T> : IDataElement,IDirection where T: DataElement<T>
{
    public int ID;

    public abstract string textIP { get; }

    protected abstract IDataElement parent { get; }

    [field: SerializeField]
    public virtual bool Enable { get; set; } = false;

    public BD dataBase => parent.dataBase;

    public EventManager eventManager => parent.eventManager;

    public Player player => parent.player;
    
    public bool IsServer => parent.IsServer;

    public Internal.Pictionary<int, T> CreatePic()
    {
        return new Internal.Pictionary<int, T>(ID, (T)this);
    }

    public Internal.Pictionary<int, H> CreatePic<H>() where H : T
    {
        return new Internal.Pictionary<int, H>(ID, (H)this);
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