using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{ 
    public EventManager eventManager;
    
    public EventCallsManager callsManager;

    [SerializeField]
    PopUpElement[] popUpElements;

    protected void Awake()
    {
        popUpElements = GetComponentsInChildren<PopUpElement>(true);

        foreach (var item in popUpElements)
        {
            item.MyAwake(this);
        }
    }
}

public abstract class PopUpElement : MonoBehaviour
{
    public PopUpManager parent;

    protected EventManager eventManager => parent.eventManager;

    public EventCallsManager callsManager=> parent.callsManager;


    public UnityEngine.Events.UnityEvent onActive;

    public UnityEngine.Events.UnityEvent onExecute;

    /// <summary>
    /// Funcion que ejecutara el popmanager en el awake, necesario para setear el eventmanager y el callsmanager
    /// </summary>
    /// <param name="popUpManager"></param>
    virtual public void MyAwake(PopUpManager popUpManager)
    {
        parent = popUpManager;
    }
}

