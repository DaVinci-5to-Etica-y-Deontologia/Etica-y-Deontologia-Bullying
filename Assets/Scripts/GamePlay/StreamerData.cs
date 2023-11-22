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


    [field: SerializeField]
    public bool Finished { get; private set; } = false;


    public IEnumerable<Internal.Pictionary<int, CommentData>> commentViews
    {
        get
        {
            return users.SelectMany((user) => user.Value.comments).OrderBy((comment)=>comment.Value.timeOnCreate);
        }
    }


    public Streamer streamer => dataBase.Streamers[streamID];

    public override string textIP => ID.ToString();

    public bool ShowEnd => (State != StreamState.Empate) && Finished;

    //public StreamState State => Viewers.current == Viewers.total ? StreamState.Completado : ((Life.current == 0 || Viewers.current <= streamer.minimalViews) ? StreamState.Fallido : StreamState.Empate);
    //public StreamState State => !Finished ? StreamState.Empate : ( (Viewers.current == Viewers.total)  ? StreamState.Completado : StreamState.Fallido);
    public StreamState State => (Viewers.current == Viewers.total) ? StreamState.Completado : (Viewers.current <= streamer.minimalViews || Life.current == 0) ? StreamState.Fallido : StreamState.Empate;

    protected override IDataElement parent => streamerParent;

    StreamerManager.Data streamerParent;

    public (UserData value, int ID, int index) this[int ID]
    {
        get
        {
            return users.GetTByID(ID);
        }
    }

    static public void AddUser(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.Streamer.AddUser(jsonData);
    }
    static public void RemoveUser(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.Streamer.RemoveUser(srch.user.index);
    }
    static public void EnableStream(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.streamer.value.SetEnable();
    }

    static public void LifeUpdate(string intString, StreamerManager.SearchResult srch)
    {
        if(!srch.streamer.value.IsServer) //el server ya se encarga de esta logica, de no aplicar esto me daria un bucle infinito
            srch.streamer.value.Life.current = float.Parse(intString);
    }

    static public void FinishStream(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.streamer.value.FinishStream(srch);
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
            var usersFiltered = users.Where((u) => u.Value.Enable).ToArray();

            if (usersFiltered.Length == 0)
                return;

            var rng = Random.Range(0, usersFiltered.Length);

            usersFiltered[rng].Value.Destroy();

            DataRpc.Create(Actions.RemoveUser, usersFiltered[rng].Value.textIP);
        }
    }



    void InternalShowEnd(IGetPercentage percentage , float dif)
    {
        //Debug.Log("Stream " + ID + " current views: " + Viewers.current);
        if (State != StreamState.Empate && Enable && !Finished)
        {
            Finished = true;
            Debug.Log("Stream " + ID + " InternalShowEnd: LIFE " + (Life.current == 0) + "\n Current views: " + Viewers.current + "  Minimal views: " + streamer.minimalViews);
            Debug.Log(" VIEWS DEFEAT: " + (Viewers.current <= streamer.minimalViews) + "\n VIEWS WIN: " + (Viewers.current == Viewers.total) + " ENABLE: " + Enable + " FINISHED: " + Finished);

            DataRpc.Create(Actions.FinishStream, textIP);
        }
    }

    //rpc
    void AddUser(string jsonUser)
    {
        var aux = JsonUtility.FromJson<UserData>(jsonUser);

        users.Add(aux.CreatePic<UserData>()).Value.Create(this);


        aux.onCreateComment += (comment) => onCreateComment?.Invoke(comment);

        aux.onLeaveComment += (comment) => onLeaveComment?.Invoke(comment);
    }

    //rpc
    void SetEnable()
    {
        Enable = true;
    }

    void FinishStream(StreamerManager.SearchResult srch)
    {
        Finished = true;
        Viewers.current = srch.Streamer.Viewers.current;
        Life.current = srch.Streamer.Life.current;

        onEndStream?.Invoke(this);

        if (streamerParent.Count > 0)
        {
            streamerParent.Count--;
            Debug.Log("Count disminuyó. Nuevo valor Count: " + streamerParent.Count);
        }
        else
            Debug.Log("Count no pudo disminuir debido a que Count es <= que 0. Count value: " + streamerParent.Count);

        Stop();

        if (streamerParent.Count <= 0)
        {
            streamerParent.streamerManager.FinishDay();
        }
    }

    //rpc
    void RemoveUser(int index)
    {
        users.GetTByIndex(index).value.Destroy();
    }


    public void Init()
    {
        this.streamerParent = StreamerManager.instance.streamersData;

        if (IsServer)
        {
            Life.onChange += InternalShowEnd;

            Viewers.onChange += InternalShowEnd;

            Life.onChange += (p, d) => DataRpc.Create(Actions.UpdateLifeStream, textIP, p.current.ToString());
        }
    }

    public void Create()
    {
        Init();

        Life.Set(streamer.Life);

        Viewers.total = streamer.maxViews;

        streamerParent.Count++;
        Debug.Log("Count incrementó. Nuevo valor Count: " + streamerParent.Count);

        if (!IsServer)
            return;

        Users(streamer.minimalViews * 2);

        DataRpc.Create(Actions.EnableStream, textIP);
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
    public virtual bool Enable { get; protected set; } = false;

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