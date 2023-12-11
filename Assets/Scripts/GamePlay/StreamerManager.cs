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

        [field: SerializeField]
        public int Count { get; set; }

        public StreamerManager streamerManager => _streamerManager;

        public BD dataBase => streamerManager.dataBase;

        public EventManager eventManager => streamerManager.eventManager;

        public Player player => streamerManager.player;

        public Pictionarys<string, bool> playersReady = new();


        StreamerManager _streamerManager;

        public bool IsServer => streamerManager.IsServer;
        public Data(StreamerManager streamerManager)
        {
            _streamerManager = streamerManager;
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

    /*
    [SerializeField]
    AuxWrapper<DataRpc[]> listRpc;
    */



    public StreamerData Actual { get; private set; }

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

    public bool IsServer => Runner.IsServer || Runner.IsSinglePlayer;

    

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



    #region Actions Functions

    static void AddStream(string streamJson)
    {
        var streamer = JsonUtility.FromJson<StreamerData>(streamJson);

        instance.streamersData.streamers.Add(streamer.CreatePic());

        streamer.Create();

        instance.onStreamerCreate.delegato?.Invoke(streamer);
    }


    static public void CreateStream()
    {
        DataRpc.Create(Actions.CreateStream);
    }

    static public void CreateStream(string jsonData, StreamerManager.SearchResult srch)
    {
        if (instance.IsServer)
        {
            DataRpc.Create(Actions.AddStream, "", new StreamerData(instance.streamersData.streamers.Prepare(), instance.dataBase.SelectStreamer()));
        }
    }

    static public void AddNewStream(string jsonData, StreamerManager.SearchResult srch)
    {
        bool aux = instance.streamersData.Count == 0;

        AddStream(jsonData);

        if (aux)
        {
            instance.ChangeStream(0);
        }
    }

    static public void StartUpdateStreamers(string jsonData, StreamerManager.SearchResult srch)
    {
        if (instance.IsServer)
        {
            instance.StopAllCoroutines();

            instance.Rpc_GlobalPause();

            instance.StartCoroutine(instance.PrependUpdate(JsonUtility.ToJson(instance.streamersData)));

            TransitionManager.instance.ChangeText("Un jugador nuevo se esta uniendo a la partida");
        }
    }
    static public void EndUpdateStreamers(string jsonData, StreamerManager.SearchResult srch)
    {
        TransitionManager.instance.SetTransition(TransitionManager.WaitEnd);
        DataRpc.Create(Actions.StartGame, "", instance.player.ID.ToString());
    }

    static public void StartReady(string jsonData, StreamerManager.SearchResult srch)
    {
        var playersReady = instance.streamersData.playersReady;

        if (playersReady.TryGetValue(jsonData, out var ready))
        {
            playersReady[jsonData] = !ready;
        }
        else
        {
            playersReady.Add(jsonData, false);
        }

        bool chckReady = playersReady.All((p) => p.Value);

        if(chckReady)
            instance.GlobalUnPause();

        instance.eventManager.events.SearchOrCreate<EventParam<bool>>("allready").delegato.Invoke(chckReady);
    }

    #endregion


    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_Execute(string json)
    {
        if (!started)
            return;

        //listRpc = JsonUtility.FromJson<AuxWrapper<DataRpc[]>>(json);

        DataRpc dataRpc = JsonUtility.FromJson<DataRpc>(json);

        //for (int i = 0; i < listRpc.data.Length; i++)
        {
            //dataRpc = listRpc.data[i];

            var srch = Search(dataRpc.direction);

            actionsMap[dataRpc.action].Invoke(dataRpc.data, srch);

            //UnityEngine.Debug.Log($"Recibido:\n{dataRpc}");
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
        
        
        //UnityEngine.Debug.Log("El juego se des pauso");
    }



    /// <summary>
    /// Ejecuta el llamado para terminar la partida
    /// </summary>
    public void FinishDay()
    {
        streamersData.gameEnd = true;

        streamersData.endGame.Stop();

        foreach (var item in streamersData.streamers)
        {
            item.Value.Stop();
        }

        onFinishDay.delegato.Invoke();
    }

    public void CreateFirstStream()
    {
        started = true;
        CreateStream();
    }

    public IEnumerator PrependUpdate(string json)
    {
        int order = 0;
        //UnityEngine.Debug.Log("EMPEZO A CARGAR LOS DATOS");
        do
        {
            Rpc_RequestUpdate(json.SubstringClamped(0, 1000), order , false);
            json = json.SubstringClamped(1000);
            yield return null;
            order++;
        }
        while (json.Length > 1000);

        Rpc_RequestUpdate(json, order, true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_RequestUpdate(string json , int order, bool end)
    {
        if (started)
            return;

        if (!end)
        { 
            if(!buffer.ContainsKey(order))
                buffer.Add(order, json);
            else
                buffer[order] = json;
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
                        comment.Value.Parent = UserData.poolCommentData;
                        comment.Value.Init(user.Value);
                    }
                }
            }

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
                UIManager.PressButtonByID(item.Key);
                return;
            }
        }
    }

    public void ChangeStream(int index)
    {
        if (index == IndexStreamWatch)
            return;

        var previus = IndexStreamWatch;
        //print("Previous index: " + previus);

        IndexStreamWatch = index;
        //print("Index Stream Watch: " + IndexStreamWatch);

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

        if (Actual.ShowEnd)
        {
            Aux_onEndStream(aux);
        }
    }

    void Aux_onEndStream(StreamerData obj)
    {
        onStreamEnd.delegato.Invoke(obj);
        //DataRpc.Create(Actions.FinishStream);
    }
    void CommentCreateQueue(CommentData commentData)
    {
        onCreateComment.delegato.Invoke(commentData);
    }

    void CommentLeaveQueue(CommentData commentData)
    {
        onLeaveComment.delegato.Invoke(commentData);
    }

    

    void MyStart()
    {
        print("Comienza el juego");

        streamersData.endGame.Reset();

        if (IsServer)
        {
            CreateFirstStream();
            print("Game mode: " + Runner.GameMode);

            if (Runner.GameMode == GameMode.Host)
            {
                TransitionManager.instance.ChangeText("Esperando a otro jugador para empezar la partida");
                Rpc_GlobalPause();
            }
            else
            {
                DataRpc.Create(Actions.StartGame, "", instance.player.ID.ToString());
            }
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

        do
        {
            if (eventQueue.TryDequeue(out var action))
                action();
        }
        while (eventQueue.Count > 0 && watchdog.Elapsed.TotalMilliseconds < 16);

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
    }

    public void EndLoad()
    {
        TimersManager.Create(0.1f, MyStart);
    }

    public override void Spawned()
    {
        watchdog.Start();

        UnityEngine.Debug.Log("server: " + IsServer);

        player.Moderator = (Runner.IsServer && Runner.SessionInfo.PlayerCount % 2 != 0) || Runner.IsSinglePlayer;

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

    //const int limitRpc = 2; // el limite de caracteres es de: 32767

    //static int sum;

    public static void Create(Actions action)
    {
        //UnityEngine.Debug.Log(action);
        Filter(new DataRpc() { _action = action, data = string.Empty, direction = string.Empty });
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
            /*
            if((sum + dataRpcs.Peek().data.Length) < limitRpc)
            {
                sum += dataRpcs.Peek().data.Length;
            }
            else
            {
                finish = true;
                break;
            }
            */

            //if (++sum > limitRpc)
                finish = true;
            

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

            //var aux = JsonUtility.ToJson(new AuxWrapper<DataRpc[]>(_definitiveList.ToArray()));

            var aux = JsonUtility.ToJson(_definitiveList[0]);

            //UnityEngine.Debug.Log($"JSON enviado: {sum} de {aux.Length}  \n  {aux}  \n\n");

            /*
            UnityEngine.Debug.Log("Streamer Request Count: " + streamsRequests.Count);
            UnityEngine.Debug.Log("Users Request Count: " + usersRequests.Count);
            UnityEngine.Debug.Log("Comments Request Count: " + commentsRequests.Count);
            */

            _definitiveList.Clear();
            //sum = 0;
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