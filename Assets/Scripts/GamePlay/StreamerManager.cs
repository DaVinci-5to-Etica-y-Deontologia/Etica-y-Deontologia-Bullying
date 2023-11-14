using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using Fusion;

public class StreamerManager : NetworkBehaviour
{
    public struct SearchResult
    {
        public (StreamerData value, int ID, int index)  streamer;
        public (User value, int ID, int index)          user;
        public (CommentData value, int ID, int index)   comment;

        public StreamerData Streamer => streamer.value;

        public User User => user.value;

        public CommentData CommentData => comment.value;
    }

    public static StreamerManager instance;

    public BD dataBase;

    public EventManager eventManager;

    public Player player;

    public static Queue<System.Action> eventQueue = new();

    public EventParam<StreamerData> onStreamerCreate;

    public EventParam<StreamerData> onStreamerChange;

    public EventParam<CommentData> onCreateComment;

    public EventParam<CommentData> onLeaveComment;

    public EventParam<StreamerData> onStreamEnd;

    public EventParam onFinishDay;

    public bool gameEnd;

    [SerializeField]
    public DataPic<StreamerData> streamers = new();

    public Timer endGame;

    [SerializeField]
    CommentView prefab;

    [SerializeField, Range(1, 60)]
    float startDeley;

    [SerializeField]
    ChatContentRefresh contain;

    [SerializeField]
    float multiply = 1;

    LinkedPool<CommentView> pool;    

    Stopwatch watchdog;

    public int IndexStreamWatch
    {
        get => _indexStreamWatch;

        set
        {
            _indexStreamWatch = value;

            if (_indexStreamWatch < 0)
                _indexStreamWatch = streamers.Count - 1;
            
            else if(_indexStreamWatch >= streamers.Count)
                _indexStreamWatch = 0;
            
        }
    }

    public (StreamerData value, int ID, int index) this[int ID]
    {
        get
        {
            return streamers.GetTByID(ID);
        }
    }

    public int Count { get; private set; }

    public bool IsServer => Runner != null && Runner.IsServer;

    public StreamerData Actual { get; private set; }

    [SerializeField]
    int _indexStreamWatch = -1;

    Timer delay;

    

    static public CommentView SpawnComment()
    {
        return Instantiate(instance.prefab, instance.contain.transform);
    }

    static public void CreateComment(CommentData obj)
    {
        var aux = instance.pool.Obtain().Self;

        aux.transform.SetAsLastSibling();

        aux.commentData = obj;

        aux.SetActiveGameObject(true);
    }


    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_Execute(string json)
    {
        DataRpc dataRpc = JsonUtility.FromJson<DataRpc>(json);

        //UnityEngine.Debug.Log($"recibido:\n{dataRpc}");

        var srch = Search(dataRpc.direction);

        switch (dataRpc.action)
        {
            case Actions.Ban:
                {
                    srch.User.Ban();
                }
                break;

            case Actions.Admonition:
                {
                    srch.User.Admonition(srch.comment.index);
                }
                break;

            case Actions.Picantear:
                {
                    srch.User.Picantear();
                }
                break;

            case Actions.Corromper:
                {
                    srch.User.ChangeMoral();
                }
                break;

            case Actions.Suspect:
                {
                    srch.User.SuspectChange(dataRpc.data);
                }
                break;




            case Actions.AddUser:
                {
                    srch.Streamer.AddUser(dataRpc.data);
                }
                break;

            case Actions.RemoveUser:
                {
                    srch.Streamer.RemoveUser(srch.user.index);
                }
                break;



            case Actions.AddComment:
                {
                    srch.User.AddComment(dataRpc.data);
                }
                break;

            case Actions.RemoveComment:
                {
                    srch.User.RemoveComment(srch.comment.index);
                }
                break;

            case Actions.AddStream:
                {
                    instance.AddStream(dataRpc.data);
                }
                break;
        }
    }

    public static SearchResult Search(string dir)
    {
        var splirDir = dir.Split('.');

        SearchResult searchResult = new SearchResult();

        if(splirDir.Length > 0 && dir!=string.Empty)
        {
            searchResult.streamer = instance[int.Parse(splirDir[0])];

            if(searchResult.Streamer != null && splirDir.Length > 1)
            {
                searchResult.user = searchResult.Streamer[int.Parse(splirDir[1])];

                if(searchResult.User != null && splirDir.Length > 2)
                {
                    searchResult.comment = searchResult.User[int.Parse(splirDir[2])];
                }
            }
        }

        return searchResult;
    }

    public void AddStream(string json)
    {
        var streamer = JsonUtility.FromJson<StreamerData>(json);

        streamers.Add(streamer);

        streamer.Create(streamers.lastID);

        onStreamerCreate.delegato?.Invoke(streamer);

        Count++;

        streamer.onEndStream += (s) => Count--;

        if (IsServer)
        {
            streamer.onCreateComment += CommentDataDelete_onCreateComment;
        }
    }


    /// <summary>
    /// Ejecuta el llamado para terminar la partida
    /// </summary>
    public void FinishDay()
    {
        onFinishDay.delegato.Invoke();

        gameEnd = true;

        endGame.Stop();

        foreach (var item in streamers)
        {
            item.Value.Stop();
        }
    }  

    public void CreateStream()
    {
        DataRpc.Create(Actions.AddStream, "", new StreamerData(dataBase.SelectStreamer()));
    }

