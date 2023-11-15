using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

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

    [System.Serializable]
    public class Data
    {
        [SerializeField]
        public DataPic<StreamerData> streamers = new();

        public Timer endGame;

        public bool gameEnd;

        public BD dataBase => streamerManager.dataBase;

        public EventManager eventManager => streamerManager.eventManager;

        public Player player => streamerManager.player;

        public bool IsServer => streamerManager.IsServer;

        StreamerManager streamerManager;

        public Data(StreamerManager streamerManager)
        {
            this.streamerManager = streamerManager;
        }
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

    public Data streamersData;

    [SerializeField]
    CommentView prefab;

    [SerializeField, Range(1, 60)]
    float startDeley;

    [SerializeField]
    ChatContentRefresh contain;

    [SerializeField]
    float multiply = 1;

    [SerializeField]
    int _indexStreamWatch = -1;

    LinkedPool<CommentView> pool;    

    Stopwatch watchdog;

    bool started;

    Timer delay;

    Dictionary<int,string> buffer = new();

    public int IndexStreamWatch
    {
        get => _indexStreamWatch;

        set
        {
            _indexStreamWatch = value;

            if (_indexStreamWatch < 0)
                _indexStreamWatch = streamersData.streamers.Count - 1;
            
            else if(_indexStreamWatch >= streamersData.streamers.Count)
                _indexStreamWatch = 0;
            
        }
    }

    public (StreamerData value, int ID, int index) this[int ID]
    {
        get
        {
            return streamersData.streamers.GetTByID(ID);
        }
    }

    public int Count { get; private set; }

    public bool IsServer => Runner.IsServer || Runner.IsSinglePlayer;

    public StreamerData Actual { get; private set; }
    

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
        if (!started)
            return;

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



            case Actions.CreateStream:
                {
                    if (IsServer)
                        DataRpc.Create(Actions.AddStream, "", new StreamerData(dataBase.SelectStreamer()));
                }
                break;

            case Actions.AddStream:
                {
                    instance.AddStream(dataRpc.data);
                }
                break;



            case Actions.StartUpdateStreamers:
                {
                    if (IsServer)
                    {
                        //ejecuto la pausa para todos con un rpc
                        Rpc_GlobalPause();
                        instance.StartCoroutine(instance.PrependUpdate(JsonUtility.ToJson(streamersData)));
                    }
                }
                break;

            case Actions.EndUpdateStreamers:
                {
                    UnityEngine.Debug.Log("SE EJECUTÓ EndUpdateStreamers");
                    //Reanudo la partida para todos
                    GlobalUnPause();
                }
                break;

        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_GlobalPause()
    {
        GameManager.instance.Pause(true);
        TransitionManager.instance.SetTransition(TransitionManager.WaitStart);

        UnityEngine.Debug.Log("El juego se pauso");
    }
    
    public void GlobalUnPause()
    {
        GameManager.instance.Pause(false);
        TransitionManager.instance.SetTransition(TransitionManager.WaitEnd);
        
        UnityEngine.Debug.Log("El juego se des pauso");
    }

    public static SearchResult Search(string dir)
    {
        SearchResult searchResult = new SearchResult();

        if (dir == string.Empty)
            return searchResult;

        var splirDir = dir.Split('.');

        if(splirDir.Length > 0)
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

        streamersData.streamers.Add(streamer);

        streamer.Create(streamersData.streamers.lastID);

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

        streamersData.gameEnd = true;

        streamersData.endGame.Stop();

        foreach (var item in streamersData.streamers)
        {
            item.Value.Stop();
        }
    }  


    public void CreateFirstStream()
    {
        started = true;
        CreateStream();
        ChangeStream(0);
    }

    public void CreateStream()
    {
        DataRpc.Create(Actions.CreateStream);
    }

    public IEnumerator PrependUpdate(string json)
    {
        int order = 0;
        UnityEngine.Debug.Log("EMPEZO A CARGAR LOS DATOS");
        do
        {
            Rpc_RequestUpdate(json.SubstringClamped(0, 500), order , false);
            json = json.SubstringClamped(500);
            yield return null;
            order++;
        }
        while (json.Length > 500);

        Rpc_RequestUpdate(json, order, true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_RequestUpdate(string json , int order, bool end)
    {
        if (started)
            return;

        if (!end)
        {
            buffer.Add(order, json);
        }
        else 
        {
            buffer.Add(order, json);

            var newJson = string.Empty;

            for (int i = 0; i < buffer.Count; i++)
            {
                if (buffer.TryGetValue(i, out var s))
                    newJson += s;
                else
                    UnityEngine.Debug.LogError("No se recupero el dato: " + i);
            }

            buffer.Clear();

            JsonUtility.FromJsonOverwrite(newJson, streamersData);

            started = true;

            foreach (var stream in streamersData.streamers)
            {
                stream.Value.Init();

                foreach (var user in stream.Value.users)
                {
                    user.Value.Init(stream.Value);

                    foreach (var comment in user.Value.comments)
                    {
                        comment.Value.Init(stream.Key, user.Key);
                    }
                }
            }

            UnityEngine.Debug.Log("ACABARON DE CARGAR LOS DATOS");

            eventQueue.Enqueue(
                ()=> 
                {
                    DataRpc.Create(Actions.EndUpdateStreamers);
                    ChangeStream(0);
                    CreateStream();
                });
        }
    }

    public void ChangeStreamByID(int ID)
    {
        ChangeStream(streamersData.streamers.GetTByID(ID).index);
    }

    public void NextStreamer()
    {
        foreach (var item in streamersData.streamers)
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
        
        foreach (var item in streamersData.streamers)
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
            aux = streamersData.streamers.GetTByIndex(previus).value;

            aux.onCreateComment -= CommentCreateQueue;

            aux.onLeaveComment -= CommentLeaveQueue;

            aux.onEndStream -= Aux_onEndStream;
        }

        aux = streamersData.streamers.GetTByIndex(IndexStreamWatch).value;

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

        streamersData.endGame.Reset();

        if (IsServer)
            CreateFirstStream();
        else
            DataRpc.Create(Actions.StartUpdateStreamers);
            
    }

    private void Update()
    {
        if (!started)
        {
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

        streamersData.endGame = TimersManager.Create(5 * 60, FinishDay).Stop();
    }

    private void Awake()
    {
        instance = this;

        streamersData = new(this);

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

    public const string CreateStream = "CreateStream";

    public const string AddStream = "AddStream";

    public const string RemoveStream = "RemoveStream";

    public const string StartUpdateStreamers = "StartUpdateStreamers";

    public const string EndUpdateStreamers = "EndUpdateStreamers";
}

public struct DataRpc
{
    public string action;

    public string direction;

    public string data;

    public static void Create(string action)
    {
        UnityEngine.Debug.Log(action);
        StreamerManager.instance.Rpc_Execute(JsonUtility.ToJson(new DataRpc() { action = action}));
    }

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