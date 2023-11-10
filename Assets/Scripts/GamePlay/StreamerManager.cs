using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;

public class StreamerManager : SingletonMono<StreamerManager>
{
    public struct SearchResult
    {
        public (Streamer value, int ID, int index) streamer;
        public (User value, int ID, int index) user;
        public (CommentData value, int ID, int index) comment;

        public Streamer Streamer => streamer.value;

        public User User => user.value;

        public CommentData CommentData => comment.value;
    }

    public BD dataBase;

    public EventManager eventManager;

    public static Queue<System.Action> eventQueue = new();

    [SerializeField]
    CommentView prefab;

    [SerializeField, Range(1, 60)]
    float startDeley;

    [SerializeField]
    ChatContentRefresh contain;

    [SerializeField]
    DataPic<Streamer> streamers = new();

    [SerializeField]
    float multiply = 1;

    LinkedPool<CommentView> pool;

    EventParam<CommentData> onCreateComment;

    EventParam<CommentData> onLeaveComment;

    Stopwatch watchdog;

    public int indexStreamWatch
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

    public (Streamer value, int ID, int index) this[int ID]
    {
        get
        {
            return streamers.GetTByID(ID);
        }
    }

    public Streamer Actual { get; private set; }

    [SerializeField]
    int _indexStreamWatch = 0;

    Timer delay;

    //RPC REAL
    public static void Execute(string json)
    {
        DataRpc dataRpc = JsonUtility.FromJson<DataRpc>(json);

        //UnityEngine.Debug.Log($"recibido:\n{dataRpc}");

        var srch = Search(dataRpc.direction);

        switch (dataRpc.action)
        {
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

        }
    }
    
    

    public static SearchResult Search(string dir)
    {
        var splirDir = dir.Split('.');

        SearchResult searchResult = new SearchResult();

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


    /// <summary>
    /// Calcula la sumatoria final de todo el danio y la cantidad de viewers
    /// </summary>
    public void FinishDay()
    {
        //onFinishDay.delegato.Invoke(sumSeed);
    }

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

    public void CreateStream()
    {
        var streamer = new Streamer();

        streamers.Add(streamer);

        streamer.Create(streamers.lastID , Random.Range(5,10));
    }

    public void ChangeStream(int index)
    {
        var previus = indexStreamWatch;

        indexStreamWatch = index;

        var aux = streamers.GetTByIndex(previus).value;

        aux.onCreateComment -= CommentCreateQueue;

        aux.onLeaveComment -= CommentLeaveQueue;

        aux = streamers.GetTByIndex(indexStreamWatch).value;

        aux.onCreateComment += CommentCreateQueue;

        aux.onLeaveComment += CommentLeaveQueue;
        Actual = aux;
    }

    void CommentCreateQueue(CommentData commentData)
    {
        onCreateComment.delegato.Invoke(commentData);

        //si autoridad de estado bla bla bla

        var timerDestroy = TimersManager.Create(30, () =>
        {
            commentData.user.Aplicate(commentData.comment.Views, commentData.comment.Damage, commentData.textIP);
        });

        commentData.onDestroy += () => timerDestroy.Stop();
    }

    void CommentLeaveQueue(CommentData commentData)
    {
        onLeaveComment.delegato.Invoke(commentData);

        /*
        while (watchdog.ElapsedMilliseconds < (1f / 20) && LeaveCommentQueue.Count > 0)
        {
            
        }*/
    }

    void MyStart()
    {
        print("Comienza el juego");

        CreateStream();
        CreateStream();
        CreateStream();
        ChangeStream(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ChangeStream(indexStreamWatch + 1);

        if (Input.GetKeyDown(KeyCode.Q))
            ChangeStream(indexStreamWatch - 1);

        while(eventQueue.Count > 0 && watchdog.Elapsed.TotalMilliseconds < 16)
        {
            eventQueue.Dequeue().Invoke();
        }

        watchdog.Restart();
    }

    public void Load()
    {
        onCreateComment = eventManager.events.SearchOrCreate<EventParam<CommentData>>("createcomment");

        onLeaveComment = eventManager.events.SearchOrCreate<EventParam<CommentData>>("leavecomment");

        GameManager.instance.StartCoroutine(pool.CreatePool(30, ()=> eventManager.events.SearchOrCreate<EventParam>("poolloaded").delegato.Invoke()));
    }

    protected override void Awake()
    {
        base.Awake();

        TimersManager.Create(startDeley, MyStart);

        pool = new(prefab);

        watchdog = new Stopwatch();

        watchdog.Start();
    }

}

public class Actions
{
    public const string Ban = "Ban";

    public const string Admonition = "Admonition";

    public const string AddUser = "AddUser";

    public const string RemoveUser = "RemoveUser";

    public const string AddComment = "AddComment";

    public const string RemoveComment = "RemoveComment";

    //public const string Ban = "Ban";
}

public struct DataRpc
{
    public string action;

    public string direction;

    public string data;

    public static void Create(string action, string direction)
    {
        //UnityEngine.Debug.Log(action + ": " + direction);
        StreamerManager.Execute(JsonUtility.ToJson(new DataRpc() { action = action, direction = direction }));
    }

    public static void Create(string action, string direction, string data)
    {
        //UnityEngine.Debug.Log(action + ": " + direction + " " + data);
        StreamerManager.Execute(JsonUtility.ToJson(new DataRpc() { action = action, direction = direction , data = data}));
    }

    public static void Create(string action, string direction, object data)
    {
        //UnityEngine.Debug.Log(action + ": " + direction + " " + JsonUtility.ToJson(data, true));
        StreamerManager.Execute(JsonUtility.ToJson(new DataRpc() { action = action, direction = direction, data = JsonUtility.ToJson(data) }));
    }

    public override string ToString()
    {
        return $"accion: {action}\ndireccion: {direction}\ndata: {data}";
    }
}