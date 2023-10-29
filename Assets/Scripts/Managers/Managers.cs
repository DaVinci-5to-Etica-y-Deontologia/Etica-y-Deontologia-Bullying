using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton pattern para los MonoBehaviour
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    public static T instance;

    protected virtual void Awake()
    {
        instance = (T)this;
    }
}

/// <summary>
/// Singleton pattern para los ScriptableObject
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonScript<T> : ScriptableObject where T : SingletonScript<T>
{
    static protected T instance;

    protected virtual void OnEnable()
    {
        instance = (T)this;
    }
}

/// <summary>
/// Singleton pattern para las clases comunes
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonClass<T> where T : SingletonClass<T>
{
    static protected T instance;

    protected SingletonClass()
    {
        instance = (T)this;
    }
}

[System.Serializable]
public struct Wrapper<T>
{
    [SerializeField]
    public T atribute;

    public Wrapper(T atribute)
    {
        this.atribute = atribute;
    }
}

[System.Serializable]
public struct WrapperReference<T>
{
    [SerializeReference]
    public T atribute;

    public WrapperReference(T atribute)
    {
        this.atribute = atribute;
    }
}



public class Manager<T> : SingletonClass<Manager<T>>
{
    Pictionarys<string, T> _pic = new Pictionarys<string, T>();

    public Manager()
    {
        //LoadSystem.AddPostLoadCorutine(() => Manager.pic.Add(typeof(T).Name, _pic.keys));
    }

    public static Pictionarys<string, C> SearchByType<C>() where C : T
    {
        Pictionarys<string, C> aux = new Pictionarys<string, C>();

        foreach (var item in pic)
        {
            if(item.Value is C)
            {
                aux.Add(item.Key, (C)item.Value);
            }
        }

        return aux;
    }

    static public Pictionarys<string,T> pic
    {
        get
        {
            if (instance == null)
                new Manager<T>();

            return instance._pic;
        }
    }
}

public class Manager : SingletonMono<Manager>
{
    [SerializeField]
    Pictionarys<string, string[]> _pic = new Pictionarys<string, string[]>();

    static public Pictionarys<string, string[]> pic
    {
        get
        {
            if (instance == null)
                GameManager.instance.gameObject.AddComponent<Manager>();

            return instance._pic;
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }
}

public interface Init
{
    void Init();
}

public interface Init<T>
{
    void Init(T param);
}