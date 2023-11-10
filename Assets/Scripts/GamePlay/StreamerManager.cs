using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;

public class StreamerManager : SingletonMono<StreamerManager>
{
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

    public Streamer this[int ID]
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

    public static void Execute(string action, string direction)
    {
        var srch = Search(direction);

        switch (action)
        {
            case Actions.Ban:
                srch.user.Ban(srch.comment.ID);
                break;

            case Actions.Admonition:
                srch.user.Admonition(srch.comment.ID);
                break;
        }
    }

    public static (Streamer streamer, User user, CommentData comment) Search(string dir)
    {
        var splirDir = dir.Split('.');

        Streamer stream = null;

        User user =null;

        CommentData comment =  null;

        if(splirDir.Length > 0)
        {
            stream = instance[int.Parse(splirDir[0])];

            if(stream!=null && splirDir.Length > 1)
            {
                user = stream[int.Parse(splirDir[1])];

                if(user!=null && splirDir.Length > 2)
                {
                    comment = user[int.Parse(splirDir[2])];
                }
            }
        }

        return (stream, user, comment);
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

        var aux = streamers.GetTByIndex(previus);

        aux.onCreateComment -= CommentCreateQueue;

        aux.onLeaveComment -= CommentLeaveQueue;

        aux = streamers.GetTByIndex(indexStreamWatch);

        aux.onCreateComment += CommentCreateQueue;

        aux.onLeaveComment += CommentLeaveQueue;
        Actual = aux;
    }

    void CommentCreateQueue(CommentData commentData)
    {
        onCreateComment.delegato.Invoke(commentData);
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

    //public const string Ban = "Ban";
}