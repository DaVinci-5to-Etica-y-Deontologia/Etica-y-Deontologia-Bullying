using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class LinkedPool<T> where T : IPoolElement<T>
{
    public IPoolElement<T> first;

    public IPoolElement<T> last;

    public int Count { get; private set; }

    T model;

    public IEnumerator CreatePool(int cantidad, System.Action action = null)
    {
        Count = 1;

        CreateFirst();

        var wachdog = new Stopwatch();

        wachdog.Start();

        for (int i = 0; i < cantidad - 1; i++)
        {
            last.Next = model.Create();

            last = last.Next;

            last.Parent = this;

            last.inPool = true;

            if (wachdog.Elapsed.TotalMilliseconds > 1 / 60 * 1000)
            {
                wachdog.Reset();
                yield return null;
            }

            Count++;
        }

        action?.Invoke();
    }

    public T Obtain()
    {
        if (first == null)
        {
            var aux = model.Create();
            aux.Parent = this;
            return aux;
        }

        Count--;

        var ret = first;

        first = first.Next;

        ret.inPool = false;

        return (T)ret;
    }

    public void Return(IPoolElement<T> poolElement)
    {
        if (poolElement.inPool)
            return;

        poolElement.inPool = true;

        Count++;

        if (first == null)
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

    public override string ToString()
    {
        var aux = first;

        string pantalla = string.Empty;

        while (aux != null)
        {
            pantalla += "->" + aux;

            aux = aux.Next;
        }

        return pantalla + "\n" + Count;
    }

    void CreateFirst()
    {
        first = model.Create();

        last = first;

        last.Parent = this;

        last.inPool = true;
    }

    public LinkedPool(T model)
    {
        this.model = model;
    }
}

public interface IPoolElement<T> where T : IPoolElement<T>
{
    LinkedPool<T> Parent { get; set; }

    IPoolElement<T> Next { get; set; }

    T Create();

    void Destroy();

    bool inPool { get; set; }
}


