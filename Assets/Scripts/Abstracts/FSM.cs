using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM<T, Context> : FSMSerialize<T, Context> where T : FSM<T, Context>
{ 
    protected FSM(Context reference)
    {
        Init(reference);
    }
}

[System.Serializable]
public abstract class FSMSerialize<T, Context> : ISwitchState<T> where T : FSMSerialize<T, Context>
{
    [HideInInspector]
    public Context context;

    IState<T> currentState;

    public IState<T> CurrentState
    {
        get => currentState;
        set => SwitchState(value);
    }

    T FSMConvertToChild()
    {
        return (T)this;
    }

    void SwitchState(IState<T> state)
    {
        if (state == currentState || state == null)
            return;

        currentState.OnExitState(FSMConvertToChild());
        Init(state);
    }

    public void UpdateState()
    {
        currentState.OnStayState(FSMConvertToChild());
    }

    /// <summary>
    /// Obligatorio para setear el estado inicial
    /// </summary>
    /// <param name="first"></param>
    protected void Init(IState<T> first)
    {
        currentState = first;
        currentState.OnEnterState(FSMConvertToChild());
    }

    /// <summary>
    /// Obligatorio en caso de que se utilice la version serializable para unity
    /// </summary>
    /// <param name="reference"></param>
    public virtual void Init(Context reference)
    {
        this.context = reference;
    }
}