using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    public static event UnityAction onPause
    {
        add
        {
            instance._fsmGameMaganer.pause.onPause += value;
        }

        remove
        {
            instance._fsmGameMaganer.pause.onPause -= value;
        }
    }

    public static event UnityAction onPlay
    {
        add
        {
            instance._fsmGameMaganer.pause.onPlay += value;
        }

        remove
        {
            instance._fsmGameMaganer.pause.onPlay -= value;
        }
    }
    public static Pictionarys<MyScripts, UnityAction> fixedUpdate => instance._fixedUpdate;
    public static Pictionarys<MyScripts, UnityAction> update => instance._update;

    public static GameManager instance;

    [SerializeField]
    EventManager _eventManager;

    [Header("Executions")]
    [Space]

    [Tooltip("Evento llamado en el primer awake")]
    public UnityEvent awakeUnityEvent;

    [Tooltip("Evento llamado en el primer update")]
    public UnityEvent updateUnityEvent;

    [Tooltip("Evento llamado en el primer fixedUpdate")]
    public UnityEvent fixedUpdateUnityEvent;

    [Tooltip("Evento llamado cuando se destruye el GameManager, usualmente en el cambio de escena")]
    public UnityEvent onDestroyUnityEvent;

    [Header("Game States")]
    [Space]

    [Tooltip("Evento llamado cuando se entra o se sale del gameplay")]
    public UnityEvent<bool> onPlayUnityEvent;

    [Tooltip("Evento llamado cuando se comienza la carga")]
    public UnityEvent onStartLoad;

    [Tooltip("Evento llamado cuando se finaliza la carga")]
    public UnityEvent onFinishLoad;

    [Header("Conditions events")]
    [Space]

    [Tooltip("Evento llamado cuando se la condicion de victoria")]
    public UnityEvent victory;

    [Tooltip("Evento llamado cuando se la condicion de derrota")]
    public UnityEvent defeat;



    Pictionarys<MyScripts, UnityAction> _update = new Pictionarys<MyScripts, UnityAction>();

    Pictionarys<MyScripts, UnityAction> _fixedUpdate = new Pictionarys<MyScripts, UnityAction>();

    FSMGameMaganer _fsmGameMaganer;

    #region funciones

    public void OnLoad()
    {
        _fsmGameMaganer.CurrentState = _fsmGameMaganer.load;
    }

    public void EndLoad()
    {
        _fsmGameMaganer.CurrentState = _fsmGameMaganer.gamePlay;
    }

    public void TogglePause()
    {
        if (_fsmGameMaganer.CurrentState != _fsmGameMaganer.load)
            _fsmGameMaganer.CurrentState = (_fsmGameMaganer.CurrentState == _fsmGameMaganer.pause) ? _fsmGameMaganer.gamePlay : _fsmGameMaganer.pause;
    }

    public void Pause(bool pause)
    {
        if(_fsmGameMaganer.CurrentState != _fsmGameMaganer.load)
            _fsmGameMaganer.CurrentState = (!pause) ? _fsmGameMaganer.gamePlay : _fsmGameMaganer.pause;
    }
    
    public void Defeat()
    {
        TimersManager.Create(1f, 0f, 2, Mathf.Lerp, (save) => Time.timeScale = save).AddToEnd(() =>
        {
            defeat.Invoke();
        }).SetUnscaled(true);
    }

    public void Victory()
    {
        victory.Invoke();
    }

    public void Quit()
    {
        Application.Quit();
    }

    void MyUpdate(Pictionarys<MyScripts, UnityAction> update)
    {
        for (int i = 0; i < update.Count; i++)
        {
            update[i]();
        }
    }

    void MyUpdate()
    {
        MyUpdate(_update);
    }

    void MyFixedUpdate()
    {
        MyUpdate(_fixedUpdate);
    }

    public override void Spawned()
    {
        _fsmGameMaganer = new FSMGameMaganer(this);

        var victory = _eventManager.events.SearchOrCreate<EventParam>("victory");
        var close = _eventManager.events.SearchOrCreate<EventParam>("close");
        var defeat = _eventManager.events.SearchOrCreate<EventParam>("defeat");

        victory.delegato += Victory;
        victory.delegato += () => close.delegato.Invoke();
        defeat.delegato += Defeat;
        defeat.delegato += () => close.delegato.Invoke();

        awakeUnityEvent?.Invoke();

        updateUnityEvent.AddListener(MyUpdate);

        fixedUpdateUnityEvent.AddListener(MyFixedUpdate);

        enabled = true;
    }

    protected void Awake()
    {
        instance = this;

        enabled = false;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
            Spawned();
            
    }

    private void Update()
    {
        updateUnityEvent.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateUnityEvent.Invoke();
    }

    private void OnDestroy()
    {
        _eventManager.events.SearchOrCreate<EventParam>("victory").delegato -= Victory;

        _eventManager.events.SearchOrCreate<EventParam>("defeat").delegato -= Defeat;

        onDestroyUnityEvent?.Invoke();
    }

    #endregion
}

public class FSMGameMaganer : FSM<FSMGameMaganer, GameManager>
{
    public Load load = new Load();
    public Gameplay gamePlay = new Gameplay();
    public Pause pause = new Pause();

    public FSMGameMaganer(GameManager reference) : base(reference)
    {
        Init(load);
    }
}

public class Load : IState<FSMGameMaganer>
{
    public void OnEnterState(FSMGameMaganer param)
    {
        param.context.onStartLoad.Invoke();
    }

    public void OnExitState(FSMGameMaganer param)
    {
        param.context.onFinishLoad.Invoke();
    }

    public void OnStayState(FSMGameMaganer param)
    {
    }
}

public class Gameplay : IState<FSMGameMaganer>
{
    public void OnEnterState(FSMGameMaganer param)
    {
        param.context.onPlayUnityEvent.Invoke(true);
    }

    public void OnExitState(FSMGameMaganer param)
    {
        param.context.onPlayUnityEvent.Invoke(false);
    }

    public void OnStayState(FSMGameMaganer param)
    {
    }
}

public class Pause : IState<FSMGameMaganer>
{
    public event UnityAction onPause;

    public event UnityAction onPlay;

    public void OnEnterState(FSMGameMaganer param)
    {
        Time.timeScale = 0;
        onPause?.Invoke();
        param.context.enabled = false;
    }

    public void OnExitState(FSMGameMaganer param)
    {
        onPlay?.Invoke();
        Time.timeScale = 1;
        param.context.enabled = true;
    }

    public void OnStayState(FSMGameMaganer param)
    {
    }
}