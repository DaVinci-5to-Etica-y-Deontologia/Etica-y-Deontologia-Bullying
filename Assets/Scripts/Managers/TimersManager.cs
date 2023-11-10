using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(menuName="Managers/TimersManager")]
public class TimersManager : SingletonScript<TimersManager>
{
    public static List<Timer> timersList => instance._timersList;

    [SerializeField]
    List<Timer> _timersList = new List<Timer>();


    /// <summary>
    /// Crea un timer que se almacena en una lista para restarlos de forma automatica
    /// </summary>
    /// <param name="totTime2">el tiempo que dura el contador</param>
    /// <param name="m">el multiplicador del contador</param>
    /// <returns>Devuelve la referencia del contador creado</returns>
    public static Timer Create(float totTime2 = 10)
    {
        Timer newTimer = new Timer(totTime2);
        return newTimer;
    }

    /// <summary>
    /// Crea una rutina que ejecutara una funcion al cabo de un tiempo
    /// </summary>
    /// <param name="totTime">el tiempo total a esperar</param>
    /// <param name="action">la funcion que se ejecutara</param>
    /// <param name="loop">En caso de ser false se quita de la cola, y en caso de ser true se auto reinicia</param>
    /// <returns>retorna la rutina creada</returns>
    public static TimedAction Create(float totTime, Action action)
    {
        TimedAction newTimer = new TimedAction(totTime, action);
        return newTimer;
    }

    /// <summary>
    /// Crea una rutina completa, la cual ejecutara una funcion al comenzar/reiniciar, en el update, y al finalizar
    /// </summary>
    /// <param name="totTime"></param>
    /// <param name="update"></param>
    /// <param name="end"></param>
    /// <param name="loop">En caso de ser false se quita de la cola, y en caso de ser true se auto reinicia</param>
    /// <param name="unscaled"></param>
    /// <returns></returns>
    public static TimedCompleteAction Create(float totTime, Action update, Action end)
    {
        TimedCompleteAction newTimer = new TimedCompleteAction(totTime, update, end);
        return newTimer;
    }

    #region lerps

    static public TimedLerp<T> Create<T>(T original, T final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        return Create(() => original, () => final, seconds, Lerp, save);
    }

    static public TimedLerp<T> Create<T>(T original, System.Func<T> final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        return Create(() => original, final, seconds, Lerp, save);
    }

    static public TimedLerp<T> Create<T>(System.Func<T> original, T final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        return Create(original, () => final, seconds, Lerp, save);
    }

    static public TimedLerp<T> Create<T>(System.Func<T> original, System.Func<T> final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        return new TimedLerp<T>(original, final, seconds, Lerp, save);
    }

    static public TimedCompleteAction LerpWithCompare<T>(T original, T final, float velocity, System.Func<T, T, float, T> Lerp, System.Func<T, T, bool> compare, System.Action<T> save)
    {
        TimedCompleteAction tim = null;

        System.Action

        update = () =>
        {
            original = Lerp(original, final, Time.deltaTime * velocity);
            save(original);
            if (compare(original, final))
                tim.Set(0);
            else
                tim.Reset();
        }
        ,
        end = () =>
        {
            save(final);

        };

        tim = Create(1, update, end);

        return tim;
    }

    #endregion

    public void MyOnDestroy()
    {
        timersList.Clear();
    }

    public void MyUpdate()
    {
        for (int i = timersList.Count-1; i >= 0; i--)
        {
            while (i >= timersList.Count)
                i--;

            timersList[i].SubsDeltaTime(i);
        }
    }
}


[System.Serializable]
public class Tim : IGetPercentage
{
    /// <summary>
    /// Valor maximo en el cual clampea
    /// </summary>
    public float total;

    [SerializeField]
    protected float _current;

    protected bool notifyOnSuscribe=true;

    /// <summary>
    /// que hace cuando se setea el current
    /// </summary>
    protected System.Action<float> internalSetCurrent;

    /// <summary>
    /// Valor actual
    /// </summary>
    public virtual float current
    {
        get => _current;
        set
        {
            internalSetCurrent(value-_current); //lo seteo con un delegado para asi poder quitar o agregar la funcion de ejecutar un evento al modificar
        }
    }

    /// <summary>
    /// Valor maximo
    /// </summary>
    float IGetPercentage.total => total;

    protected event System.Action<IGetPercentage, float> _onChange; //version interna que almacera todos los que desean ser notificados cuando se modifique el timer

