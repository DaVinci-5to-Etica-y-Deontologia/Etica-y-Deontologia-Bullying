using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[CreateAssetMenu(menuName = "Managers/EventManager")]
public class EventManager : ScriptableObject
{
    [SerializeField]
    Pictionarys<string, Euler.SpecificEventParent> _events = new Pictionarys<string, Euler.SpecificEventParent>();

    public Pictionarys<string, Euler.SpecificEventParent> events => _events;

    public void Trigger(string nameOfEvent)
    {
        _events[nameOfEvent].delegato?.DynamicInvoke();
    }

    public void MyOnDestroy()
    {
        for (int i = 0; i < _events.Count; i++)
        {
            if(_events[i]!=null)
                _events[i].delegato = null;
        }
    }
}

public class EventParam : Euler.SpecificEvent<UnityAction> { };

public class EventParam<T1> : Euler.SpecificEvent<UnityAction<T1>> { };

public class EventParam<T1, T2> : Euler.SpecificEvent<UnityAction<T1, T2>> { };

public class EventParam<T1, T2, T3> : Euler.SpecificEvent<UnityAction<T1, T2, T3>> { };


namespace Euler
{
    public class SpecificEventParent
    {
        public System.Delegate delegato { get; set; }
    }

    public class SpecificEvent<T> : SpecificEventParent where T : System.Delegate
    {
        new public T delegato { get => (T)base.delegato; set => base.delegato = value; }

        public void Suscribe(T action)
        {
            if (delegato == null)
            {
                delegato = action;
                return;
            }

            delegato = (T)System.Delegate.Combine(delegato, action);
        }

        public void Desuscribe(T action)
        {
            delegato = (T)System.Delegate.Remove(delegato, action);
        }

        public static SpecificEvent<T> operator -(SpecificEvent<T> a, T b)
        {
            a.Desuscribe(b);
            return a;
        }

        public static SpecificEvent<T> operator +(SpecificEvent<T> a, T b)
        {
            a.Suscribe(b);
            return a;
        }
    }
}


