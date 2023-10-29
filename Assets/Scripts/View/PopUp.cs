using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopUp : MonoBehaviour
{
    [SerializeField]
    protected EventManager eventManager;

    [SerializeField]
    protected EventCallsManager callsManager;

    public UnityEngine.Events.UnityEvent onActive;

    public UnityEngine.Events.UnityEvent onExecute;
}