    /// <summary>
    /// Evento que se ejecutara cada vez que se actualice el valor actual
    /// </summary>
    public event System.Action<IGetPercentage, float> onChange
    {
        add
        {
            if (_onChange == null)
            {
                internalSetCurrent += InternalEventSetCurrent;
            }

            _onChange += value;

            if(notifyOnSuscribe)
                InternalEventSetCurrent(0);
        }

        remove
        {
            _onChange -= value;

            if (_onChange == null)
                internalSetCurrent -= InternalEventSetCurrent;
        }
    }

    /// <summary>
    /// Reinicia el contador a su valor por defecto, para reiniciar la cuenta
    /// </summary>
    public virtual Tim Reset()
    {
        current = total;

        return this;
    }

    /// <summary>
    /// Efectua una resta en el contador
    /// </summary>
    /// <param name="n">En caso de ser negativo(-) suma al contador, siempre y cuando no este frenado</param>
    public virtual float Substract(float n)
    {
        current -= n;
        return current;
    }

    /// <summary>
    /// Setea el contador
    /// </summary>
    /// <param name="totalTim">El numero a contar</param>
    public Tim Set(float totalTim)
    {
        total = totalTim;
        Reset();

        return this;
    }


    /// <summary>
    /// Si se desea ejecutar el evento onChange, cuando se suscribe un nuevo elemento
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public Tim NotifyOnSuscribe(bool b)
    {
        notifyOnSuscribe = b;
        return this;
    }

    public float Percentage()
    {
        return current / total;
    }

    public float InversePercentage()
    {
        return 1 - Percentage();
    }


    /// <summary>
    /// Clampeo de la variable
    /// </summary>
    /// <param name="value"></param>
    protected void InternalSetCurrent(float value)
    {
        _current += value;

        _current = Mathf.Clamp(_current, 0, total);
    }

    /// <summary>
    /// Trigger del evento
    /// </summary>
    /// <param name="value"></param>
    protected void InternalEventSetCurrent(float value)
    {
        _onChange(this, value);
    }

    public Tim()
    {
        internalSetCurrent = InternalSetCurrent;
    }

    public Tim(float totTim)
    {
        _current = totTim;
        total = totTim;
        internalSetCurrent = InternalSetCurrent;
    }
}


[System.Serializable]
public class Timer : Tim
{
    
    protected bool _unscaled;

    protected bool loop;

    protected float _multiply =1;

    bool _freeze = true; //por defecto no esta agregado