    public void ChangeStreamByID(int ID)
    {
        ChangeStream(streamers.GetTByID(ID).index);
    }

    public void NextStreamer()
    {
        foreach (var item in streamers)
        {
            if(!item.Value.ShowEnd)
            {
                ChangeStreamByID(item.Key);
                return;
            }
        }
    }

    public List<StreamerData> FinishedStreams()
    {
        var list = new List<StreamerData>();
        
        foreach (var item in streamers)
        {
            if (item.Value.ShowEnd)
            {
                list.Add(item.Value);
            }
        }

        return list;
    }
   
    public void ChangeStream(int index)
    {
        var previus = IndexStreamWatch;

        IndexStreamWatch = index;

        StreamerData aux;

        if (previus>=0)
        {
            aux = streamers.GetTByIndex(previus).value;

            aux.onCreateComment -= CommentCreateQueue;

            aux.onLeaveComment -= CommentLeaveQueue;

            aux.onEndStream -= Aux_onEndStream;
        }

        aux = streamers.GetTByIndex(IndexStreamWatch).value;

        aux.onCreateComment += CommentCreateQueue;

        aux.onLeaveComment += CommentLeaveQueue;

        aux.onEndStream += Aux_onEndStream;

        Actual = aux;

        onStreamerChange.delegato?.Invoke(Actual);

        if (aux.ShowEnd)
            Aux_onEndStream(aux);
    }

    void Aux_onEndStream(StreamerData obj)
    {
        onStreamEnd.delegato.Invoke(obj);

        if (Count <= 0)
            FinishDay();
    }

    void CommentCreateQueue(CommentData commentData)
    {
        onCreateComment.delegato.Invoke(commentData);
    }

    void CommentLeaveQueue(CommentData commentData)
    {
        onLeaveComment.delegato.Invoke(commentData);
    }

    void CommentDataDelete_onCreateComment(CommentData commentData)
    {
        var timerDestroy = TimersManager.Create(30, () =>
        {
            commentData.user.Aplicate(commentData.comment.Views, commentData.comment.Damage, commentData.textIP);
        });

        commentData.onDestroy += () => timerDestroy.Stop();
    }

    void MyStart()
    {
        print("Comienza el juego");

        endGame.Reset();

        CreateStream();

        ChangeStream(0);
    }

    private void Update()
    {
        if (!IsServer)
        {
            eventQueue.Clear();
            return;
        }

        while(eventQueue.Count > 0 && watchdog.Elapsed.TotalMilliseconds < 16)
        {
            eventQueue.Dequeue().Invoke();
        }

        watchdog.Restart();
    }

    public void Load()
    {
        onFinishDay = eventManager.events.SearchOrCreate<EventParam>("finishday");

        onCreateComment = eventManager.events.SearchOrCreate<EventParam<CommentData>>("createcomment");

        onLeaveComment = eventManager.events.SearchOrCreate<EventParam<CommentData>>("leavecomment");

        onStreamerChange = eventManager.events.SearchOrCreate<EventParam<StreamerData>>("streamchange");

        onStreamerCreate = eventManager.events.SearchOrCreate<EventParam<StreamerData>>("streamcreate");

        onStreamEnd = eventManager.events.SearchOrCreate<EventParam<StreamerData>>("streamend");

        GameManager.instance.StartCoroutine(pool.CreatePool(30, ()=> eventManager.events.SearchOrCreate<EventParam>("poolloaded").delegato.Invoke()));

        GameManager.instance.StartCoroutine(User.LoadPerfilSprite());
    }

    public void EndLoad()
    {
        MyStart();
    }

    public override void Spawned()
    {
        watchdog.Start();

        UnityEngine.Debug.Log("server: " + IsServer);

        endGame = TimersManager.Create(5 * 60, FinishDay).Stop();
    }

    private void Awake()
    {
        instance = this;

        pool = new(prefab);

        watchdog = new Stopwatch();
    }
}

public class Actions
{
    public const string Ban = "Ban";

    public const string Admonition = "Admonition";

    public const string Picantear = "Picantear";

    public const string Corromper = "Corromper";

    public const string Suspect = "Sus";

    public const string AddUser = "AddUser";

    public const string RemoveUser = "RemoveUser";

    public const string AddComment = "AddComment";

    public const string RemoveComment = "RemoveComment";

    public const string AddStream = "AddStream";

    public const string RemoveStream = "RemoveStream";
}

public struct DataRpc
{
    public string action;

    public string direction;

    public string data;

    public static void Create(string action, string direction)
    {
        //UnityEngine.Debug.Log(action + ": " + direction);
        StreamerManager.instance.Rpc_Execute(JsonUtility.ToJson(new DataRpc() { action = action, direction = direction }));
    }

    public static void Create(string action, string direction, string data)
    {
        //UnityEngine.Debug.Log(action + ": " + direction + " " + data);
        StreamerManager.instance.Rpc_Execute(JsonUtility.ToJson(new DataRpc() { action = action, direction = direction, data = data }));
    }

    public static void Create(string action, string direction, object data)
    {
        //UnityEngine.Debug.Log(action + ": " + direction + " " + JsonUtility.ToJson(data, true));
        StreamerManager.instance.Rpc_Execute(JsonUtility.ToJson(new DataRpc() { action = action, direction = direction, data = JsonUtility.ToJson(data) }));
    }

    public override string ToString()
    {
        return $"accion: {action}\ndireccion: {direction}\ndata: {data}";
    }
}