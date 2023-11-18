using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

public class StreamerManager : NetworkBehaviour
{
    public struct SearchResult
    {
        public (StreamerData value, int ID, int index)  streamer;
        public (UserData value, int ID, int index)          user;
        public (CommentData value, int ID, int index)   comment;

        public StreamerData Streamer => streamer.value;

        public UserData User => user.value;

        public CommentData CommentData => comment.value;
    }

    [System.Serializable]
    public class Data : IDataElement
    {
        [SerializeField]
        public DataPic<StreamerData> streamers = new();

        public Timer endGame;

        public bool gameEnd;

        StreamerManager streamerManager;

        public BD dataBase => streamerManager.dataBase;

        public EventManager eventManager => streamerManager.eventManager;

        public Player player => streamerManager.player;

        public bool IsServer => streamerManager.IsServer;
        public Data(StreamerManager streamerManager)
        {
            this.streamerManager = streamerManager;
        }

    }

    public static StreamerManager instance;

    public static Dictionary<string, System.Action<string, SearchResult>> actionsMap = new();

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


    [SerializeField]
    AuxWrapper<DataRpc[]> listRpc;

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
        var aux = instance.pool.Obtain();

        aux.transform.SetAsLastSibling();

        aux.commentData = obj;

        aux.SetActiveGameObject(true);
    }

    public static SearchResult Search(string dir)
    {
        SearchResult searchResult = new SearchResult();

        if (string.IsNullOrEmpty(dir))
            return searchResult;

        var splirDir = dir.Split('.');

        if (splirDir.Length > 0)
        {
            searchResult.streamer = instance[int.Parse(splirDir[0])];

            if (searchResult.Streamer != null && splirDir.Length > 1)
            {
                searchResult.user = searchResult.Streamer[int.Parse(splirDir[1])];

                if (searchResult.User != null && splirDir.Length > 2)
                {
                    searchResult.comment = searchResult.User[int.Parse(splirDir[2])];
                }
            }
        }

        return searchResult;
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_Execute(string json)
    {
        if (!started)
            return;

        listRpc = JsonUtility.FromJson<AuxWrapper<DataRpc[]>>(json);

        for (int i = 0; i < listRpc.data.Length; i++)
        {
            var dataRpc = listRpc.data[i];

            var srch = Search(dataRpc.direction);

            UnityEngine.Debug.Log($"Recibido:\n{dataRpc}");

            actionsMap[dataRpc.action].Invoke(dataRpc.data, srch);
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

    public void AddStream(string streamJson)
    {
        var streamer = JsonUtility.FromJson<StreamerData>(streamJson);

        streamersData.streamers.Add(streamer.CreatePic());

        streamer.Create();

        onStreamerCreate.delegato?.Invoke(streamer);

        Count++;

        streamer.onEndStream += (s) =>
        {
            if (Count > 0)
                Count--;
        };

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
                onStreamerCreate?.delegato.Invoke(stream.Value);

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

            CreateStream();
            ChangeStream(0);

            DataRpc.Create(Actions.EndUpdateStreamers);
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

        if (previus >= 0)
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
        {
            Aux_onEndStream(aux);
        }
    }

    void Aux_onEndStream(StreamerData obj)
    {
        //print("SE EJECUTÓ: Aux_onEndStream");
        onStreamEnd.delegato.Invoke(obj);

        if (Count <= 0)
        {
            FinishDay();
        }
            
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
        }).SetMultiply(player.multiply);

        commentData.onDestroy += () => timerDestroy.Stop();
    }

    void AddUser(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.Streamer.AddUser(jsonData);
    }
    void RemoveUser(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.Streamer.RemoveUser(srch.user.index);
    }
    void CreateStream(string jsonData, StreamerManager.SearchResult srch)
    {
        if (IsServer)
        {
            DataRpc.Create(Actions.AddStream, "", new StreamerData(streamersData.streamers.Prepare(), dataBase.SelectStreamer()));
        }
    }
    void AddNewStream(string jsonData, StreamerManager.SearchResult srch)
    {
        UnityEngine.Debug.Log("Se ejecuto el add stream");

        bool aux = Count == 0;

        instance.AddStream(jsonData);

        if (aux)
        {
            ChangeStream(0);
        }
    }
    void EnableStream(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.streamer.value.SetEnable();
    }
    void StartUpdateStreamers(string jsonData, StreamerManager.SearchResult srch)
    {
        if (IsServer)
        {
            UnityEngine.Debug.Log("SE EJECUTÓ ActStartUpdateStreamers");
            Rpc_GlobalPause();
            instance.StartCoroutine(instance.PrependUpdate(JsonUtility.ToJson(streamersData)));
        }
    }
    void EndUpdateStreamers(string jsonData, StreamerManager.SearchResult srch)
    {
        UnityEngine.Debug.Log("SE EJECUTÓ EndUpdateStreamers");
        GlobalUnPause();
    }

    void MyStart()
    {
        print("Comienza el juego");

        streamersData.endGame.Reset();

        if (IsServer)
        {
            CreateFirstStream();
            //Rpc_GlobalPause();
        } 
        else
        {
            //print("Creo StartUpdateStreamers");
            DataRpc.Create(Actions.StartUpdateStreamers);
        }
    }

    private void Update()
    {
        while (DataRpc.Count > 0)
        {
            instance.Rpc_Execute(DataRpc.definitiveList);
        }

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

        GameManager.instance.StartCoroutine(UserData.LoadPerfilSprite());

        actionsMap.Add(Actions.Ban.className, UserData.Ban);
        actionsMap.Add(Actions.Admonition.className, UserData.Admonition);
        actionsMap.Add(Actions.Picantear.className, UserData.Picantear);
        actionsMap.Add(Actions.Corromper.className, UserData.ChangeMoral);
        actionsMap.Add(Actions.Suspect.className, UserData.SuspectChange);
        actionsMap.Add(Actions.AddComment.className, UserData.AddComment);
        actionsMap.Add(Actions.RemoveComment.className, UserData.RemoveComment);

        actionsMap.Add(Actions.AddUser.className, AddUser);
        actionsMap.Add(Actions.RemoveUser.className, RemoveUser);
        actionsMap.Add(Actions.CreateStream.className, CreateStream);
        actionsMap.Add(Actions.AddStream.className, AddNewStream);
        actionsMap.Add(Actions.EnableStream.className, EnableStream);
        actionsMap.Add(Actions.StartUpdateStreamers.className, StartUpdateStreamers);
        actionsMap.Add(Actions.EndUpdateStreamers.className, EndUpdateStreamers);
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


[System.Serializable]
public class Actions
{
    public static Actions Ban { get; private set; } = new ActBan();

    public static Actions Admonition { get; private set; } = new ActAdmonition();

    public static Actions Picantear { get; private set; } = new ActPicantear();

    public static Actions Corromper { get; private set; } = new ActCorromper();

    public static Actions Suspect { get; private set; } = new ActSus();

    public static Actions AddUser { get; private set; } = new ActAddUser();

    public static Actions RemoveUser { get; private set; } = new ActRemoveUser();

    public static Actions AddComment { get; private set; } = new ActAddComment();

    public static Actions RemoveComment { get; private set; } = new ActRemoveComment();

    public static Actions CreateStream { get; private set; } = new ActCreateStream();

    public static Actions AddStream { get; private set; } = new ActAddStream();

    public static Actions RemoveStream { get; private set; } = new ActRemoveStream();

    public static Actions StartUpdateStreamers { get; private set; } = new ActStartUpdateStreamers();

    public static Actions EndUpdateStreamers { get; private set; } = new ActEndUpdateStreamers();

    public static Actions EnableStream { get; private set; } = new ActEnableStream();

    public override bool Equals(object obj)
    {
        return this.GetType() == obj.GetType();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public string className;
    public Actions()
    {
        className = this.GetType().Name;
    }
}
[System.Serializable] public class ActionStream : Actions { }
[System.Serializable] public class ActionUser : Actions { }
[System.Serializable] public class ActionComment : Actions { }


[System.Serializable] public class ActCreateStream : ActionStream { }

[System.Serializable] public class ActAddStream : ActionStream { }
[System.Serializable] public class ActRemoveStream : ActionStream { }
[System.Serializable] public class ActStartUpdateStreamers : ActionStream { }
[System.Serializable] public class ActEndUpdateStreamers : ActionStream { }
[System.Serializable] public class ActEnableStream : ActionComment { }


[System.Serializable] public class ActAddUser : ActionUser { }
[System.Serializable] public class ActRemoveUser : ActionUser { }
[System.Serializable] public class ActBan : ActionUser { }
[System.Serializable] public class ActAdmonition : ActionUser { }
[System.Serializable] public class ActPicantear : ActionUser { }
[System.Serializable] public class ActCorromper : ActionUser { }
[System.Serializable] public class ActSus : ActionUser { }



[System.Serializable] public class ActAddComment : ActionComment { }
[System.Serializable] public class ActRemoveComment : ActionComment { }



[System.Serializable]
public struct DataRpc
{
    public string action;

    public string direction;

    public string data;

    Actions _action;

    static Queue<DataRpc> streamsRequests = new();
    static Queue<DataRpc> usersRequests = new();
    static Queue<DataRpc> commentsRequests = new();

    static List<DataRpc> _definitiveList = new();

    public static int Count => streamsRequests.Count + usersRequests.Count + commentsRequests.Count;

    const int limitRpc = 30000; // el limite de caracteres es de: 32767

    static int sum;

    public static void Create(Actions action)
    {
        //UnityEngine.Debug.Log(action);
        Filter(new DataRpc() { _action = action, data = string.Empty });
    }
    public static void Create(Actions action, string direction)
    {
        //UnityEngine.Debug.Log(action + ": " + direction)
        Filter(new DataRpc() { _action = action, direction = direction, data = string.Empty });
    }

    public static void Create(Actions action, string direction, string data)
    {
        Filter(new DataRpc() { _action = action, direction = direction, data = data });
    }

    public static void Create(Actions action, string direction, object data)
    {
        //UnityEngine.Debug.Log(action + ": " + direction + " " + JsonUtility.ToJson(data, true));
        Filter(new DataRpc() { _action = action, direction = direction, data = JsonUtility.ToJson(data) });
    }

    public override string ToString()
    {
        return $"accion: {action}\ndireccion: {direction}\ndata: {data}";
    }
   

    static bool finish;

    static void Concat(Queue<DataRpc> dataRpcs)
    {
        while (dataRpcs.Count > 0 && !finish)
        {
            if((sum + dataRpcs.Peek().data.Length) < limitRpc)
            {
                sum += dataRpcs.Peek().data.Length;
            }
            else
            {
                finish = true;
                break;
            }
            

            _definitiveList.Add(dataRpcs.Dequeue());
        }
    }

    public static string definitiveList
    {
        get
        {
            Concat(streamsRequests);
            Concat(usersRequests);
            Concat(commentsRequests);

            var aux = JsonUtility.ToJson(new AuxWrapper<DataRpc[]>(_definitiveList.ToArray()));


            UnityEngine.Debug.Log($"JSON enviado: {aux.Length} de {sum} \n  {aux}  \n\n");

            /*
            UnityEngine.Debug.Log("Streamer Request Count: " + streamsRequests.Count);
            UnityEngine.Debug.Log("Users Request Count: " + usersRequests.Count);
            UnityEngine.Debug.Log("Comments Request Count: " + commentsRequests.Count);
            */

            _definitiveList.Clear();
            sum = 0;
            finish = false;

            return aux;
        }
    }


    public static void Filter(DataRpc dataRpc)
    {
        //UnityEngine.Debug.Log("Recibí la petición de: " + dataRpc.ToString());

        dataRpc.action = dataRpc._action.className;

        if (dataRpc._action is ActionStream)
            streamsRequests.Enqueue(dataRpc);
        else if (dataRpc._action is ActionUser)
            usersRequests.Enqueue(dataRpc);
        else if (dataRpc._action is ActionComment)
            commentsRequests.Enqueue(dataRpc);
    }
}





[System.Serializable]
public struct AuxWrapper<T>
{
    [SerializeField]
    public T data;

    public AuxWrapper(T data)
    {
        this.data = data;
    }
}