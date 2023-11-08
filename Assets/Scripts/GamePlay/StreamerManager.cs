using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

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

    Pool<CommentView> pool= new();

    
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
    public Streamer Actual { get; private set; }

    public Streamer this[int ID]
    {
        get
        {
            return streamers.GetTByID(ID);
        }
    }


    [SerializeField]
    int _indexStreamWatch = 0;


    Timer delay;

    

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

        var aux = streamers.GetTByIndex(previus);

        aux.onCreateComment -= StreamerManager_onCreateComment;

        foreach ( var item in aux.commentViews)
        {
            item.Value.Destroy();
        }

        aux = streamers.GetTByIndex(indexStreamWatch);

        aux.onCreateComment += StreamerManager_onCreateComment;

        foreach (var item in aux.commentViews)
        {
            StreamerManager_onCreateComment(item.Value);
        }

        Actual = aux;
    }

    private void StreamerManager_onCreateComment(CommentData obj)
    {
        var aux = pool.Obtain().Self;

        aux.transform.SetAsLastSibling();

        aux.commentData = obj;

        aux.SetActiveGameObject(false);
    }

    protected override void Awake()
    {
        base.Awake();

        TimersManager.Create(startDeley, MyStart);

        StartCoroutine(pool.CreatePool(100, prefab));

        //onFinishDay = eventManager.events.SearchOrCreate<EventParam<(int, int)>>("finishday");
    }

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

public interface IPoolElement<T> where T : IPoolElement<T>
{
    Pool<T> Parent { get; set; }

    IPoolElement<T> Next { get; set; }

    T Create();

    void Destroy();

    T Self => (T)this;
}


public class Pool<T> where T : IPoolElement<T>
{
    public IPoolElement<T> first;

    public IPoolElement<T> last;

    T model;

    public IEnumerator CreatePool(int cantidad, T model)
    {
        this.model = model;

        CreateFirst();

        var wachdog = new Stopwatch();

        wachdog.Start();

        for (int i = 0; i < cantidad; i++)
        {
            last.Next = model.Create();

            last = last.Next;

            last.Parent = this;

            if (wachdog.ElapsedMilliseconds > 1/60 * 1000)
            {
                wachdog.Reset();
                yield return null;
            }
        }
    }

    public IPoolElement<T> Obtain()
    {
        var ret = first;

        if(first.Next!=null)
            first = first.Next;
        else
        {
            CreateFirst();
        }
            

        return ret;
    }

    public void Return(IPoolElement<T> poolElement)
    {
        if(last==null)
        {
            last = poolElement;
            first = last;
        }
        else
        {
            last.Next = poolElement;

            last = last.Next;
        }

        last.Next = null;
    }

    void CreateFirst()
    {
        first = model.Create();

        last = first;

        last.Parent = this;
    }
}