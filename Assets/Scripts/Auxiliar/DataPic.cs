using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataPic<T>
{
    [SerializeField]
    Pictionarys<int, T> data = new();

    public int lastID { get; private set; }

    public int Count => data.Count;

    public T GetTByIndex(int index)
    {
        return data.GetPicByIndex(index).Value;
    }

    public T GetTByID(int ID)
    {
        if(!data.TryGetValue(ID, out var value))
        {
            Debug.LogError("no se encontro el ID: " + ID);
            return default;
        }

        return value;
    }

    public int GetIDByIndex(int index)
    {
        return data.GetPicByIndex(index).Key;
    }

    public int GetIndexByID(int ID)
    {
        if (!data.ContainsKey(ID, out var value))
        {
            Debug.LogError("no se encontro el ID: " + ID);
            return default;
        }

        return value;
    }

    public Internal.Pictionary<int, T> Add(T value)
    {
        lastID++;

        return data.Add(lastID, value);
    }

    public Internal.Pictionary<int, T> Add(Internal.Pictionary<int, T> pictionary)
    {
        lastID++;
        return data.Add(pictionary);
    }

    public void Remove(int ID)
    {
        data.Remove(ID);
    }

    public void RemoveAt(int index)
    {
        data.RemoveAt(index);
    }
}
