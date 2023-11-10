using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : SingletonMono<PopUpManager>
{ 
    public EventManager eventManager;
    
    public EventCallsManager callsManager;

    [SerializeField]
    PopUpElement[] popUpElements;

    protected override void Awake()
    {
        base.Awake();
        foreach (var item in popUpElements)
        {
            item.Awake();
        }
    }
}

public abstract class PopUpElement : MonoBehaviour
{
    protected EventManager eventManager => PopUpManager.instance.eventManager;

    public EventCallsManager callsManager=> PopUpManager.instance.callsManager;


    public UnityEngine.Events.UnityEvent onActive;

    public UnityEngine.Events.UnityEvent onExecute;

    virtual public void Awake()
    {

    }
}