    /// <summary>
    /// delta time seleccionado del timer
    /// </summary>
    public float deltaTime
    {
        get
        {
            return _unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Propiedad que sirve para agregar o quitar de la cola para la resta
    /// </summary>
    bool freeze
    {
        get => _freeze;
        set
        {
            if (value == _freeze)
                return;

            if(value)
            {
                TimersManager.timersList.Remove(this);
            }
            else
            {
                TimersManager.timersList.Add(this);
            }

            _freeze = value;
        }
    }

    /// <summary>
    /// Chequea si el contador llego a su fin
    /// </summary>
    /// <returns>Devuelve true si llego a 0</returns>
    public bool Chck
    {
        get
        {
            return _current <= 0;
        }
    }

    /// <summary>
    /// Modifica el numero que multiplica la constante temporal, y asi acelerar o disminuir el timer
    /// </summary>
    /// <param name="m">Por defecto es 1</param>
    public Timer SetMultiply(float m)
    {
        _multiply = m;

        return this;
    }

    /// <summary>
    /// En caso de que el contador este detenido lo reanuda
    /// </summary>
    public Timer Start()
    {
        freeze = false;

        return this;
    }

    /// <summary>
    /// Frena el contador, no resetea ni modifica el contador actual
    /// </summary>
    public Timer Stop()
    {
        freeze = true;

        return this;
    }

    /// <summary>
    /// Setea el current, sin hacer trigger del evento
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public Timer SetInitCurrent(float f)
    {
        _current = f;
        return this;
    }

    /// <summary>
    /// Setea si el timer debe reiniciarse de forma automatica o no
    /// </summary>
    /// <param name="l"></param>
    /// <returns></returns>
    public Timer SetLoop(bool l)
    {
        loop = l;

        return this;
    }

    /// <summary>
    /// Setea el contador, y comienza la cuenta (si se quiere) desde ese numero
    /// </summary>
    /// <param name="totalTim">El numero a contar</param>
    /// <param name="f">Si arranca a contar o no</param>
    public Timer Set(float totalTim, bool f=true)
    {
        base.Set(totalTim);
        freeze = !f;

        return this;
    }

   
    /// <summary>
    /// Setea si utiliza el time.deltatime o el Time.unscaledDeltaTime
    /// </summary>
    /// <param name="u"></param>
    /// <returns></returns>
    public Timer SetUnscaled(bool u)
    {
        _unscaled = u;

        return this;
    }

    /// <summary>
    /// vuelve el valor current el maximo, y vuelve a poner al contador en la cola
    /// </summary>
    /// <returns></returns>
    public override Tim Reset()
    {
        base.Reset();
        Start();
        return this;
    }


    /// <summary>
    /// Realiza la resta automatica asi como las funciones necesarias dentro del TimerManager y recibe el indice dentro del manager
    /// </summary>
    /// <returns></returns>
    public virtual float SubsDeltaTime(int index)
    {
        var aux = Substract(deltaTime * _multiply);

        if (aux <= 0)
        {
            if (loop)
            {
                Reset();
                return 0;
            }
                
            else
                StopWithIndex(index);
        }

        return aux;
    }

    /// <summary>
    /// Stopea de forma interna
    /// </summary>
    /// <param name="index"></param>
    void StopWithIndex(int index)
    {
        _freeze = true;

        TimersManager.timersList.RemoveAt(index);
    }

    /// <summary>
    /// Configura el timer para su uso
    /// </summary>
    /// <param name="totTim">valor por defecto a partir de donde se va a contar</param>
    /// <param name="m">Modifica el multiplicador del timer, por defecto 0</param>
    public Timer(float totTim) : base(totTim)
    {
        Start();
    }
}

/// <summary>
/// rutina que ejecutara una accion desp de que termine el tiemer
/// </summary>
[System.Serializable] 
public class TimedAction : Timer
{
    protected Action end;


    public override float SubsDeltaTime(int index)
    {
        var aux = base.SubsDeltaTime(index);
        if(aux<=0)
        {
            end?.Invoke();
        }

        return aux;
    }

    /// <summary>
    /// Aniade un evento al evento end
    /// </summary>
    /// <param name="end"></param>
    /// <returns></returns>
    public TimedAction AddToEnd(Action end)
    {
        this.end += end;

        return this;
    }

    /// <summary>
    /// Quita un evento del end
    /// </summary>
    /// <param name="end"></param>
    /// <returns></returns>
    public TimedAction SubstractToEnd(Action end)
    {
        this.end -= end;

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="action"></param>
    public TimedAction(float timer, Action action) : base(timer)
    {
        this.end = action;
    }

}

/// <summary>
/// rutina que ejecutara una funcion al comenzar/reiniciar, otra en cada frame, y otra al final
/// </summary>
[System.Serializable]
public class TimedCompleteAction : TimedAction
{
    protected Action update;

    public override float SubsDeltaTime(int index)
    {
        update();

        return base.SubsDeltaTime(index);
    }


    /// <summary>
    /// Aniade un evento al update
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public TimedCompleteAction AddToUpdate(Action update)
    {
        this.update +=update;

        return this;
    }


    /// <summary>
    /// Quita un evento del Update
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public TimedCompleteAction SubstractToUpdate(Action update)
    {
        this.update -= update;

        return this;
    }

    /// <summary>
    /// crea una rutina que ejecutara una funcion al comenzar/reiniciar, otra en cada frame, y otra al final
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="update"></param>
    /// <param name="end"></param>
    public TimedCompleteAction(float timer, Action update, Action end) : base(timer, end)
    {
        this.update = update;
    }
}

public class TimedLerp<T> : TimedCompleteAction
{

    System.Func<T> original;
    System.Func<T> final;
    System.Func<T, T, float, T> lerp;
    public event System.Action<T> save;
    
    public TimedLerp<T> AddToSave(System.Action<T> save)
    {
        this.save += save;

        return this;
    }

    public TimedLerp<T> SubstractToSave(System.Action<T> save)
    {
        this.save -= save;

        return this;
    }
    void Update()
    {
        save(lerp(original(), final(), InversePercentage()));
    }

    void End()
    {
        save(final());
    }


    public TimedLerp(Func<T> original, Func<T> final, float timer, Func<T, T, float, T> lerp, Action<T> save) : base(timer,null,null)
    {
        this.original = original;
        this.final = final;
        this.lerp = lerp;
        this.save = save;

        update = Update;

        end = End;
    }
}


/// <summary>
/// Interfaz para representar un porcentage en la UI o afines
/// </summary>
public interface IGetPercentage
{
    float Percentage();

    float InversePercentage();

    float current { get; }

    float total { get; }
}