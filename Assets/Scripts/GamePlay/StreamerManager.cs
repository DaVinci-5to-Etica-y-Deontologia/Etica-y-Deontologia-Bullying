using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamerManager : SingletonMono<StreamerManager>
{

    public BD dataBase;

    public EventManager eventManager;

    [SerializeField]
    CommentView prefab;

    [SerializeField, Range(10, 60)]
    float startDeley;

    [SerializeField]
    UnityEngine.UI.ContentSizeFitter contain;

    [SerializeField]
    DataPic<Streamer> streamers = new();


    [SerializeField]
    float multiply = 1;

    
    int indexStreamWatch
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

    [SerializeField]
    int _indexStreamWatch = 0;


    Timer delay;

    public Streamer this[int ID]
    {
        get
        {
            return streamers.GetTByID(ID);
        }
    }

    public static void Execute(string action, string direction)
    {
        var splirDir = direction.Split('.');

        var stream = instance[int.Parse(splirDir[0])];

        var user = stream[int.Parse(splirDir[1])];

        var comment = user[int.Parse(splirDir[2])];

        switch (action)
        {
            case Actions.Ban:
                user.Ban(comment.ID);
                break;

            case Actions.Admonition:
                user.Admonition(comment.ID);
                break;
        }
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

    public void CreateStream()
    {
        var streamer = new Streamer();

        streamers.Add(streamer);

        streamer.Create(streamers.lastID , Random.Range(5,10));
    }

    void MyStart()
    {
        print("Comienza el juego");
        CreateStream();
        CreateStream();
        CreateStream();
        ChangeStream(0);
    }

    public void ChangeStream(int index)
    {
        var previus = indexStreamWatch;

        indexStreamWatch = index;

        streamers.GetTByIndex(previus).onCreateComment -= StreamerManager_onCreateComment;

        foreach (Transform item in contain.transform)
        {
            Destroy(item.gameObject);
        }

        var aux = streamers.GetTByIndex(indexStreamWatch);

        aux.onCreateComment += StreamerManager_onCreateComment;

        foreach (var item in aux.commentViews)
        {
            StreamerManager_onCreateComment(item.Value);
        }
    }

    private void StreamerManager_onCreateComment(CommentData obj)
    {
        SpawnComment().commentData = obj;
    }

    protected override void Awake()
    {
        base.Awake();

        TimersManager.Create(startDeley, MyStart);

        //onFinishDay = eventManager.events.SearchOrCreate<EventParam<(int, int)>>("finishday");
    }

    string json;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ChangeStream(indexStreamWatch + 1);

        if (Input.GetKeyDown(KeyCode.Q))
            ChangeStream(indexStreamWatch - 1);
    }
}

public class Actions
{
    public const string Ban = "Ban";

    public const string Admonition = "Admonition";

    //public const string Ban = "Ban";
}