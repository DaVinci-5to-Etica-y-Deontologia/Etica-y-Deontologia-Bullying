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

        AuxWrapper<string[]> listRpc = JsonUtility.FromJson<AuxWrapper<string[]>>(json);

        //UnityEngine.Debug.Log($"recibido:\n{dataRpc}");

        for (int i = 0; i < listRpc.data.Length; i++)
        {
            var dataRpc = JsonUtility.FromJson<SerializedDataRpc>(listRpc.data[i]);
            var rpcAction = JsonUtility.FromJson<AuxWrapper<Actions>>(dataRpc.newAction);

            var srch = Search(dataRpc.direction);

            switch (rpcAction.data)
            {
                case ActBan:
                    {
                        srch.User.Ban();
                    }
                    break;

                case ActAdmonition:
                    {
                        srch.User.Admonition(srch.comment.index);
                    }
                    break;

                case ActPicantear:
                    {
                        srch.User.Picantear();
                    }
                    break;

                case ActCorromper:
                    {
                        srch.User.ChangeMoral();
                    }
                    break;

                case ActSus:
                    {
                        srch.User.SuspectChange(dataRpc.data);
                    }
                    break;




                case ActAddUser:
                    {
                        srch.Streamer.AddUser(dataRpc.data);
                    }
                    break;

                case ActRemoveUser:
                    {
                        srch.Streamer.RemoveUser(srch.user.index);
                    }
                    break;



                case ActAddComment:
                    {
                        srch.User.AddComment(dataRpc.data);
                    }
                    break;

                case ActRemoveComment:
                    {
                        srch.User.RemoveComment(srch.comment.index);
                    }
                    break;



                case ActAddStream:
                    {
                        UnityEngine.Debug.Log("Se ejecuto el add stream");
                        instance.AddStream(dataRpc.data);
                    }
                    break;

                case ActCreateStream:
                    {
                        if (IsServer)
                        {
                            DataRpc.Create(Actions.AddStream, "", new StreamerData(dataBase.SelectStreamer()));
                        }
                    }
                    break;



                case ActStartUpdateStreamers:
                    {
                        if (IsServer)
                        {
                            //ejecuto la pausa para todos con un rpc
                            Rpc_GlobalPause();
                            instance.StartCoroutine(instance.PrependUpdate(JsonUtility.ToJson(streamersData)));
                        }
                    }
                    break;

                case ActEndUpdateStreamers:
                    {
                        UnityEngine.Debug.Log("SE EJECUTÓ EndUpdateStreamers");
                        //Reanudo la partida para todos
                        GlobalUnPause();
                    }
                    break;

                case ActChangeToFirstStream:
                    {
                        eventQueue.Enqueue(() => ChangeStream(0));
                    }
                    break;

            }
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
        DataRpc.Create(Actions.ChangeToFirstStream);
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

            DataRpc.Create(Actions.EndUpdateStreamers);
            ChangeStream(0);
            CreateStream();
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
        //-----------------------------
        UnityEngine.Debug.Log("*************STREAMERS LENGHT: " + Count);
        UnityEngine.Debug.Log("***************TRY INDEX: " + IndexStreamWatch);

        if (Count < 1)
            ChangeStream(index);

        //-----------------------------
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
        {
            CreateFirstStream();
            Rpc_GlobalPause();
        } 
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

        while (FilterRpc.Count > 0)
        {
            instance.Rpc_Execute(FilterRpc.definitiveList);
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


[System.Serializable]
public class Actions
{
    [SerializeField]
    public static Actions Ban = new ActBan();
    [SerializeField]
    public static Actions Admonition = new ActAdmonition();
    [SerializeField]
    public static Actions Picantear = new ActPicantear();
    [SerializeField]
    public static Actions Corromper = new ActCorromper();
    [SerializeField]
    public static Actions Suspect = new ActSus();
    [SerializeField]
    public static Actions AddUser = new ActAddUser();
    [SerializeField]
    public static Actions RemoveUser = new ActRemoveUser();
    [SerializeField]
    public static Actions AddComment = new ActAddComment();
    [SerializeField]
    public static Actions RemoveComment = new ActRemoveComment();
    [SerializeField]
    public static Actions CreateStream = new ActCreateStream();
    [SerializeField]
    public static Actions AddStream = new ActAddStream();
    [SerializeField]
    public static Actions RemoveStream = new ActRemoveStream();
    [SerializeField]
    public static Actions StartUpdateStreamers = new ActStartUpdateStreamers();
    [SerializeField]
    public static Actions EndUpdateStreamers = new ActEndUpdateStreamers();
    [SerializeField]
    public static Actions ChangeToFirstStream = new ActChangeToFirstStream();
}
[System.Serializable] public class ActionStream : Actions { }
[System.Serializable] public class ActCreateStream : ActionStream { }
[System.Serializable] public class ActAddStream : ActionStream { }
[System.Serializable] public class ActRemoveStream : ActionStream { }
[System.Serializable] public class ActChangeToFirstStream : ActionComment { }
[System.Serializable] public class ActStartUpdateStreamers : ActionStream { }
[System.Serializable] public class ActEndUpdateStreamers : ActionStream { }


[System.Serializable] public class ActionUser : Actions { }
[System.Serializable] public class ActAddUser : ActionUser { }
[System.Serializable] public class ActRemoveUser : ActionUser { }
[System.Serializable] public class ActBan : ActionUser { }
[System.Serializable] public class ActAdmonition : ActionUser { }
[System.Serializable] public class ActPicantear : ActionUser { }
[System.Serializable] public class ActCorromper : ActionUser { }
[System.Serializable] public class ActSus : ActionUser { }


[System.Serializable] public class ActionComment : Actions { }
[System.Serializable] public class ActAddComment : ActionComment { }
[System.Serializable] public class ActRemoveComment : ActionComment { }

[System.Serializable]
public class SerializedDataRpc
{
    [SerializeField]
    public string newAction;
    [SerializeField]
    public string direction;
    [SerializeField]
    public string data;
}

[System.Serializable]
public class DataRpc
{
    [SerializeReference]
    public Actions newAction;

    public string direction;

    public string data;

    public static void Create<T>(T action) where T : Actions
    {
        //UnityEngine.Debug.Log(action);

        var myAction = JsonUtility.ToJson(new AuxWrapper<T>(action));
        FilterRpc.Filter(JsonUtility.ToJson(new SerializedDataRpc() { newAction = myAction}), action);
    }
    public static void Create<T>(T action, string direction) where T : Actions
    {
        //UnityEngine.Debug.Log(action + ": " + direction);

        var myAction = JsonUtility.ToJson(new AuxWrapper<T>(action));
        FilterRpc.Filter(JsonUtility.ToJson(new SerializedDataRpc() { newAction = myAction, direction = direction }), action);
    }

    public static void Create<T>(T action, string direction, string data) where T : Actions
    {
        //UnityEngine.Debug.Log(action + ": " + direction + " " + data);

        var myAction = JsonUtility.ToJson(new AuxWrapper<T>(action));
        FilterRpc.Filter(JsonUtility.ToJson(new SerializedDataRpc() { newAction = myAction, direction = direction, data = data }), action);
    }

    public static void Create<T>(T action, string direction, object data) where T : Actions
    {
        //UnityEngine.Debug.Log(action + ": " + direction + " " + JsonUtility.ToJson(data, true));

        var myAction = JsonUtility.ToJson(new AuxWrapper<T>(action));
        FilterRpc.Filter(JsonUtility.ToJson(new SerializedDataRpc() { newAction = myAction, direction = direction, data = JsonUtility.ToJson(data) }), action);
    }

    public override string ToString()
    {
        return $"accion: {newAction}\ndireccion: {direction}\ndata: {data}";
    }
}


public static class FilterRpc
{
    static List<string> streamsRequests = new List<string>();
    static List<string> usersRequests = new List<string>();
    static List<string> commentsRequests = new List<string>();

    public static int Count => streamsRequests.Count + usersRequests.Count + commentsRequests.Count;

    public static string definitiveList
    {
        get
        {
            var aux = JsonUtility.ToJson(new AuxWrapper<string[]>(streamsRequests.Concat(usersRequests).Concat(commentsRequests).ToArray()), true);
            //UnityEngine.Debug.Log("JSON emviado: \n" + aux + "\n\n");

            streamsRequests.Clear();
            usersRequests.Clear();
            commentsRequests.Clear();

            return aux;
        }
    }
    
    public static void Filter(string data, Actions action)
    {
        if (action is ActionStream)
            streamsRequests.Add(data);
        else if (action is ActionUser)
            usersRequests.Add(data);
        else if (action is ActionComment)
            commentsRequests.Add(data);
    }
}


[System.Serializable]
public struct AuxWrapper<T>
{
    [SerializeReference]
    public T data;

    public AuxWrapper(T data)
    {
        this.data = data;
    }
}